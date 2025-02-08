namespace FurryFriends.Web.Endpoints.Base;

public class ResponseBase<T>(T? data, bool success = true, string message = "Success", ICollection<string>? errors = null, string? errorCode = null) where T : class
{
  public bool Success { get; set; } = success;
  public string Message { get; set; } = message;
  public T? Data { get; set; } = data;
  public ICollection<string>? Errors { get; set; } = errors;
  public string? ErrorCode { get; set; } = errorCode;
  public DateTime Timestamp { get; set; } = DateTime.Now;

  public static ResponseBase<T> NotFound(string message, ICollection<string> errors)
  {
    return new ResponseBase<T>
    (
      data: default,
      success: false,
      message: message,
      errors: errors);
  }

  public static ResponseBase<T> FromException(Exception ex)
  {
    return new ResponseBase<T>
    (
        data: default,
        success: false,
        message: "An error occurred",
        errors: new List<string> { ex.Message },
        errorCode: "InternalServerError" // Set a generic error code
    );
  }

}
