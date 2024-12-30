using FluentValidation;
using FurryFriends.Core.ValueObjects.Validators;
namespace FurryFriends.Core.ValueObjects;

public class PhoneNumber : ValueObject
{
  public string CountryCode { get; private set; } = default!;
  public string AreaCode { get; private set; } = default!;
  public string Number { get; private set; } = default!;
  public PhoneNumber()
  {

  }
  private PhoneNumber(string countryCode, string areaCode, string number)
  {
    CountryCode = countryCode;
    AreaCode = areaCode;
    Number = number;
  }

  public static async Task<Result<PhoneNumber>> Create(string countryCode, string areaCode, string number, IValidator<PhoneNumber> validator)
  {

    var phoneNumber = new PhoneNumber(countryCode, areaCode, number);

    var validationResult = await validator.ValidateAsync(phoneNumber);
    if (validationResult.IsValid)
    {
      return Result.Success(phoneNumber);
    }
    else
    {
      return Result.Error(validationResult.ToString());
    }
  }

  protected override IEnumerable<object> GetEqualityComponents()
  {
    yield return CountryCode;
    yield return AreaCode;
    yield return Number;
  }

  public override string ToString()
  {
    return $"{CountryCode} {AreaCode} {Number}";
  }
}
