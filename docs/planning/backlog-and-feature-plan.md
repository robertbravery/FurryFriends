# FurryFriends — Backlog & Feature Plan

**Purpose:** input for writing specifications and user stories.
**Source:** the capability review and the revised (security-deferred) feature plan produced earlier
in this work session, condensed here.
**Date:** 2026-09-12
**Status of this document:** planning input, not a commitment. Items already delivered are marked
✅ so you do not write specs for work that is done.

---

## How to use this document

Each item below is written to be promoted straight into a spec:

- **ID** — stable reference for linking specs, stories and branches.
- **Complexity** — `S` ≈ hours, `M` ≈ a few days, `L` ≈ a week or more. Indicative only.
- **Why** — the justification, which is usually the *value* statement of the epic.
- **Spec focus** — the open questions and acceptance criteria that the spec must answer.

Suggested epic naming: `EP-<nn> <Track> <Feature>` and stories `US-<nnn>` linked back to the ID.

### Definition of ready (apply to every item)

A spec is ready to implement when it states: the actor, the trigger, the happy path, at least two
failure paths, the data/ownership rules, the endpoint contract (route, verb, request, response,
status codes), and how it will be tested. See **§6** for the decisions that block several items.

---

## 1. Baseline — what exists, and the gaps that matter

**Working end-to-end:** Client CRUD (pets, breeds, species, service areas); PetWalker CRUD
(profile, hourly rate, photos, schedule, service areas); location lookup; booking create + cancel;
timeslots and working hours; custom time requests; ratings with recalculation via domain event;
user registration/login; Serilog + OpenTelemetry + health checks.

### The five gaps worth fixing before adding features

| # | Gap | Detail |
|---|---|---|
| G1 | **Every endpoint is anonymous** | All `Configure()` methods call `AllowAnonymous()` — some annotated `// Replace with proper authorization`. No `AddAuthentication`, no `AddAuthorization`, no `[Authorize]` anywhere. Anyone who can reach the API can delete any client, change any walker's rate, or read every customer record. |
| G2 | ~~Bookings are created at £0~~ ✅ **FIXED 2026-09-12** | `CreateBookingHandler` hardcoded `0.0m`. Price is now derived server-side from the walker's hourly rate × booked duration. |
| G3 | **The booking lifecycle was unreachable** ✅ **FIXED** | Confirm/start/complete/no-show now have commands, handlers, validators and endpoints. |
| G4 | **Double-booking is a check-then-act race** | `BookingService.CreateBookingAsync` reads existing bookings, guards in memory, then inserts — no DB constraint, no concurrency token anywhere in `Data/Config/`. Two simultaneous requests for the same slot both succeed. |
| G5 | **Ratings and reviews have no verification** | `Rating` has `PetWalkerId` and `ClientId` but **no `BookingId`**, so nothing ties a review to a completed booking. A migration named `T001_RatingDenormalization_..._RemoveBookingId` suggests this link existed and was removed — worth understanding before re-adding. |

Still missing outright: **payments** (no code — only `docs/features/payment-processing.md`),
**notifications/email** beyond registration (`IEmailSender` is used only by `RegisterCommandHandler`),
**in-app messaging**, **pet photos**, **search/filtering** beyond list-by-location, and **any
background/scheduled work** (no `BackgroundService` or `IHostedService` in the solution).

---

## 2. Revised feature plan (security deferred)

The goal for the MVP shifts from "trustworthy" to "**feels complete and delivers the magic moment
quickly**." For a pet-walking MVP the magic moment is:

> *find a nearby walker → book → get photos and a report of the walk → rate it.*

### Deferring security — the one concession

Deferring **authentication and authorization enforcement** is a legitimate MVP trade. But three
things are *not* security and should not be deferred with it:

1. **Ownership columns in the data model.** `Booking` has `PetOwnerId`/`PetWalkerId`, but `Rating`
   has no `BookingId` and most mutations have no owner field. Adding ownership *after* real rows
   exist means backfill migrations and guesswork.
