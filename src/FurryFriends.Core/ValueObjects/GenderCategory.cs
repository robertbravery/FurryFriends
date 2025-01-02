namespace FurryFriends.Core.ValueObjects;

public class GenderType: ValueObject
{
  public GenderCategory Gender { get; private set; }

  public enum GenderCategory
  {
    Male=1,
    Female=2,
    Other=3
  }

  private GenderType(GenderCategory gender)
  {
   
    Gender = gender;
  }

  public static Result<GenderType> Create(GenderCategory gender)
  {
    if (!IsValidGender(gender))
      return Result.Error("Invalid gender.");

    return Result.Success(new GenderType(gender));
  }

  public static bool IsValidGender(GenderCategory gender)
  {
    return Enum.IsDefined(gender);
  }

  protected override IEnumerable<object> GetEqualityComponents()
  {
    yield return Gender;
  }
}
