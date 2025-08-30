using System;
using System.Security.Cryptography;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(AppDbContext context) : BaseAPIController
{
  [HttpPost("register")] //api/account/register
  public async Task<ActionResult<AppUser>> Register(RegisterDTO registerDto)
  {

    if (await UserExists(registerDto.UserName)) return BadRequest("Username is already taken.");
    
    if (await EmailExists(registerDto.Email)) return BadRequest("Email is already registered.");

    using var hmac = new HMACSHA512();

    var user = new AppUser
    {
      UserName = registerDto.UserName,
      Email = registerDto.Email,
      PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(registerDto.Password)),
      PasswordSalt = hmac.Key,
    };

    context.Users.Add(user);
    await context.SaveChangesAsync();
    return user;
  }
  private async Task<bool> UserExists(string userName)
  {
    return await context.Users.AnyAsync(x => x.UserName.ToLower() == userName.ToLower());
  }
  private async Task<bool> EmailExists(string email)
  {
    return await context.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower());
  }
}
