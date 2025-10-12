using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService(IConfiguration config) : ITokenService
{
  public string CreateToken(AppUser user)
  {
    var tokenKey = config["TokenKey"];
    if (string.IsNullOrEmpty(tokenKey))
    {
      throw new Exception("TokenKey is missing");
    }
    if (tokenKey.Length < 64)
    {
      throw new Exception("TokenKey must be at least 64 characters long");
    }
    var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(tokenKey));

    var claims = new List<Claim>
    {
      new Claim(ClaimTypes.Email, user.UserName),
      new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
    };

    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

    var tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(claims),
      Expires = DateTime.Now.AddDays(7),
      SigningCredentials = creds
    };

    var tokenHandler = new JwtSecurityTokenHandler();
    var token = tokenHandler.CreateToken(tokenDescriptor);

    return tokenHandler.WriteToken(token);
  }
}
