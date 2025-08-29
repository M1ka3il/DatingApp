using System;
using System.Security.Cryptography;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(AppDbContext context) : BaseAPIController
{
  [HttpPost("register")] //api/account/register

  public async Task<ActionResult<AppUser>> Register(string email, string username, string password)
  {
    if (await UserExists(username)) return BadRequest("Username is taken");
    if (await EmailExists(email)) return BadRequest("Email is taken");
    using var hmac = new HMACSHA512();
    var user = new AppUser
    {
      UserName = username.ToLower(),
      Email = email.ToLower(),
      PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)),
      PasswordSalt = hmac.Key
    };
    return user;
  }

  private async Task<bool> UserExists(string username)
  {
    return await context.Users.AnyAsync(x => x.UserName == username.ToLower());
  } 
  private async Task<bool> EmailExists(string email)
  {
    return await context.Users.AnyAsync(x => x.Email == email.ToLower());
  }


}
