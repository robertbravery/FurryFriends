using System;

namespace FurryFriends.Infrastructure.Services;

public class JwtSettings
{
    public string SecretKey { get; set; }
    public int ExpirationHours { get; set; }
}