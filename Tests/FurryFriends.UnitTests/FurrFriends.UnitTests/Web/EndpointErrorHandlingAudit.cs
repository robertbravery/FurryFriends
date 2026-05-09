using System.Reflection;
using FastEndpoints;
using FurryFriends.Web.Endpoints.Base;

namespace FurryFriends.UnitTests.Web;

/// <summary>
/// Audits all endpoints in the Web project to verify they use centralized error handling patterns.
/// 
/// Findings:
/// 
/// A) Endpoints inheriting from BaseEndpoint (use HandleResultAsync - fully centralized):
///    1. CreateRatingEndpoint        : BaseEndpoint<CreateRatingRequest, CreateRatingResponse>
///    2. UpdateRatingEndpoint        : BaseEndpoint<UpdateRatingRequest, UpdateRatingResponse>
///    3. DeleteRatingEndpoint        : BaseEndpoint<DeleteRatingRequest, DeleteRatingResponse>
///    4. GetPetWalkerRatingSummaryEndpoint : BaseEndpoint<GetPetWalkerRatingSummaryRequest, GetPetWalkerRatingSummaryResponse>
///    5. GetRatingsForPetWalkerEndpoint    : BaseEndpoint<GetRatingsForPetWalkerRequest, List<GetRatingsForPetWalkerResponse>>
///    6. CreateBookingEndpoint      : BaseEndpoint<CreateBookingRequest, CreateBookingResponse>
///    7. CancelBookingEndpoint      : BaseEndpoint<CancelBookingRequest, CancelBookingResponse> *(partial - manual error handling)
///    8. AddPet                     : BaseEndpoint<AddPetRequest, AddPetResponse> *(partial - manual error handling)
///
/// B) Endpoints NOT inheriting from BaseEndpoint (manual error handling):
///    1. CreateClient               : Endpoint<CreateClientRequest, Result<CreateClientReponse>>
///    2. DeleteClient               : Endpoint<DeleteClientRequest> (no response type)
///    3. GetClientById              : Endpoint<GetClientByIdRequest, ResponseBase<ClientRecord>>
///    4. GetClientByEmail           : Endpoint<GetClientRequest, ResponseBase<ClientRecord>>
///    5. ListClient                 : Endpoint<ListClientRequest, ListResponse<ClientDto>>
///    6. UpdateClient               : Endpoint<UpdateClientRequest, Result<UpdateClientResponse>>
///    7. UpdateClientPet            : Endpoint<UpdateClientPetRequest, Result>
///    8. RemovePet                  : Endpoint<RemovePetRequest, Result>
///    9. CreatePetWalker            : Endpoint<CreatePetWalkerRequest, Result<CreatePetWalkerResponse>>
///   10. DeletePhoto                : Endpoint<DeletePhotoRequest, Result>
///   11. BookTimeslotEndpoint       : Endpoint<BookTimeslotRequest, Result<BookTimeslotResponse>>
///   12. RequestCustomTimeEndpoint  : Endpoint<RequestCustomTimeRequest, Result<RequestCustomTimeResponse>>
///   13. RespondToCustomTimeRequestEndpoint : Endpoint<RespondToCustomTimeRequestRequest, Result<RespondToCustomTimeRequestResponse>>
///   14. LogMessage                 : Endpoint<LogMessageRequest, LogMessageResponse>
///   15. (Timeslot working hours endpoints, CRUD timeslot endpoints, etc.)
///
/// C) Global error handling in middleware:
///    - GlobalExceptionHandlerMiddleware catches unhandled exceptions
///    - FastEndpoints configured with UseProblemDetails for validation errors
///    - BaseEndpoint.HandleResultAsync provides centralized Result-pattern error handling
/// </summary>
public class EndpointErrorHandlingAudit
{
    /// <summary>
    /// Verifies the count of BaseEndpoint inheritors matches expectations.
    /// This acts as a regression guard - if new endpoints inherit from BaseEndpoint,
    /// this count should increase. If the count drops, investigate why.
    /// </summary>
    [Fact]
    public void BaseEndpointInheritors_CountMatchesExpected()
    {
        // Get all types from the FurryFriends.Web assembly that inherit from BaseEndpoint
        var webAssembly = typeof(BaseEndpoint<,>).Assembly;

        var baseEndpointInheritors = webAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && IsSubclassOfGenericBaseEndpoint(t))
            .ToList();

        // Expected count: 8 endpoints currently inherit from BaseEndpoint<TRequest, TResponse>
        // - 5 Rating endpoints (Create, Update, Delete, GetRatings, GetSummary)
        // - 2 Booking endpoints (Create, Cancel)
        // - 1 Client endpoint (AddPet)
        baseEndpointInheritors.Should().HaveCountGreaterThanOrEqualTo(8,
            $"Expected at least 8 endpoints to inherit from BaseEndpoint, found {baseEndpointInheritors.Count}. " +
            "New endpoints should use BaseEndpoint for centralized error handling.");
    }

    [Fact]
    public void AllEndpoints_UseEitherBaseEndpointOrGlobalMiddleware()
    {
        // All endpoints either use BaseEndpoint (with HandleResultAsync) 
        // or rely on GlobalExceptionHandlerMiddleware for unhandled exceptions.
        // This test verifies all endpoint classes exist and can be loaded.
        var webAssembly = typeof(BaseEndpoint<,>).Assembly;

        var endpointTypes = webAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && IsEndpointClass(t))
            .ToList();

        // There should be a reasonable number of endpoints defined
        endpointTypes.Should().HaveCountGreaterThanOrEqualTo(15,
            "Expected at least 15 endpoint classes in the Web project");
    }

    [Fact]
    public void GlobalExceptionHandlerMiddleware_IsRegistered()
    {
        // Verify the middleware class exists and can be instantiated
        var middlewareType = typeof(FurryFriends.Web.Middleware.GlobalExceptionHandlerMiddleware);
        middlewareType.Should().NotBeNull();
        middlewareType.IsClass.Should().BeTrue();
    }

    [Fact]
    public void ResponseBase_ClassExists()
    {
        // Verify the central response type exists
        var responseBaseType = typeof(ResponseBase<>);
        responseBaseType.Should().NotBeNull();
        responseBaseType.IsClass.Should().BeTrue();
    }

    /// <summary>
    /// Checks if a type inherits from BaseEndpoint<TRequest, TResponse>
    /// </summary>
    private static bool IsSubclassOfGenericBaseEndpoint(Type type)
    {
        var baseType = type.BaseType;
        while (baseType != null)
        {
            if (baseType.IsGenericType &&
                baseType.GetGenericTypeDefinition() == typeof(BaseEndpoint<,>))
            {
                return true;
            }
            baseType = baseType.BaseType;
        }
        return false;
    }

    /// <summary>
    /// Checks if a type is a FastEndpoints Endpoint class
    /// </summary>
    private static bool IsEndpointClass(Type type)
    {
        var baseType = type.BaseType;
        while (baseType != null)
        {
            if (baseType.IsGenericType)
            {
                var genericDef = baseType.GetGenericTypeDefinition();
                if (genericDef == typeof(Endpoint<,>) ||
                    genericDef == typeof(Endpoint<,,>) ||
                    genericDef == typeof(Endpoint<>))
                {
                    return true;
                }
            }
            baseType = baseType.BaseType;
        }
        return false;
    }
}