2. **Pricing correctness** — ✅ done.
3. **Concurrency** — double-booking is a *correctness* bug that a demo exposes the moment anyone
   opens two browser tabs.

One small abstraction — an `ICurrentUser` service returning a configured/stubbed user id, wired into
handlers but **not enforced by the pipeline** — gets most of the later refactor for about a day.

> **Note:** a related abstraction now exists as `IClientContextService` (config-driven dev default,
> `NoClientContextService` outside Development). It is a partial realisation of this idea and is a
> reasonable place to start `ICurrentUser` from.

---

### Track 1 — Harden what you already have

This is where the value is.

| ID | Item | Complexity | Status | Why | Spec focus |
|---|---|---|---|---|---|
| T1-1 | **Booking lifecycle endpoints** — confirm / start / complete / no-show | S | ✅ Done | `Booking` and `BookingService.UpdateBookingStatusAsync` already implement all five transitions; only endpoints and commands were missing. Completes the core loop. | Transition rules per status; who may trigger each; idempotency on repeat calls. |
| T1-2 | **Server-side price calculation** | S | ✅ Done | `CreateBookingHandler` hardcoded `0.0m`. Derive from walker rate × duration. Also unblocks payments. | Rounding; currency; what happens when a walker has no rate. |
| T1-3 | **Double-booking prevention** | M | Open | Unique constraint on walker+timeslot, or `rowversion` + retry. Do this before any demo. | Constraint vs. optimistic concurrency; retry policy; the error the client sees. |
| T1-4 | **`Rating` tied to a `BookingId`** | S | Open | Add the column and FK **now**. Enforcing "one review per completed booking" can wait; the schema cannot. Note the earlier `RemoveBookingId` migration. | Was it removed deliberately? One review per booking? Which statuses qualify? |
| T1-5 | **`ICurrentUser` abstraction + stub** | S | Open | Not enforcement — just plumbing. Returns a configured user id today, a real principal later. Turns the security work into a swap, not a rewrite. | Where it is injected; what happens with no user; how it is configured per environment. |
| T1-6 | **Image upload validation** | S | Open | `wwwroot/photos` accepts whatever is posted. Size/type/magic-byte checks and a non-guessable filename. Prevents an embarrassing demo. | Max size; allowed types; where files live; how orphans are cleaned. |
| T1-7 | **Domain event handlers + outbox** | M | Open | `BookingConfirmedEvent`/`Started`/`Completed`/`NoShow` are registered with **no handlers**. In-process dispatch inside `SaveChangesAsync` loses events on failure. An outbox now means notifications later are *a handler*, not a refactor. | Which events need handlers; at-least-once semantics; retry/poison handling. |
| T1-8 | **Seed/demo data quality** | S | Open | Realistic walkers, photos, schedules and history make the MVP look like a product. Underrated. | What a credible dataset contains; how it is applied and reset. |

### Track 2 — New features for MVP completeness

Ordered; the ordering is the recommendation, not just priority.

