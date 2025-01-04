using FurryFriends.Core.UserAggregate;
using FurryFriends.UseCases.Services.DataTransferObjects;
using FurryFriends.UseCases.Users.ListUser;

namespace FurryFriends.UseCases.Services;
public interface IUserService
{
  Task<User> CreateUserAsync(User user);
  Task AddBioPictureAsync(Photo photo, Guid userId);
  Task<Result<User>> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
  Task<Result<UserListDto>> ListUsersAsync(ListUsersQuery query);
  Task<Result<UserListDto>> ListUserUserByLocationAsync(ListUsersByLocationQuery query);
  Task<Result> UpdateUserHourlyRateAsync(Guid userId, decimal hourlyRate, string currency, CancellationToken cancellationToken);
}
