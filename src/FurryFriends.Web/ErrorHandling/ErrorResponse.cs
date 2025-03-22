namespace FurryFriends.Web.ErrorHandling;

public class ErrorResponse
{
  public string? Type { get; set; }
  public string? Title { get; set; }
  public int Status { get; set; }
  public string? Detail { get; set; }
  public string? Instance { get; set; }
  public Dictionary<string, string[]>? Errors { get; set; }  // Validation errors
  public ErrorResponse() { }

  public ErrorResponse(int statusCode, string title)
  {
    Status = statusCode;
    Title = title;
  }
}
