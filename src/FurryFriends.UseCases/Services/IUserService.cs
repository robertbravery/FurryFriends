using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FurryFriends.Core.UserAggregate;

namespace FurryFriends.UseCases.Services;
public interface IUserService
{
  Task<User> CreateUserAsync(User user);
  Task AddBioPictureAsync(Photo photo, Guid userId);
}
