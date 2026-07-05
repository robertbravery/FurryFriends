namespace FurryFriends.UseCases.Userauthentication.Login;

public class LoginResponse
{
  public string Token { get; set; } = null!;
  public string Role { get; set; } = null!;
    public Guid? UserId { get; set; }
    public bool IsValid { get; set; }

    public LoginResponse() { }

    public LoginResponse(string token, string role, Guid? userId)
    {
        Token = token;
        Role = role;
        UserId = userId;
        IsValid = true;
    }
}
