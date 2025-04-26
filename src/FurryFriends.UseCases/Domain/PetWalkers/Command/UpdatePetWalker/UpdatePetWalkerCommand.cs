using MediatR;

namespace FurryFriends.UseCases.Domain.PetWalkers.Command.UpdatePetWalker;

public class UpdatePetWalkerCommand : IRequest
{
  public Guid PetWalkerId { get; set; }
  public required string FirstName { get; set; }
  public required string LastName { get; set; }
  public required string CountryCode { get; set; }
  public required string PhoneNumber { get; set; }
  public required string Street { get; set; }
  public required string City { get; set; }
  public required string State { get; set; }
  public required string ZipCode { get; set; }
  public required string Country { get; set; }
  public required string Biography { get; set; }
  public DateTime DateOfBirth { get; set; }
  public int Gender { get; set; }
  public decimal HourlyRate { get; set; }
  public required string Currency { get; set; }
  public bool IsActive { get; set; }
  public bool IsVerified { get; set; }
  public int YearsOfExperience { get; set; }
  public bool HasInsurance { get; set; }
  public bool HasFirstAidCertification { get; set; }
  public int DailyPetWalkLimit { get; set; }
  public required string ServiceLocation { get; set; }
  //public required string BioPicture { get; set; }
  //public required string Photos { get; set; }
}
