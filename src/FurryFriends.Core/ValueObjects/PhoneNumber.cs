﻿using FluentValidation;
namespace FurryFriends.Core.ValueObjects;

public class PhoneNumber : ValueObject
{
  public string CountryCode { get; private set; } = default!;
  public string Number { get; private set; } = default!;
  public PhoneNumber()
  {
    
  }

  private PhoneNumber(string countryCode,  string number)
  {
    CountryCode = countryCode;
    Number = number;
  }

  public static async Task<Result<PhoneNumber>> Create(string countryCode,  string number, IValidator<PhoneNumber> validator)
  {

    var phoneNumber = new PhoneNumber(countryCode, number);
    return await Validate(validator, phoneNumber);
  }

  private static async Task<Result<PhoneNumber>> Validate(IValidator<PhoneNumber> validator, PhoneNumber phoneNumber)
  {
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
    yield return Number;
  }

  public override string ToString()
  {
    return $"{CountryCode} {Number}";
  }
  public bool Equals(PhoneNumber other)
  {
    if (ReferenceEquals(null, other)) return false;
    if (ReferenceEquals(this, other)) return true;

    return CountryCode == other.CountryCode && Number == other.Number;
  }

  public override bool Equals(object? obj)
  {
    return obj is PhoneNumber phoneNumber && Equals(phoneNumber);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(CountryCode, Number);
  }
}

