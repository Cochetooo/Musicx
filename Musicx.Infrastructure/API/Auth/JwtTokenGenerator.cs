using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Musicx.Application.Api.Interfaces.Auth;
using Musicx.Contracts.Dto.Responses;

namespace Musicx.Infrastructure.API.Auth;

public sealed class JwtTokenGenerator(IConfiguration config,
    ILoggerProvider loggerProvider) : ITokenGenerator
{
    private readonly ILogger _logger = loggerProvider.CreateLogger(nameof(JwtTokenGenerator));
    
    public string Generate(OutUser user)
    {
        _logger.LogInformation($"⛏️🔁 Generating JWT Token for user {user.Email}");
        
        var key = Encoding.UTF8.GetBytes(config["Jwt:Secret"] ??
                                         throw new Exception("Jwt:Secret missing in configuration file."));
        var creds = new SigningCredentials(new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
        };

        if (user.Roles is not null)
        {
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));

                if (role.Permissions is not null)
                {
                    foreach (var permission in role.Permissions)
                    {
                        claims.Add(new Claim("permission", permission.Name));
                    }
                }
            }
        }
        
        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(6),
            signingCredentials: creds);
        
        _logger.LogInformation($"⛏️✅ JWT Token generated successfully for user {user.Email} !");
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}