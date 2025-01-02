namespace FurryFriends.Web.Endpoints.Base;

// For endpoints without request/response types
public abstract class BaseEndpoint : EndpointWithoutRequest
{
  protected readonly IHttpContextAccessor _httpContextAccessor = default!;

  protected BaseEndpoint()
  {
  }
}

// For endpoints with request/response types
public abstract class BaseEndpoint<TRequest, TResponse> : Endpoint<TRequest, TResponse>
    where TRequest : class
    where TResponse : class
{
  protected readonly IHttpContextAccessor _httpContextAccessor = default!;

  protected BaseEndpoint()
  {
  }
}
