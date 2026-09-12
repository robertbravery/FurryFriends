# FurryFriends — Session Change Log

**Date:** 2026-09-12
**Branch:** `011-login-jwt`
**Base commit:** `c729eaf`
**Working tree at time of writing:** uncommitted — 56 modified, 53 untracked (109 paths)
**Scope:** everything done during this session: four defects you reported, the features delivered,
and the incidental fixes that came up along the way.

> **Verification caveat — read first.** The repository **cannot be built from a command line in this
> environment** (see §6). Everything marked *"verified by inspection"* was checked by reading the
> code, balancing braces, grepping for orphaned references and confirming library signatures against
> the shipped packages — but **not by a compiler**. Items marked *"confirmed at runtime"* were
> confirmed by you in the running application.

---

## 1. Summary

| # | Change | Type | Status |
|---|---|---|---|
| 1 | Missing `GetClientByIdQuery` handler — bookings page crashed on load | Defect | Confirmed at runtime |
| 2 | Booking price hardcoded to `0.0m` — every create threw | Defect | Confirmed at runtime |
| 3 | Booking confirmation modal showed zero GUID and `0001-01-01` | Defect | Confirmed at runtime |
| 4 | Dead-end page after closing the booking confirmation | Defect / UX | Verified by inspection |
| 4b | Confirm / start / complete / no-show returned **415 Unsupported Media Type** | Defect | Diagnosed from API logs |
| 4c | `/timeslots` and `/timeslots/available` called without the `/api` prefix | Defect | Diagnosed from API logs |
| 4d | Signed-in client missing from the Booking Management client picker | Defect | Diagnosed from API logs |
| 5 | Booking lifecycle endpoints (confirm / start / complete / no-show) | Feature | Verified by inspection |
| 6 | Availability unification, Phase 1 (calculation core) | Feature | Verified by inspection |
| 7 | Availability unification, Phase 2 (availability UI) | Feature | Verified by inspection |
| 8 | `DailyPetWalkLimit` made nullable end-to-end | Feature | Verified by inspection |
| 9 | Walker availability shown in the booking walker picker | Feature | Confirmed at runtime |
| 10 | Client picker removed; client resolved from context | Feature | Confirmed at runtime |
| 11–16 | Incidental fixes (see §4) | Defect | Mixed |

---

## 2. Defects fixed

### 2.1 No handler for `GetClientByIdQuery` — the bookings page crashed

**Symptom**

