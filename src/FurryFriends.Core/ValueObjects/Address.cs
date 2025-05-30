﻿using FluentValidation.Results;
using FurryFriends.Core.Extensions;
using FurryFriends.Core.ValueObjects.Validators;

namespace FurryFriends.Core.ValueObjects;

public class Address : ValueObject
{
  public string Street { get; } = default!;
  public string City { get; } = default!;
  public string StateProvinceRegion { get; } = default!;
  public string ZipCode { get; } = default!;
  public string Country { get; private set; } = default!;

  public Address()
  {
  }

  private Address(string street, string city, string state, string country, string zipCode)
  {

    Street = street;
    City = city;
    StateProvinceRegion = state;
    Country = country;
    ZipCode = zipCode;
  }

  public static Result<Address> Create(string street, string city, string state, string country, string zipCode)
  {
    var address = new Address(street, city, state, country, zipCode);
    var validationResult = Validate(address);

    return validationResult.IsValid
      ? Result.Success(address)
      : validationResult.Errors.ToInvalidValidationErrorResult();

  }

  private static ValidationResult Validate(Address address)
  {
    var validator = new AddressValidator();
    var validationResult = validator.Validate(address);
    return validationResult;
  }

  public bool Equals(Address other)
  {
    if (ReferenceEquals(null, other)) return false;
    if (ReferenceEquals(this, other)) return true;

    return Street == other.Street && City == other.City && StateProvinceRegion == other.StateProvinceRegion &&
           ZipCode == other.ZipCode && Country == other.Country;
  }

  public override bool Equals(object? obj)
  {
    return obj is Address address && Equals(address);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(Street, City, StateProvinceRegion, ZipCode, Country);
  }

  protected override IEnumerable<object> GetEqualityComponents()
  {
    yield return Street;
    yield return City;
    yield return StateProvinceRegion;
    yield return ZipCode;
  }

  public override string ToString()
  {
    return $"{Street}, {City}, {StateProvinceRegion} {ZipCode}, {Country}";
  }
}

