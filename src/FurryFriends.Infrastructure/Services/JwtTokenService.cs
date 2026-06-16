using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using FurryFriends.Core;

namespace FurryFriends.Infrastructure.Services;

public class JwtTokenService
{{
    private readonly string _jwtKey; 
    private readonly int _expirationHours; 

    public JwtTokenService(IOptions<JwtSettings> settings)
    {{
        _jwtKey = settings.Value.SecretKey;
        _expirationHours = settings.Value.ExpirationHours;
    }}

    public string GenerateToken(string email, string role)
    {{
        var tokenHandler = new JwtSecurityTokenHandler(); 
        var tokenDescriptor = new SecurityTokenDescriptor
        {{
            Subject = new ClaimsIdentity(new[] {
                new Claim("email", email),
                new Claim("role", role)
            }),
            Expires = DateTime.UtcNow.AddHours(_expirationHours),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey)), SecurityAlgorithms.HmacSha256Signature)
        }};

        var token = tokenHandler.CreateToken(tokenDescriptor); 
        return tokenHandler.WriteToken(token);
    }}

    public bool ValidateToken(string token, out string email, out string role)
    {{
        email = null;
        role = null;

        try
        {{
            var tokenHandler = new JwtSecurityTokenHandler(); 
            var principal = tokenHandler.ReadJwtToken(token); 

            email = principal.Claims.First(c => c.Type == "email").Value;
            role = principal.Claims.First(c => c.Type == "role").Value;

            return true;
        }}
        catch
        {{
            return false;
        }}
    }}
}}

    public string GenerateToken(string email, string role)
    {{
        var tokenHandler = new JwtSecurityTokenHandler(); 
        var tokenDescriptor = new SecurityTokenDescriptor
        {{
            Subject = new ClaimsIdentity(new[] {
                new Claim("email", email),
                new Claim("role", role)
            }),
            Expires = DateTime.UtcNow.AddHours(_expirationHours),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_jwtKey)), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor); 
        return tokenHandler.WriteToken(token);
    }}

    public bool ValidateToken(string token, out string email, out string role)
    {{
        email = null;
        role = null;

        try
        {{
            var tokenHandler = new JwtSecurityTokenHandler(); 
            var principal = tokenHandler.ReadJwtToken(token); 

            email = principal.Claims.First(c => c.Type == "email").Value;
            role = principal.Claims.First(c => c.Type == "role").Value;

            return true;
        }}
        catch
        {{
            return false;
        }}
    }}
}}