namespace FurryFriends.Core.Interfaces;

public interface IEmailSender
{
  Task SendConfirmationEmail(string email);
  Task SendEmailAsync(string to, string from, string subject, string body);
}
