﻿using FurryFriends.Core.PetWalkerAggregate.Specifications;
using FurryFriends.Core.UserAggregate;
using FurryFriends.Core.ValueObjects;
using FurryFriends.UseCases.Services.DataTransferObjects;
using FurryFriends.UseCases.Users.ListUser;

namespace FurryFriends.UseCases.Services;
public class UserService : IUserService
{
  private readonly IRepository<PetWalker> _repository;

  public UserService(IRepository<PetWalker> repository)
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

  public Task<PetWalker> CreateUserAsync(PetWalker user)
  {
    throw new NotImplementedException();
  }

  public async Task<Result<PetWalker>> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
  {
    var spec = new GetPetWalkerByEmailSpecification(email);
    var entity = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
    if (entity == null)
      return Result.NotFound("User Not Found");

    return entity;
  }

  public async Task<Result<UserListDto>> ListUsersAsync(ListUsersQuery query)
  {
    var spec = new ListPetWalkerSpecification(query.SearchString, query.PageNumber, query.PageSize);
    var users = await _repository.ListAsync(spec);
    var totalCount = await _repository.CountAsync(spec);
    return new UserListDto(users, totalCount);
  }

  public async Task<Result<UserListDto>> ListUserUserByLocationAsync(ListUsersByLocationQuery query)
  {
    var spec = new ListPetWalkerByLocationSpecification(query.SearchString, query.Location, query.PageNumber, query.PageSize);
    var users = await _repository.ListAsync(spec);
    var totalCount = await _repository.CountAsync(spec);
    return new UserListDto(users, totalCount);
  }

  public async Task<Result> UpdateUserHourlyRateAsync(Guid userId, decimal hourlyRate, string currency, CancellationToken cancellationToken)
  {
    var user = await _repository.GetByIdAsync(userId, cancellationToken);
    if (user == null)
    {
      return Result.Error("User not found.");
    }
    var compensation = Compensation.Create(hourlyRate, currency);
    user.UpdateCompensation(compensation);
    await _repository.UpdateAsync(user, cancellationToken);

    return Result.Success();
  }
}
