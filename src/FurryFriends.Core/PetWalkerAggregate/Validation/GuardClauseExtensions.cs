using System.Text.RegularExpressions;

namespace FurryFriends.Core.PetWalkerAggregate.Validation;
public static class GuardClausesExtensions
{
  public static void InvalidEmail(this IGuardClause guardClause, string email, string parameterName)
  {
    if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
    {
      throw new ArgumentException("Invalid email address.", parameterName);
    }
  }

}
