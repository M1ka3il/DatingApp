using System;
using API.DTO;
using API.Entities;
using API.Interfaces;

namespace API.Extensions;

public static class AppUserExtensions
{
  public static UserDTO ToUserDTO(this AppUser user, ITokenService tokenService)
  {
    return new UserDTO
    {
      id = user.Id,
      UserName = user.UserName,
      Email = user.Email,
      Token = tokenService.CreateToken(user)
    };
  }
}
