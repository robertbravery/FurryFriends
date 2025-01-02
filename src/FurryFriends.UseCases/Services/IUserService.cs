using FurryFriends.Core.UserAggregate;

namespace FurryFriends.UseCases.Services;
public interface IUserService
{
  Task<User> CreateUserAsync(User user);
  Task AddBioPictureAsync(Photo photo, Guid userId);
  Task<Result<User>> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
}
