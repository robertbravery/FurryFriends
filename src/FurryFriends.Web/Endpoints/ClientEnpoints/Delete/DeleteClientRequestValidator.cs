namespace FurryFriends.Web.Endpoints.ClientEnpoints.Delete;

public class DeleteClientRequestValidator : AbstractValidator<DeleteClientRequest>
{
  public DeleteClientRequestValidator()
  {
    RuleFor(x => x.ClientId)
        .NotEmpty()
        .WithMessage("Client ID is required");
  }
}
