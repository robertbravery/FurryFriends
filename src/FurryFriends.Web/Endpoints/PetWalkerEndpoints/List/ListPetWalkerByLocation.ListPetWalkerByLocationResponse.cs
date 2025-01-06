namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.List;

public class ListPetWalkerByLocationResponse(
  List<PetWalkerListResponseDto> rowsData,
  int pageNumber,
  int pageSize,
  int totalCount, string[] hideColumns)
//, bool success = true, string message = "Success", List<string>? errors = null) 
//: ResponseBase<List<UserListResponseDto>>(rowsData, success, message, errors)
{
  public List<PetWalkerListResponseDto> RowsData { get; set; } = rowsData;
  public int PageNumber { get; } = pageNumber;
  public int PageSize { get; } = pageSize;
  public int TotalCount { get; } = totalCount;

  public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
  public bool HasPreviousPage => PageNumber > 1;
  public bool HasNextPage => PageNumber < TotalPages;
  public string[] HideColumns { get; set; } = hideColumns;

}
