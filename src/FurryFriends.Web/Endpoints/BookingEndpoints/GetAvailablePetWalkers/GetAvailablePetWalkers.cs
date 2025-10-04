using FurryFriends.UseCases.Domain.PetWalkers.Query.ListPetWalker;

namespace FurryFriends.Web.Endpoints.BookingEndpoints.GetAvailablePetWalkers;

/// <summary>
/// Endpoint for getting available PetWalkers for booking selection with pagination, sorting, and filtering
/// </summary>
public class GetAvailablePetWalkers(IMediator mediator) : Endpoint<GetAvailablePetWalkersRequest, GetAvailablePetWalkersResponse>
{
    private readonly IMediator _mediator = mediator;

    public override void Configure()
    {
        Get(GetAvailablePetWalkersRequest.Route);
        AllowAnonymous(); // Or apply appropriate authorization
        Options(x => x.WithName("GetAvailablePetWalkers_" + Guid.NewGuid().ToString()));
        Description(d => d
            .Produces<GetAvailablePetWalkersResponse>(200)
            .Produces(400)
            .Produces(404));
    }

    public override async Task HandleAsync(GetAvailablePetWalkersRequest request, CancellationToken ct)
    {
        // Use the existing ListPetWalkerByLocationQuery but get more results to allow for filtering
        Guid? localityId = null;

        // If service area is provided, we would need to resolve it to a locality ID
        // For now, we'll use the search term approach
        var searchTerm = !string.IsNullOrEmpty(request.SearchTerm) ? request.SearchTerm : null;

        // Get a larger set of results to allow for proper filtering and pagination
        var query = new ListPetWalkerByLocationQuery(searchTerm, localityId, 1, 1000); // Get up to 1000 results
        var result = await _mediator.Send(query, ct);

        if (!result.IsSuccess || result.Value.Users == null)
        {
            var emptyResponse = new GetAvailablePetWalkersResponse
            {
                PetWalkers = new List<PetWalkerSummaryResponse>(),
                PageNumber = request.Page,
                PageSize = request.PageSize,
                TotalCount = 0,
                TotalPages = 0,
                HasPreviousPage = false,
                HasNextPage = false,
                AvailableServiceAreas = new List<string>()
            };
            await SendOkAsync(emptyResponse, ct);
            return;
        }

        var allPetWalkers = result.Value.Users
            .Where(pw => pw.IsActive && pw.IsVerified)
            .Select(pw => new PetWalkerSummaryResponse
            {
                Id = pw.Id,
                FullName = pw.Name.FullName,
                Email = pw.Email.EmailAddress,
                Biography = pw.Biography,
                HourlyRate = pw.Compensation.HourlyRate,
                Currency = pw.Compensation.Currency,
                IsActive = pw.IsActive,
                IsVerified = pw.IsVerified,
                YearsOfExperience = pw.YearsOfExperience,
                HasInsurance = pw.HasInsurance,
                HasFirstAidCertification = pw.HasFirstAidCertificatation,
                DailyPetWalkLimit = pw.DailyPetWalkLimit,
                BioPictureUrl = pw.Photos.FirstOrDefault(p => p.PhotoType == Core.PetWalkerAggregate.Enums.PhotoType.BioPic)?.Url,
                Rating = 0.0, // This would need to be calculated from reviews
                ReviewCount = 0, // This would need to be calculated from reviews
                ServiceAreas = pw.ServiceAreas.Select(sa => sa.Locality.LocalityName).ToList()
            });

        // Apply service area filtering if provided
        if (!string.IsNullOrEmpty(request.ServiceArea))
        {
            allPetWalkers = allPetWalkers
                .Where(pw => pw.ServiceAreas.Any(sa =>
                    sa.Contains(request.ServiceArea, StringComparison.OrdinalIgnoreCase)));
        }

        // Apply sorting
        allPetWalkers = ApplySorting(allPetWalkers, request.SortBy, request.SortDirection);

        // Get total count before pagination
        var totalCount = allPetWalkers.Count();
        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        // Apply pagination after filtering and sorting
        var petWalkersList = allPetWalkers
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        // Get all unique service areas for filtering dropdown
        var availableServiceAreas = result.Value.Users
            .SelectMany(pw => pw.ServiceAreas.Select(sa => sa.Locality.LocalityName))
            .Distinct()
            .OrderBy(sa => sa)
            .ToList();

        var response = new GetAvailablePetWalkersResponse
        {
            PetWalkers = petWalkersList,
            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasPreviousPage = request.Page > 1,
            HasNextPage = request.Page < totalPages,
            AvailableServiceAreas = availableServiceAreas
        };

        await SendOkAsync(response, ct);
    }

    private static IEnumerable<PetWalkerSummaryResponse> ApplySorting(
        IEnumerable<PetWalkerSummaryResponse> petWalkers,
        string sortBy,
        string sortDirection)
    {
        var isDescending = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);

        return sortBy.ToLower() switch
        {
            "name" => isDescending
                ? petWalkers.OrderByDescending(pw => pw.FullName)
                : petWalkers.OrderBy(pw => pw.FullName),
            "rate" => isDescending
                ? petWalkers.OrderByDescending(pw => pw.HourlyRate)
                : petWalkers.OrderBy(pw => pw.HourlyRate),
            "experience" => isDescending
                ? petWalkers.OrderByDescending(pw => pw.YearsOfExperience)
                : petWalkers.OrderBy(pw => pw.YearsOfExperience),
            "rating" => isDescending
                ? petWalkers.OrderByDescending(pw => pw.Rating)
                : petWalkers.OrderBy(pw => pw.Rating),
            _ => petWalkers.OrderBy(pw => pw.FullName) // Default to name ascending
        };
    }
}
