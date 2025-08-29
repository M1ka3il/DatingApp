using System;
using System.Security.Cryptography;
using API.Data;
using API.DTO;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(AppDbContext context) : BaseAPIController
{
  [HttpPost("register")] //api/account/register
  public async Task<ActionResult<AppUser>> Register(RegisterDTO registerDTO)
  {
    if (await UserExists(registerDTO.Username)) return BadRequest("Username is taken");
    if (await EmailExists(registerDTO.Email)) return BadRequest("Email is taken");
    using var hmac = new HMACSHA512();
    var user = new AppUser
    {
      UserName = registerDTO.Username.ToLower(),
      Email = registerDTO.Email.ToLower(),
      PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(registerDTO.Password)),
      PasswordSalt = hmac.Key
    };
    return user;
  }
  #region snippet_AccountController_Exists
  private async Task<bool> UserExists(string username)
  {
    return await context.Users.AnyAsync(x => x.UserName == username.ToLower());
  }
  private async Task<bool> EmailExists(string email)
  {
    return await context.Users.AnyAsync(x => x.Email == email.ToLower());
  }

  #endregion snippet_AccountController_Exists

  [HttpPost("login")]
  public async Task<ActionResult<AppUser>> Login(LoginDTO loginDTO)
  {
    var user = await context.Users.SingleOrDefaultAsync(x => x.Email == loginDTO.Email.ToLower());
    if (user == null) return Unauthorized("Invalid email address");
    using var hmac = new HMACSHA512(user.PasswordSalt);
    var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(loginDTO.Password));
    for (int i = 0; i < computedHash.Length; i++)
    {
      if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
    }
    return user;
  }

}
