using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class TestHttpContextMiddleware
{
  private readonly RequestDelegate _next;

  public TestHttpContextMiddleware(RequestDelegate next)
  {
    _next = next;
  }

  public async Task InvokeAsync(HttpContext httpContext, IHttpContextAccessor httpContextAccessor)
  {
    httpContextAccessor.HttpContext = httpContext;
    await _next(httpContext);
  }
}
