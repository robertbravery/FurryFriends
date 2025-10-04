
namespace FurryFriends.Web.Endpoints.BookingEndpoints.GetAvailablePetWalkers;

public class GetAvailablePetWalkersRequest
{
  public const string Route = "/petwalkers/available";

  // Filtering
  public string ServiceArea { get; set; } = string.Empty;
  public string SearchTerm { get; set; } = string.Empty;

  // Pagination
  public int Page { get; set; } = 1;
  public int PageSize { get; set; } = 15;

  // Sorting
  public string SortBy { get; set; } = "name"; // name, rate, experience, rating
  public string SortDirection { get; set; } = "asc"; // asc, desc
}
