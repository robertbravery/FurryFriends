using FastEndpoints;
using FurryFriends.UseCases.Users.List;
namespace FurryFriends.Web.Endpoints.UserEndpoints.List;


public class ListUsersResponse(List<UserListResponseDto> userListResponseDtos, int pageNumber, int pageSize, int totalCount)
{
  public List<UserListResponseDto> RowsData { get; set; } =  userListResponseDtos;
  public int PageNumber { get; } = pageNumber;
  public int PageSize { get; } = pageSize;
  public int TotalCount { get; } = totalCount;

  public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
  public bool HasPreviousPage => PageNumber > 1;
  public bool HasNextPage => PageNumber < TotalPages;
  public string[] HideColumns { get; set; } = new[] { "Id" };

}
