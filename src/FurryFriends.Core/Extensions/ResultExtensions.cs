using Ardalis.Result;

namespace FurryFriends.Core.Extensions;

public static class ResultExtensions
{
    public static bool IsFailure<T>(this Result<T> result)
    {
        return !result.IsSuccess;
    }

    public static bool IsFailure(this Result result)
    {
        return !result.IsSuccess;
    }
}