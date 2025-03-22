using FluentValidation;

namespace FurryFriends.UseCases.Domain.Clients.Query.GetClient;
internal class GetClientQueryValidator : AbstractValidator<GetClientQuery>
{
  public GetClientQueryValidator()
  {
    RuleFor(x => x.EmailAddress).NotEmpty().EmailAddress();
  }
}