```
System.InvalidOperationException: No service for type
'MediatR.IRequestHandler`2[GetClientByIdQuery, Result`1[ClientDTO]]' has been registered.
```

**Root cause** — `GetClientByIdQuery` was declared as a record in `GetClientQuery.cs` and sent by
`GetClientById.cs`, but **no handler for it existed anywhere in the solution**. Only the by-email
`GetClientQueryHandler` was implemented, so `GET /api/Clients/id/{id}` could never have worked. The
old client picker used the by-email path and masked it; removing the picker made the booking flow
always resolve the client **by id**, which walked straight into the gap.

**Fix**

| File | Change |
|---|---|
| `src/FurryFriends.UseCases/Domain/Clients/Query/GetClient/GetClientByIdQueryHandler.cs` | **new** — mirrors the by-email handler |
| `src/FurryFriends.UseCases/Domain/Clients/Query/GetClient/GetClientByIdQueryValidator.cs` | **new** — `Id` not empty |
| `src/FurryFriends.Core/ClientAggregate/Specifications/ClientByIdWithDetailsSpec.cs` | **new** — read-only spec |
| `src/FurryFriends.UseCases/Domain/Clients/Dto/ClientDtoMapping.cs` | **new** — single `Client → ClientDTO` projection |
| `src/FurryFriends.UseCases/Services/ClientService/IClientService.cs` | added `GetClientByIdAsync` |
| `src/FurryFriends.UseCases/Services/ClientService/ClientService.cs` | implemented it; returns `Result.NotFound` |
| `GetClientQueryHandler.cs` | refactored onto the shared mapper |

**Latent second bug avoided.** `ClientByIdWithPetsSpec` does **not** eager-load `BreedType.Species`,
and **no lazy-loading proxy is configured anywhere in the solution**. The DTO mapper reads
`p.BreedType.Species.Name`, so reusing that spec would have replaced the exception with a
`NullReferenceException`. The new spec includes both levels.

**Coverage check.** All request types in `src` were scanned for missing handlers. `GetClientByIdQuery`
was the only genuinely unregistered one (`SetScheduleCommand` implements plain `IRequestHandler`;
two FastEndpoints request DTOs implement FastEndpoints' `IRequest`).

**Behaviour note:** the new spec returns **active pets only**, matching `ClientByEmailSpec`. This also
closes the "deactivated pets remain bookable" issue at the API level. `ClientDTO.Pets[].IsActive` is
therefore always `true` on this path.

---

### 2.2 Booking price hardcoded to zero

**Symptom**

```
System.ArgumentException: Required input price cannot be zero or negative. (Parameter 'price')
  at FurryFriends.Core.BookingAggregate.Booking.Create(...) in Booking.cs:line 57
```

**Root cause** — `CreateBookingHandler.cs:44` passed a literal `0.0m`:

```csharp
var booking = await _bookingService.CreateBookingAsync(
    request.PetWalkerId, request.PetOwnerId, request.Start, request.End, 0.0m);
```

`Booking.Create` guards `Guard.Against.NegativeOrZero(price, nameof(price))`, so **every** booking
attempt through this endpoint was guaranteed to throw. The UI computed a price and displayed it, but
the API's `CreateBookingRequest` had no `Price` property, so the value was silently discarded on
deserialization.

**Fix — price is now server-authoritative.** The `price` parameter was **removed** from
`IBookingService.CreateBookingAsync` and the price is derived from the walker inside the service:

```csharp
var hours = (decimal)(endTime - startTime).TotalHours;
return Math.Round(hours * hourlyRate, 2);
```

Same formula and rounding as the booking form, so the displayed and stored figures agree. Removing
the parameter rather than passing the computed value means the client cannot choose the price.

| File | Change |
|---|---|
| `src/FurryFriends.UseCases/Services/BookingService/IBookingService.cs` | `price` parameter removed, documented |
| `src/FurryFriends.UseCases/Services/BookingService/BookingService.cs` | `CalculatePrice` helper; guards a missing/zero rate with a specific message |
| `src/FurryFriends.UseCases/Domain/Bookings/Command/CreateBookingHandler.cs` | no longer passes `0.0m` |
| `src/FurryFriends.Web/Endpoints/BookingEndpoints/Create/CreateBooking.cs` | **also** fixed: response echoed `result.Start` as the end date |

The `IBookingService` signature change is contained: there is exactly **one** implementation and
**one** production caller. (The BlazorUI `BookingService` is a different interface.)

---

### 2.3 Confirmation modal showed an empty booking

**Symptom** — "Booking Created Successfully!", then `ID: 00000000-0000-0000-0000-000000000000`,
`Date: Monday, January 01, 0001`, `Time: 00:00 - 00:00`. The booking itself *was* created.

**Root cause** — a client/server contract mismatch. `CreateBookingEndpoint` derives from
`BaseEndpoint<TRequest, TResponse>`, declared as:

```csharp
public abstract class BaseEndpoint<TRequest, TResponse> : Endpoint<TRequest, Result<TResponse>>
```

so the wire format is an Ardalis `Result<T>` envelope:

```json
{ "value": { "bookingId": "...", "startDate": "...", "endDate": "..." }, "status": ..., "isSuccess": true }
```

The client deserialized the **envelope** straight into the flat `BookingResponseDto`. No member
matched, so every property bound to its default — silently, with no exception.

**Provenance** — commit `b79373d` *"refactor(web): implement centralized endpoint error handling and
base class"* (2026-05-09) changed the response shape across 24 files, **none of them in the
BlazorUI**. The client was never updated. Until 2.2 was fixed no booking could be created, so the
mismatch had never been observable.

**Fix**

| File | Change |
|---|---|
| `src/FurryFriends.BlazorUI/Services/Implementation/ApiResultPayload.cs` | **new** — unwraps the `Result<T>` envelope |
| `src/FurryFriends.BlazorUI/Services/Implementation/BookingService.cs` | 3 call sites now use it |

It detects the envelope by requiring **both** a `value` member and a `status`/`isSuccess` member, so
it will not misfire on a bare payload. This matters because the codebase has **two** conventions:
`Create`, `Cancel`, `Confirm`, `Start`, `Complete` and `MarkNoShow` wrap their payload; the GET
endpoints (`GetClientBookings`, `GetPetWalkerBookings`, `GetAvailablePetWalkers`,
`GetAvailableSlots`, `CanBookTimeSlot`) return bare payloads via `SendOkAsync`. The helper accepts
both. Property matching is case-insensitive because the server's naming policy is not guaranteed.

Call sites fixed: `CreateBookingAsync`, `CancelBookingAsync`, and `PostBookingActionAsync` (which
serves confirm, start, complete and no-show — all four were broken the same way).

---

### 2.4b Confirm / start / complete / no-show returned 415 Unsupported Media Type

**Symptom** — clicking Confirm on Booking Management showed a red banner reading only
*"The request failed."* with no further detail, which made it look like a client-side problem.

**Root cause** — diagnosed from `FurryFriends.Web/Logs/web-log-20260912.txt`, where every attempt
appeared as:

```
POST https://localhost:57679/api/bookings/fc894975-.../confirm - 415 null null 10.6701ms
```

The route matched and the endpoint executed, but it returned **415 Unsupported Media Type** with an
**empty body**. `PostBookingActionAsync` was the only `PostAsync(..., null)` call in the whole
BlazorUI — the request DTO binds a body, so FastEndpoints requires a JSON content type, and posting
with no content sends none.

The empty body then produced *"The request failed."* because `ExtractErrorMessage("")` returns that
literal fallback, while `BuildErrorMessage` prefers `Errors` over `Message` — discarding the status
code that would have made the cause obvious.

**Fix**

| File | Change |
|---|---|
| `src/FurryFriends.BlazorUI/Services/Implementation/BookingService.cs` | `PostBookingActionAsync` now sends `new StringContent("{}", Encoding.UTF8, "application/json")` |
| same | empty-body failures now report `HTTP <code> <reason>` instead of the generic fallback |

This was the **only** `PostAsync(..., null)` in the codebase, so it affected all four lifecycle
actions at once. Confirmed by the log: 7 × 415, all on `/confirm`.

---

### 2.4c Legacy timeslot URLs missing the `/api` prefix

`TimeslotService` builds several URLs as absolute paths (`"/timeslots"`). Because `HttpClient`
`BaseAddress` is `…/api`, an absolute path **replaces** the base path entirely, so the request went to
`https://localhost:57679/timeslots` and 404'd. The log showed this repeatedly at 17:25–17:34.

| File | Change |
|---|---|
| `src/FurryFriends.BlazorUI/Services/Implementation/TimeslotService.cs` | `GetTimeslotsAsync` → `/api/timeslots`; `GetAvailableTimeslotsAsync` → `/api/timeslots/available` |

Both routes were verified to exist server-side (`GetTimeslotsRequest.Route`, `GetAvailableTimeslotsRequest.Route`).
Live callers: `EditPetWalkerPopup` (Schedule tab) and `ClientBooking`.

**Not fixed** — the same file has three more broken URLs, left alone deliberately:

- `GetWorkingHoursAsync` → `"/petwalkers/{id}/workinghours"` — the real route is
  `/working-hours/{petWalkerId}`, so it is wrong in both prefix *and* shape. No UI callers.
- `GetPetWalkerCustomTimeRequestsAsync` → `"/petwalkers/{id}/custom-time-requests"` and
  `GetClientCustomTimeRequestsAsync` → `"/clients/{id}/custom-time-requests"` — **no such endpoints
  exist server-side at all**, so a prefix fix would not help. See §5.

---

### 2.4d Signed-in client missing from the Booking Management client picker

**Symptom** — under Booking Management, switching **View by → Client** produced a dropdown that did
not contain the signed-in client (`cfb272ef-…`), so their bookings could not be reached at all.

**Root cause** — `BookingList.LoadIdentitiesAsync` called the parameterless `GetClientsAsync()`
overload, which internally calls `GetClientsAsync(1, 10)`:

```csharp
public async Task<List<ClientDto>> GetClientsAsync()
{
    var response = await GetClientsAsync(1, 10);   // only the first 10 clients
    ...
}
```

The endpoint orders by `Name.FirstName`, so the picker held only the first ten clients
alphabetically and the signed-in client was usually not among them. The pet walker branch of the
same method already asked for 200, so the two sides disagreed. The API log confirms it — every
picker load was `GET /api/Clients/list?page=1&pageSize=10`, and the clients the user did manage to
select (`dbb81474-…`, `a1808576-…`) are both different from `cfb272ef-…`.

**Fix** — `src/FurryFriends.BlazorUI.Client/Pages/Bookings/BookingList.razor.cs`

| Change | Detail |
|---|---|
| Client branch now pages properly | `GetClientsAsync(1, IdentityPageSize)` instead of the 10-capped overload |
| New `IdentityPageSize = 200` const | Both branches (clients and pet walkers) use it, so they can no longer drift |
| Defaults to the signed-in client | On load, and on switching to the Client view, `_selectedIdentityId` is resolved from `IClientContextService` and the bookings load immediately |
| New `ResolveCurrentClientIdAsync` | Returns `Guid.Empty` when no client context exists, in which case the picker behaves exactly as before |

The default is the substantive part: the user no longer has to find themselves in the picker at all.

**Left alone** — the parameterless `GetClientsAsync()` overload still caps at 10. After this change
it has **no remaining callers**, so nothing is broken, but it stays a trap for the next person.

---

### 2.4 Dead-end page after closing the confirmation

**Symptom** — after the confirmation modal, closing it returned you to the "Book a Walk" page with
the walker selected, step 2 highlighted and an empty date picker.

**Root cause** — two interacting problems:

1. `CloseSuccessModal()` only hid the modal; it never navigated anywhere.
2. `BookingFormComponent.SubmitBooking` ran **two seconds after submitting, while the modal was
   still open**:

   ```csharp
   await OnBookingCompleted.InvokeAsync(response.Data);
   await Task.Delay(2000);
   ResetForm();
   ```

   That cleared the form underneath. The parent then immediately re-adopted the walker and
   auto-advanced to step 2 (`OnParametersSetAsync`), producing exactly the half-empty state seen.

**Fix**

| File | Change |
|---|---|
| `BookingManagement.razor.cs` | `CloseSuccessModal` now navigates to `/bookings/manage/{clientId}`; split out `HideSuccessModal`; added `IClientContextService` injection and `ResolveClientIdAsync` |
| `BookingManagement.razor` | `@key="_formKey"` on the form; "Close" relabelled **"View My Bookings"** (icon `fa-list`) |
| `BookingFormComponent.razor.cs` | removed the `Task.Delay(2000)` + `ResetForm()`; deleted the now-dead `ResetForm` |

**Resulting behaviour**

| Action | Behaviour |
|---|---|
| **View My Bookings** | `/bookings/manage/{clientId}` — the new booking, its status and its actions |
| Click outside the modal | same |
| **Create Another Booking** | stays put; `_formKey++` makes Blazor rebuild the form so the previous pet, date and time cannot leak forward; the walker is retained |

Navigating to `/bookings/manage/{clientId}` rather than bare `/bookings/manage` matters because that
page needs an identity selected before it loads anything. `ClientId` comes from the route when
present, otherwise from `IClientContextService`; if neither resolves (Production) it falls back to
`/bookings/manage` and you pick a client.

---

## 3. Features delivered

### 3.1 Booking lifecycle endpoints (Track 1 #1)

`Booking` and `BookingService.UpdateBookingStatusAsync` already implemented all transitions; only
the endpoints and commands were missing.

- **UseCases** — `Confirm`, `Start`, `Complete`, `MarkBookingNoShow` × `{Command, Handler, Validator}`
  (12 files) in `Domain/Bookings/Command/`, plus `Dto/BookingStatusDto.cs`.
- **Web** — `Endpoints/BookingEndpoints/{Confirm,Start,Complete,MarkNoShow}/` and
  `Records/BookingStatusResponse.cs`. Routes `POST /bookings/{bookingId}/{confirm|start|complete|no-show}`.
- **Flag for "walker has no schedule"** — `Domain/PetWalkers/Query/GetSchedule/HasPetWalkerAvailability{Query,Handler}.cs`.
- **Tests** — `UseCase/Bookings/{Confirm,Start,Complete,MarkBookingNoShow}Tests.cs` using real
  `Booking` aggregates; `BlazorUI/BookingServiceTests.cs`; `BlazorUI/BookingStatusRulesTests.cs`;
  `FunctionalTests/BookingEndpoints/GetBookingsDateBindingTests.cs`.
- **UI** — `BookingList` at `/bookings/manage` with status chips and per-status actions governed by
  the shared `BookingStatusRules` (mirrors the domain state machine).

### 3.2 Availability unification — Phase 1 (calculation core)

Two duplicate availability systems were unified. **The legacy tables were retained deliberately** on
your instruction: `Timeslot`, `WorkingHours`, `TravelBuffer` and `CustomTimeRequest` remain, and
`GetAvailabilityHandler` still honours legacy `Booked`/`Unavailable` timeslot rows as blocks.

- `src/FurryFriends.Core/PetWalkerAggregate/Services/AvailabilityCalculator.cs` — pure static
  `Calculate(...)` returning `IReadOnlyList<(DateTime Start, DateTime End)>`; plus
  `AvailabilityDateOverride`.
- `src/FurryFriends.Core/PetWalkerAggregate/PetWalkerDateException.cs` and
  `Specifications/PetWalkerDateExceptionSpecifications.cs`.
- `PetWalker` — new `SlotDurationMinutes`, `InterBookingGapMinutes`, `HasAnySchedule`; guards
  (15–480 and 0–240); defaults `DefaultDailyPetWalkLimit = 8`, `DefaultSlotDurationMinutes = 60`,
  `DefaultInterBookingGapMinutes = 0`.
- `GetAvailabilityHandler.cs` rewritten onto the calculator; excludes `Cancelled`/`NoShow` from
  blocking.
- Migration `20260912115905_AddPetWalkerAvailabilitySettings` — new `PetWalkerDateExceptions` table,
  two new columns, default on `DailyPetWalkLimit`. **No destructive changes.**
- Tests — `Core/PetWalkerAggregateTests/AvailabilityCalculatorTests.cs` (25 tests).

### 3.3 Availability unification — Phase 2 (walker availability UI)

- **UseCases** — `Dto/PetWalkerAvailabilityDtos.cs`; `Command/Availability/`
  (`SetPetWalkerDateException`, `DeletePetWalkerDateException`, `UpdatePetWalkerAvailabilitySettings`);
  `Query/GetAvailability/` (two queries + handlers).
- **Web** — `Endpoints/PetWalkerEndpoints/Availability/` — GET/POST/DELETE `date-exceptions` and
  GET/PUT `availability-settings`.
- **BlazorUI** — `Components/Pages/Timeslots/PetwalkerAvailability.{razor,.razor.cs,.razor.css}` with
  four tabs (Weekly Schedule / Date Exceptions / Slot Settings / Custom Requests); inline `<style>`
  moved to scoped CSS using the existing design tokens. `IScheduleService` gained five methods.
  NavMenu: added **Availability** under Pet Walkers; **Bookings** now "Book a Walk" (`/bookings`) and
  "Booking Management" (`/bookings/manage`); removed the "Booking Test" menu entry.

**Phase 3 was deferred as unverified and cross-cutting.** Still open: a batch availability-summary
endpoint to replace the N+1 sequential probe; reconciling `EditPetWalkerPopup`'s Schedule tab with
`/petwalker/availability`; surfacing `DailyPetWalkLimit` on the create/edit forms.

> **The Custom Requests tab of this page cannot work today** — it calls
> `GET /petwalkers/{id}/custom-time-requests`, which does not exist on the API. See §5 item 10.

### 3.4 `DailyPetWalkLimit` made nullable end-to-end

`int` → `int?` across the create/update requests, commands and DTOs. The old
`ResolveDailyPetWalkLimit` fallback helper was removed; both service paths now use
`if (dto.DailyPetWalkLimit is { } x) petWalker.UpdateDailyPetWalkLimit(x);` and the validator narrows
with `.When(x => x.DailyPetWalkLimit.HasValue)`. This preserves three states — omitted (keep/use
default), explicit `0` (no bookings that day), explicit positive value.

Tests: `UseCase/PetWalkerTests/PetWalkerServiceDailyWalkLimitTests.cs` (6 tests, including the
explicit-zero case). `DailyPetWalkLimit = 0` is legitimate because
`BookingValidationRules.ValidateDailyBookingLimit` rejects when `bookingsOnDate >= limit`.

### 3.5 Availability shown in the booking walker picker

`PetWalkerSelectionComponent` gained `SelectedDate`/`SelectedDateChanged`, a date picker, and a
sequential availability probe (`_availability` → `WalkerAvailability(IsAvailable, HasSchedule, SlotCount)`)
with four distinct states and a `.no-availability` dimming style. `CanBeSelected` prevents picking a
walker with nothing free. `BookingFormComponent` passes `SelectedDate` down.

### 3.6 Client picker removed; client resolved from context

**No default client was configured before this session.** `ClientContextService` returned
`Guid.Empty` and `SetCurrentClientIdAsync` had **no callers**; no `appsettings*.json` contained the
test id.

- `src/FurryFriends.BlazorUI/appsettings.Development.json` — added
  `"Dev": { "CurrentClientId": "cfb272ef-ebb9-4716-91eb-2ae26d5768df" }`.
- `ClientContextService` — reads it as a **fallback only** (an explicitly set client still wins) and
  logs once when the fallback is active.
- `Program.cs` — Development → `ClientContextService`; everything else → new
  `NoClientContextService`, which always returns `Guid.Empty` so a testing default can never become
  a production identity.
- `BookingFormComponent` — the client dropdown was removed along with `LoadClientsAsync`,
  `OnClientSelectedAsync`, `_clients` and `_isLoadingClients`. The owner resolves from the `ClientId`
  parameter (deep links) or the client context, nothing else. If neither resolves, the details step
  says a booking cannot be made rather than showing a picker that should not exist.

---

## 4. Incidental fixes

| # | Issue | Fix | File(s) |
|---|---|---|---|
| 11 | `The type initializer for 'FastEndpoints.RequestBinder\`1' threw an exception` — `[FromQuery]` on **scalar** properties (`DateTime`). Scalars bind from query by convention; the attribute is only for complex types. | Removed `[FromQuery]` from `CanBookTimeSlotRequest`, `GetClientBookingsRequest`, `GetPetWalkerBookingsRequest`, `GetAvailableSlotsRequest`. Verified zero remain in `FurryFriends.Web`. | `Endpoints/BookingEndpoints/**` |
| 12 | "No available time slots" — the date input never triggered a reload, so `LoadAvailableSlotsAsync` ran **zero** times. | Added `@bind:after="OnDateChanged"` to the date input. | `DateTimeSelectionComponent.razor` |
| 13 | `Unable to resolve service for type 'JwtTokenService'` at Web startup — the JWT feature was half-implemented. | `RegisterAuthenticationServices`: `Configure<JwtSettings>` with a fallback key + `AddSingleton<JwtTokenService>()`; added a `Jwt` section to `FurryFriends.Web/appsettings.json`. Verified with a DI probe. | `Infrastructure/InfrastructureServiceExtensions.cs`, `Web/appsettings.json`, `Web/Program.cs` |
| 14 | Intermittent `TaskCanceledException` / `SocketException(995)` on `GET /api/Clients/id/{id}` from a stale pooled TLS connection, which surfaced as "this client has no pets". | `ClientService` retries once on transient transport failure (`IOException` / `HttpRequestException` / inner `IOException`) on a fresh connection. A failed lookup is now reported as a failure, not as "no pets". | `BlazorUI/Services/Implementation/ClientService.cs` |
| 15 | Missing `using MediatR;` in the new availability query (UseCases has no MediatR global using). | Added. | `HasPetWalkerAvailabilityQuery.cs` |
| 16 | `FurryFriends.UseCases.csproj` was truncated to 0 bytes and later lost its BOM during an editing mishap while your app held the file locked. | Repaired; verified 599 bytes with BOM and no git diff. **No `.csproj`, `.props` or `.sln` file is modified in the working tree.** | `FurryFriends.UseCases.csproj` |

---

## 5. Known issues — found but NOT fixed

These are real and will bite later. Each is listed with where it is.

1. **`PetId` is never stored on a booking.** The booking form *requires* you to select a pet
   (`IsBookingDetailsValid` checks `PetId != Guid.Empty`) and sends it, but `Booking` has **no
   `PetId` property** — no column, no field, nothing in `BookingConfiguration`. Every booking is
   recorded against a client but not a pet. Fixing this needs a schema change and a migration.
2. **The two legacy reflection paths still create £0 bookings.** `BookTimeslotHandler.cs:91` and
   `RespondToCustomTimeRequestHandler.cs:142` bypass `Booking.Create` entirely and set `Price = 0m`
   by reflection — the same technique that dodged the guard which broke the main path. Unaffected by
   the §2.2 fix.
3. **Ratings have the same envelope mismatch as §2.3.** The five Rating endpoints derive from
   `BaseEndpoint`, and `RatingService` reads them flat:
   `GetRatingSummaryAsync` silently binds to defaults (averages and counts read as zero), and
   `GetRatingsAsync` deserializes a `List<T>` from an object root, throws, and is swallowed by its
   `catch` so the UI shows no ratings. `ApiResultPayload` can be reused; it needs the same two-line
   change.
4. **Double-booking is still a check-then-act race.** No DB constraint, no concurrency token anywhere
   in `Data/Config/`. This is backlog item **T1-3**.
5. **`BookingService.GetAvailableTimeSlotsAsync`** retains its own 1-hour slot loop and has no
   callers. Dead code.
6. **Legacy surfaces retained and still routable**: the whole timeslot/working-hours surface —
   `POST /timeslots`, `GET /timeslots`, `DELETE /timeslots/{TimeslotId}`,
   `PUT /timeslots/{TimeslotId}`, `GET /timeslots/available`, `POST /timeslots/{TIMESLOTID}/book`,
   `POST /timeslots/request-custom`, `POST /timeslots/respond-custom-time/{RequestId}`, and
   `/working-hours*` — plus the `BookingTest.razor` page at `/booking-test` (removed from NavMenu,
   still reachable by URL).
7. **`DailyPetWalkLimit` is still absent from the pet-walker create form and `EditPetWalkerPopup`.**
   It is currently settable only via Availability → Slot Settings, or SQL.
8. **`FurryFriends.AspireHost` is an empty shell** — both `AddProject` calls commented out, despite
   `AGENTS.md` calling it the primary entry point.
9. **No test suite was run at any point in this session.** `dotnet test` has never been executed here.
   Several of the defects above would have been caught by it. Test files were written but never run.
10. **The Availability page's Custom Requests tab cannot work — its endpoint does not exist.**
    `PetwalkerAvailability.razor.cs:464` calls
    `TimeslotService.GetPetWalkerCustomTimeRequestsAsync(_selectedPetWalkerId, status)`, which GETs
    `/petwalkers/{id}/custom-time-requests`. The API exposes only
    `POST /timeslots/request-custom` and `POST /timeslots/respond-custom-time/{RequestId}` — there is
    **no list endpoint for either pet walker or client**, so the tab always fails and shows an error.
    Confirmed in the API log: `GET /petwalkers/12706b7f-.../custom-time-requests - 404` at 17:30 and
    17:34. Fixing this needs a new query + handler + endpoint (`CustomTimeRequest` is a keeper
    feature), not a URL correction. The sibling `GetClientCustomTimeRequestsAsync` has no UI callers
    but the same gap.

---

## 6. Verification status and how to verify

### The build environment is broken

Three separate problems, all pre-existing:

1. `FurryFriends.Core.csproj` line 2 declares the obsolete
   `<Sdk Name="Microsoft.Build.CentralPackageVersions" Version="2.1.3" />`. Under SDK 9 it cannot
   resolve offline; the package *is* in the NuGet cache and resolves once `NUGET_PACKAGES` is set
   explicitly. It is redundant — `Directory.Build.props` already sets
   `ManagePackageVersionsCentrally=true`.
2. `global.json` requests `9.0.100-rc.2.24474.11` with `rollForward: latestMajor`, so plain
   `dotnet build` selects **SDK 10.0.301**, whose workload-resolver SDK directories do not exist →
   `MSB4276` fails nested project-reference evaluation. A 9.0.305 SDK *is* installed.
3. Even with both worked around, the nested `MSBuild` task in
   `_GetProjectReferenceTargetFrameworkProperties` returns `FAILED` with **zero diagnostics**.

Consequently **no compile was possible for `FurryFriends.BlazorUI` or `FurryFriends.Web`**, and the
project files were frequently locked by your running app.

### What *was* verified

- `FurryFriends.BlazorUI.Client` compiled clean (0 errors) at one point via `-t:CoreCompile`.
- Brace/paren balance on every edited file; grep for orphaned references after each removal
  (`ResetForm`, the client picker members, `IClientService`/`IScheduleService` implementers).
- Library signatures confirmed against the **shipped packages** rather than from memory:
  `Ardalis.GuardClauses` 5.0.0 XML docs for the 3-argument `Null`/`NegativeOrZero` overloads;
  `Ardalis.Result` source for `public T Value { get; init; }` (a plain auto-property, so the
  endpoint's `result.Value is null` check cannot throw).
- Caught that `FurryFriends.Core.ValueObjects` is not in Core's global usings, so
  `nameof(Compensation.HourlyRate)` would not have compiled.
- Confirmed **one** implementation and **one** production caller before changing the
  `IBookingService.CreateBookingAsync` signature.
- Confirmed all 55 request types in `src` have registered handlers.
- Working tree: **no `.csproj`, `.props` or `.sln` modifications.**

### Recommended verification steps

1. Fix the build (§6 problems 1–2) or build in Visual Studio.
2. `dotnet build FurryFriends.sln`, then **run the test suite** — it has never been run.
3. Restart the API and BlazorUI.
4. Walk the flow: select a walker → pick a date → **available slots must appear** → pick a slot →
   details step shows **no client dropdown** and the pet list for the dev client → submit → the
   confirmation must show a real GUID, `Monday, September 14, 2026` and `10:00 - 11:00`.
5. Click **View My Bookings** → the new booking must be listed with a status chip.
6. Exercise **Confirm → Start → Complete** on it. These four lifecycle actions went through the
   §2.3 unwrap fix and have not been confirmed at runtime.

**Log lines to confirm the client context is working:**
`falling back to the development client cfb272ef-... from Dev:CurrentClientId`,
`Loading pets for client: "cfb272ef-..."`, `Successfully loaded 1 pets`.

---

## 7. Configuration and data changes requiring action

| Change | Action needed |
|---|---|
| `appsettings.Development.json` → `Dev:CurrentClientId` | Dev-only convenience that **bypasses identity**: any signed-in user gets the configured client's pets, and the API endpoints remain anonymous. Fine for development; remove when authentication lands. `ClientContextService`'s two methods and `NoClientContextService` are the only places that need to change. |
| `FurryFriends.Web/appsettings.json` → `Jwt` section | Added with `ExpirationHours: 8`. The secret currently falls back to a default — replace before any non-local use. |
| Migration `20260912115905_AddPetWalkerAvailabilitySettings` | Applied by you. Additive only: new `PetWalkerDateExceptions` table, two new columns, default on `DailyPetWalkLimit`. |
| Legacy tables `Timeslots` / `WorkingHours` / `TravelBuffers` | **Retained by your instruction** for a possible future detailed functional requirement. |
