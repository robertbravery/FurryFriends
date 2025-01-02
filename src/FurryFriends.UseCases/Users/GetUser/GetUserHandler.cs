using FurryFriends.Core.UserAggregate;
using FurryFriends.Core.UserAggregate.Specifications;

namespace FurryFriends.UseCases.Users.GetUser;
public class GetUserHandler(IReadRepository<User> _repository) : IQueryHandler<GetUserQuery, Result<UserDto>>
{
  public async Task<Result<UserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
  {
    var spec = new GetUserByEmailSpecification(request.Email);
    var entity = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (entity == null) return Result.NotFound("User Not Found");

    return new UserDto(entity.Id, entity.Email, entity.Name.FullName, entity.PhoneNumber.ToString(), entity.Address.City);
  }
}
