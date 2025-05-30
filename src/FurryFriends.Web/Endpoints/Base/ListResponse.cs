﻿namespace FurryFriends.Web.Endpoints.Base;

public class ListResponse<T>(
  List<T> rowsData,
  int pageNumber,
  int pageSize,
  int totalCount, string[] hideColumns) where T : class
{
  public List<T> RowsData { get; set; } = rowsData;
  public int PageNumber { get; } = pageNumber;
  public int PageSize { get; } = pageSize;
  public int TotalCount { get; } = totalCount;

  public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
  public bool HasPreviousPage => PageNumber > 1;
  public bool HasNextPage => PageNumber < TotalPages;
  public string[] HideColumns { get; set; } = hideColumns;

}
