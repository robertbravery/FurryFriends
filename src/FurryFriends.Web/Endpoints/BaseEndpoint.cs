namespace FurryFriends.Web.Endpoints;

// For endpoints without request/response types
public abstract class BaseEndpoint : EndpointWithoutRequest
{
  protected readonly IHttpContextAccessor _httpContextAccessor;

  protected BaseEndpoint(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }
}

// For endpoints with request/response types
public abstract class BaseEndpoint<TRequest, TResponse> : Endpoint<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
{
  protected readonly IHttpContextAccessor _httpContextAccessor;

  protected BaseEndpoint(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }
}
