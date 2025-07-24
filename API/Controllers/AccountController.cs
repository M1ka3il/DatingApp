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
    var hmac = new HMACSHA512();
    
  }


}
