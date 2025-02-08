using FluentValidation;

namespace FurryFriends.UseCases.Clients.GetClient;
internal class GetClientQueryValidator : AbstractValidator<GetClientQuery>
{
  public GetClientQueryValidator()
  {
    RuleFor(x => x.EmailAddress).NotEmpty().EmailAddress();
  }
}