| ID | Feature | Complexity | Why it's MVP-critical | Spec focus |
|---|---|---|---|---|
| T2-9 | **Search, filters, sort & paging** | L | Discovery is the first thing a user does. `ListPetWalkerByLocation` alone is not a search experience. | Date-range availability, price range, distance, service type, rating sort; result shape and paging contract. |
| T2-10 | **Transactional notifications** — booking confirmed, reminder, walk started/completed, review request | M | An MVP that doesn't confirm a booking feels broken. Flows straight out of T1-7. | Channels (email first); templates; opt-out; what happens on send failure. |
| T2-11 | **Walker onboarding + availability setup** | M | A walker needs a guided setup (profile, service area, rates, schedule). Without it you cannot populate supply. | Steps and required fields; simple admin approval toggle, not full verification. |
| T2-12 | **Pet profiles v2** — photos + medical/dietary/temperament notes | M | Medical conditions, dietary restrictions and special needs are already modelled — they are just not surfaced usefully to the walker at booking time. | What the walker sees and when; who may edit; attachment limits. |
| T2-13 | **Walk report with photos** — **the magic moment** | L | Check-in/check-out, GPS trace, notes and photos. This is what makes a client rebook and what generates genuine reviews. Highest perceived-value item on this list. | Trigger points; report contents; photo handling; visibility rules. |
| T2-14 | **Cancellation & reschedule with a simple policy** | M | Free until 24h, then a fee. `Cancel(reason)` has no policy today; both sides need reschedule. | Windows and fees; who may cancel; refund semantics deferred with payments. |
| T2-15 | **Favourites & one-tap rebook** | S | Cheapest retention lever in the backlog — repeat bookings are the marketplace's economics. | Where favourites surface; rebook prefill and conflict handling. |
| T2-16 | **Recurring bookings** (weekly walks) | L | Most walking demand is recurring, not one-off. Big differentiator vs. one-off competitors. | Series definition; per-occurrence edits; how the daily limit and gaps apply. |
| T2-17 | **Calendar sync** (ICS export / Google) | M | Large time-saver for walkers; drives supply-side loyalty. | One-way vs. two-way; refresh cadence; conflict source of truth. |
| T2-18 | **Minimal admin back-office** | M | View users, bookings, disputes; suspend a walker. Even a rough one stops you editing the database by hand. | Who has access; what is audited; suspend/reinstate behaviour. |

### Track 3 — Deliberately deferred

**Payments** (use "settle with your walker directly" for v1 — removes the single largest piece of
scope and is genuinely common for MVP marketplaces), full verification/background checks, in-app
messaging, subscriptions, dynamic pricing, multi-currency, native mobile, AI matching, analytics.

> **Watch item:** messaging is the one to monitor. If test users start asking *"how do I ask the
> walker a question?"*, that is the signal to promote it from Track 3 into Track 2.

---

## 3. Full ranked backlog (wider view)

Kept for the items Track 1/2 deliberately park. Complexity added for sizing.

### P0 — Fix before adding anything

| # | Item | Complexity | Note |
|---|---|---|---|
| 1 | Authentication + authorization (JWT/OIDC, roles `Client`/`PetWalker`/`Admin`, ownership checks) | L | Removes total data exposure; blocks review/booking spoofing as a side effect. **Deferred by decision**, but see T1-5 and G1. |
| 2 | Real booking pricing | S | ✅ Done |
| 3 | Double-booking prevention at the DB level | M | = T1-3 |
| 4 | Booking lifecycle endpoints | S | ✅ Done |
| 5 | Rate limiting + idempotency keys on POST | M | Cheap now, prevents abuse and duplicate bookings/charges later. |

### P1 — Core product completeness (weeks 1–6)

| # | Item | Complexity |
|---|---|---|
| 6 | Payment & payout flow (card-on-file, capture on completion, cancellation fees, walker payouts) | L |
| 7 | Transactional notifications (via existing domain events) — = T2-10 | M |
| 8 | Verified reviews tied to completed bookings — = T1-4 + enforcement | M |
| 9 | Real search & availability — = T2-9 | L |
| 10 | Walker onboarding & verification | M |
| 11 | Cancellation & reschedule policy engine — = T2-14 | M |
| 12 | In-app messaging (own aggregate, not a chat bolt-on) | L |
| 13 | Walk tracking / live status | L |

### P2 — Growth & retention (weeks 6–14)

| # | Item | Complexity |
|---|---|---|
| 14 | Recurring & multi-day bookings — = T2-16 | L |
| 15 | Pet profiles v2 — = T2-12 | M |
| 16 | Favourites & rebooking — = T2-15 | S |
| 17 | Trust & safety — incident reporting, emergency contacts, insurance expiry | M |
| 18 | Admin back-office — = T2-18 | M |
| 19 | Referral & loyalty programme (`ReferralSource` enum already exists) | M |
| 20 | Calendar sync — = T2-17 | M |

### P3 — Scale & differentiation (quarter 2+)

