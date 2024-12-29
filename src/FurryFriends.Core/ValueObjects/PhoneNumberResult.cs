namespace FurryFriends.Core.ValueObjects;

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
