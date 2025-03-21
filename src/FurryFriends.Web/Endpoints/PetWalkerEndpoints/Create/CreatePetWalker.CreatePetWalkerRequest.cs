﻿using FurryFriends.Core.ValueObjects;

namespace FurryFriends.Web.Endpoints.PetWalkerEndpoints.Create;

public class CreatePetWalkerRequest
{
  public const string Route = "/PetWalker";

  public string FirstName { get; set; } = default!;

  public string LastName { get; set; } = default!;

  public string Email { get; set; } = default!;

  public string PhoneCountryCode { get; set; } = default!;

  public string PhoneNumber { get; set; } = default!;

  public string Street { get; set; } = default!;

  public string City { get; set; } = default!;

  public string State { get; set; } = default!;

  public string Country { get; set; } = default!;

  public string PostalCode { get; set; } = default!;

  public GenderType.GenderCategory Gender { get; set; } = GenderType.GenderCategory.Other;

  public string? Biography { get; set; }

  public DateTime DateOfBirth { get; set; }

  public decimal HourlyRate { get; set; }

  public string Currency { get; set; } = default!;

  public bool IsActive { get; set; }

  public bool IsVerified { get; set; }

  public int YearsOfExperience { get; set; }

  public bool HasInsurance { get; set; }

  public bool HasFirstAidCertification { get; set; }

  public int DailyPetWalkLimit { get; set; }
}
