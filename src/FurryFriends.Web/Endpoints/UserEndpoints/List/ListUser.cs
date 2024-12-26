using FurryFriends.Core.Entities;
using FurryFriends.UseCases.Users.List;

namespace FurryFriends.Web.Endpoints.UserEndpoints.List;

public class ListUser : Endpoint<ListUsersRequest, ListUsersResponse>
  {
      private readonly IMediator _mediator;

      public ListUser(IMediator mediator)
      {
          _mediator = mediator;
      }

    public override void Configure()
    {
        Get(ListUsersRequest.Route); // Specify the route
        AllowAnonymous(); // Adjust as needed
        Summary(s =>
        {
            s.Summary = "List Users";
            s.Description = "Retrieves a list of all users";
        }
        
        );
    }

    public override async Task HandleAsync(ListUsersRequest request, CancellationToken cancellationToken)
    {
        var query = new ListUsersQuery(request.SearchTerm, request.Page, request.PageSize);
        var users = await _mediator.Send(query, cancellationToken);
    
      var x = users.Value.Users.ToList().ConvertAll(c=> new UserListResponseDto(c.Id,c.Name, c.Email, c.Address.City));
    var totalCount = users.Value.TotalCount;
    
      var response = new ListUsersResponse(x, request.Page, request.PageSize, totalCount);



      await SendAsync(response, 200, cancellationToken); // Status code 200 OK
    }
}
