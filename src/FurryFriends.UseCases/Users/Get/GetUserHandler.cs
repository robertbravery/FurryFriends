using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.SharedKernel;
using FurryFriends.Core.ContributorAggregate.Specifications;
using FurryFriends.Core.Entities;
using FurryFriends.Core.UserAggregate.Specifications;

namespace FurryFriends.UseCases.Users.Get;
public class GetUserHandler(IReadRepository<User> _repository) : IQueryHandler<GetUserQuery, Result<UserDto>>
{
  public async Task<Result<UserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
  {
    var spec = new GetUserByEmailSpecification(request.Email);
    var entity = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (entity == null) return Result.NotFound("User Not Found");

    return new UserDto(entity.Id, entity.Email, entity.Name, entity.PhoneNumber.ToString(), entity.Address.City);
  }
}
