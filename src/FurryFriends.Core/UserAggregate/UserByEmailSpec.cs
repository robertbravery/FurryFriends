using FurryFriends.Core.UserAggregate;

public class UserByEmailSpecification : Specification<User>
{
    public  bool IsSatisfiedBy(User user, string searchValue)
    {
        return user.Email.Contains(searchValue);
    }
}