Multi-pet bookings & group walks with per-pet pricing · analytics dashboards · subscriptions &
prepaid packages · dynamic pricing · route optimisation · internationalisation & multi-currency ·
native mobile (.NET MAUI) · AI-assisted matching · insurance & add-on products.

---

## 4. Non-functional requirements

Informational; no rating. Fold into the specs above rather than tracking separately.

- **Event delivery robustness** — with real notifications and payments hanging off domain events,
  in-process dispatch inside `SaveChangesAsync` will lose events on failure. Add an **outbox table**
  + background publisher. (Pairs with T1-7.)
- **Audit trail** — who changed what, when. `AuditableEntity` exists; extend it to immutable audit
  records for money and identity changes. (Pairs with T2-18.)
- **Data protection / GDPR** — PII classification, right-to-erasure (replaces the current hard
  `DeleteClient`), retention for walk reports and GPS traces, consent capture. (Pairs with T2-13.)
- **Concurrency strategy** — apply the T1-3 pattern to timeslots, service areas and ratings too.
- **Observability** — propagate a correlation ID from BlazorUI through the API into logs; each side
  currently logs independently.
- **Testing** — race-condition tests against real SQL Server for the double-booking path, contract
  tests for events, end-to-end booking journeys.
- **Deployment** — `FurryFriends.AspireHost/Program.cs` is **still an empty shell** (both
  `AddProject` calls commented out) despite `AGENTS.md` calling it the primary entry point. Either
  wire it up or correct the docs.
- **Local build ergonomics** — see §5; the repository does not currently build from a clean
  command-line `dotnet build`.

---

## 5. Suggested sequencing

| Sprint | Items | Outcome |
|---|---|---|
| **1 — Complete the core loop** | T1-1 ✅, T1-2 ✅, T1-3, T1-4, T1-5 | book → confirm → walk → complete → review, fully working |
| **2 — Discoverable and alive** | T2-9, T2-10, T1-7, T1-8 | users can find walkers and hear about their bookings |
| **3 — Worth rebooking** | T2-11, T2-12, T2-13 | supply onboarding, better pets, and the magic moment |
| **4 — Retention** | T2-14, T2-15, T2-16 | cancellations handled, repeat bookings become easy |

**Sprint 1 is mostly done** — T1-1 and T1-2 landed on 2026-09-12. Remaining: T1-3, T1-4, T1-5.

Also worth doing while in Sprint 1: fix `FurryFriends.AspireHost` and the command-line build
(see §6).

---

## 6. Decisions needed before writing specs

These block or shape several items. Each needs an answer from you, not from the code.

1. **Rating ↔ Booking** — `BookingId` appears to have existed and been removed by migration
   `T001_RatingDenormalization_..._RemoveBookingId`. Was that deliberate denormalisation? Do we
   re-add the link, and is it one review per booking?
2. **Authentication timing** — the plan defers enforcement. Is T1-5 (`ICurrentUser` stub) the agreed
   concession, given `IClientContextService` already exists as a dev-only stand-in?
3. **Payments** — confirm "settle with your walker directly" for v1, or bring payments into scope?
4. **Currency** — `Compensation` carries a currency but `Booking` stores only a bare `Price`
   decimal. Multi-currency is P3; does `Booking` need a currency column before then?
5. **Booking ↔ Pet** — `Booking` currently has **no `PetId`**, although the booking form requires
   you to choose a pet. Every booking is recorded against a client but not a pet. Decide the schema
   change (this needs a migration and is a prerequisite for T2-9 dog-size filters and T2-12).
6. **Command-line build** — `global.json` requests SDK `9.0.100-rc.2` with
   `rollForward: latestMajor`, which selects SDK 10, whose workload-resolver SDK directories are
   missing. `FurryFriends.Core.csproj` additionally declares the obsolete
   `<Sdk Name="Microsoft.Build.CentralPackageVersions" Version="2.1.3" />`, which cannot resolve
   offline. Decide whether to remove that line (redundant: `Directory.Build.props` already sets
   `ManagePackageVersionsCentrally`) and pin `global.json` to an installed SDK.
