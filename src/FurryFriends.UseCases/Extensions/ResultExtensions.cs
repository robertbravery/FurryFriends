namespace FurryFriends.UseCases.Extensions;
public static class ResultExtensions
{
  public static Result<T> Combine<T>(this IEnumerable<Result<T>> results)
  {
    if (results.Any(r => !r.IsSuccess))
    {
      return Result.Error((ErrorList)results.Where(r => !r.IsSuccess).SelectMany(r => r.Errors));
    }

    return Result.Success();
  }

}
