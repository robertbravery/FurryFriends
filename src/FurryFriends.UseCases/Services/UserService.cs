using FurryFriends.Core.UserAggregate;

namespace FurryFriends.UseCases.Services;
public class UserService : IUserService
{
  private readonly IRepository<User> _repository;

  public UserService(IRepository<User> repository)
  {
    _repository = repository;
  }

  public async Task AddBioPictureAsync(Photo photo, Guid userId)
  {
    var user = await _repository.GetByIdAsync(userId) 
      ?? throw new InvalidOperationException("User not found.");

    // Update the user's bio picture
    user.AddPhoto(photo); 

    // Save changes to the repository
    await _repository.UpdateAsync(user);

    // Optionally, you can fire a domain event or log the update
  }

  public Task<User> CreateUserAsync(User user)
  {
    throw new NotImplementedException();
  }
}
