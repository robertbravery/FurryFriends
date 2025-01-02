using System.Text.RegularExpressions;

namespace FurryFriends.Core.ValueObjects;

public class Email: ValueObject
{
  private static readonly Regex EmailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
  public string EmailAddress { get; private set; }

  private Email(string emailAddress)
  { 
    EmailAddress = emailAddress;
  }

  public static Result<Email> Create(string emailAddress)
  {
    if (!IsValidEmail(emailAddress))
      return Result.Error("Invalid email address.");

    var email  = new Email(emailAddress);
    return Result.Success(email);
  }

  private static bool IsValidEmail(string email)
  {
    try
    { 
      _ = new System.Net.Mail.MailAddress(email).Address == email;
    }
    catch
    {
      return false;
    }
    return true;
  }

  // Override equality operators
  public override bool Equals(object? obj)
  {
    if (obj is Email other)
    {
      return EmailAddress == other.EmailAddress;
    }
    return false;
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(EmailAddress);
  }

  protected override IEnumerable<object> GetEqualityComponents()
  {
    yield return EmailAddress;

  }

  public override string ToString()
  {
    return EmailAddress;
  }
}

