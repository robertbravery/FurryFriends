namespace FurryFriends.Web.Endpoints.Base;

public class ListRequestBase
{
  public string? SearchTerm { get; set; }
  public int Page { get; set; } = 1;
  public int PageSize { get; set; } = 10;
}

