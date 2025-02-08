namespace FurryFriends.Web.Middleware;

public class ValidateEmailMiddleware
{
  private readonly RequestDelegate _next;

  public ValidateEmailMiddleware(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    //Note: Do we need this?

    //var path = context.Request.Path.Value;

    //// Check if the request is for the /users/{email} endpoint
    //const string UserPath = "/PetWalker/email/";
    //if (path != null && path.StartsWith(UserPath) )
    //{
    //  var email = path.Substring(UserPath.Length);

    //  // Validate the email parameter
    //  if (string.IsNullOrEmpty(email) || !new EmailAddressAttribute().IsValid(email))
    //  {
    //    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    //    await context.Response.WriteAsync("Invalid email format.");
    //    return;
    //  }
    //}

    await _next(context);
  }
}
