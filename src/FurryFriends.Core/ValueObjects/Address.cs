using Ardalis.GuardClauses;
using Ardalis.SharedKernel;

namespace FurryFriends.Core.ValueObjects;

public class Address : ValueObject
{
  public string Street { get; } = default!;
  public string City { get; } = default!;
  public string StateProvinceRegion { get; } = default!;
  public string ZipCode { get; } = default!;


  public Address()
  {

  }

  public Address(string street, string city, string state, string zipCode)
  {
    Guard.Against.NullOrEmpty(street, nameof(street));
    Guard.Against.NullOrEmpty(city, nameof(city));
    Guard.Against.NullOrEmpty(state, nameof(state));
    Guard.Against.NullOrEmpty(zipCode, nameof(zipCode));

    Street = street;
    City = city;
    StateProvinceRegion = state;
    ZipCode = zipCode;
  }

  public static Address Create(string street, string city, string state, string zipCode) => new(street, city, state, zipCode);

  protected override IEnumerable<object> GetEqualityComponents()
  {
    yield return Street;
    yield return City;
    yield return StateProvinceRegion;
    yield return ZipCode;
  }
}

