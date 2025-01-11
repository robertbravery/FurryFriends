namespace FurryFriends.Web.Endpoints.Base;

public class ResponseBase<T>(T? data, bool success = true, string message = "Success", List<string>? errors = null) where T : class
{
  public bool Success { get; set; } = success;
  public string Message { get; set; } = message;
  public T? Data { get; set; } = data;
  public List<string>? Errors { get; set; } = errors;
  public DateTime Timestamp { get; set; } = DateTime.Now;

  public static ResponseBase<T> NotFound(string email)
  {
    return new ResponseBase<T>
    (
      data: default,
      success: false,
      message: $"User with email {email} was not found",
      errors: [$"User with email {email} was not found"]);
  }
}
