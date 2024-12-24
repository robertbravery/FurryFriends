using FluentValidation;
using FluentValidation.Results;
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

      public static Result<PhoneNumber> Create(string countryCode, string areaCode,string number, IValidator<PhoneNumber> validator)
      {
          
          var phoneNumber = new PhoneNumber(countryCode, areaCode, number);

          var validationResult = validator.Validate(phoneNumber);
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
  }

  public class PhoneNumberValidator : AbstractValidator<PhoneNumber>
  {
      // Define validation rules here...
  }

  public class PhoneNumberResult
  {
      public bool IsValid { get; private set; }
      public PhoneNumber? PhoneNumber { get; private set; }
      public string[] Errors { get; private set; }

      public PhoneNumberResult(bool isValid, PhoneNumber? phoneNumber, string[] errors)
      {
          IsValid = isValid;
          PhoneNumber = phoneNumber;
          Errors = errors;
      }
  }
