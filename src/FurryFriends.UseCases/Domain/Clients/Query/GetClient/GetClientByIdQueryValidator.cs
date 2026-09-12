using FluentValidation;

namespace FurryFriends.UseCases.Domain.Clients.Query.GetClient;

internal class GetClientByIdQueryValidator : AbstractValidator<GetClientByIdQuery>
{
  public GetClientByIdQueryValidator()
  {
    RuleFor(x => x.Id).NotEmpty();
  }
}
