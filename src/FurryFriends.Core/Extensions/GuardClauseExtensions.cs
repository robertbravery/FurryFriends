namespace FurryFriends.Core.Extensions;
public static class GuardClauseExtensions
{
  public static void FutureDate(this IGuardClause guardClause, DateTime date, string parameterName)
  {
    if (date > DateTime.Now)
    {
      throw new ArgumentException("Date cannot be in the future.", parameterName);
    }
  }
}
