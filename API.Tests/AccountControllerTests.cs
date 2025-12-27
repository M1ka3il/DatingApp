using System.Security.Cryptography;
using System.Text;
using API.Controllers;
using API.Data;
using API.DTO;
using API.Entities;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace API.Tests.Controllers
{
  public class AccountControllerTests
  {
    private static AppDbContext CreateContext()
    {
      var options = new DbContextOptionsBuilder<AppDbContext>()
          .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
          .Options;

      return new AppDbContext(options);
    }

    private static TokenService CreateTokenService()
    {
      var config = new ConfigurationBuilder()
          .AddInMemoryCollection(new Dictionary<string, string?>
          {
            // exakt 64 Zeichen (reicht, weil dein Service < 64 verbietet)
            ["TokenKey"] = new string('A', 64)
          })
          .Build();

      return new TokenService(config);
    }


    private static AccountController CreateController(AppDbContext context)
        => new AccountController(context, CreateTokenService());

    private static AppUser CreateUser(string username, string email, string password)
    {
      using var hmac = new HMACSHA512();
      return new AppUser
      {
        UserName = username.ToLower(),
        Email = email.ToLower(),
        PasswordSalt = hmac.Key,
        PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password))
      };
    }
    private static void AssertJwt(string? token)
    {
      Assert.False(string.IsNullOrWhiteSpace(token));

      var parts = token!.Split('.');
      Assert.Equal(3, parts.Length);

      Assert.All(parts, p => Assert.False(string.IsNullOrWhiteSpace(p)));

      var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
      Assert.True(handler.CanReadToken(token));
    }

    [Fact]
    public async Task Register_WhenUsernameTaken_ReturnsBadRequest()
    {
      // Arrange
      await using var context = CreateContext();
      context.Users.Add(CreateUser("micha", "micha@test.de", "Pa$$w0rd"));
      await context.SaveChangesAsync();

      var controller = CreateController(context);

      var dto = new RegisterDTO
      {
        UserName = "Micha", // bewusst mit anderer Groß/Kleinschreibung
        Email = "new@test.de",
        Password = "whatever"
      };

      // Act
      var result = await controller.Register(dto);

      // Assert
      var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
      Assert.Equal("Username is taken", badRequest.Value);
    }

    [Fact]
    public async Task Register_WhenEmailTaken_ReturnsBadRequest()
    {
      // Arrange
      await using var context = CreateContext();
      context.Users.Add(CreateUser("micha", "micha@test.de", "Pa$$w0rd"));
      await context.SaveChangesAsync();

      var controller = CreateController(context);

      var dto = new RegisterDTO
      {
        UserName = "newuser",
        Email = "MICHA@TEST.DE", // anderer Case
        Password = "whatever"
      };

      // Act
      var result = await controller.Register(dto);

      // Assert
      var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
      Assert.Equal("Email is taken", badRequest.Value);
    }

    [Fact]
    public async Task Register_WhenValid_CreatesUser_AndReturnsUserDto()
    {
      // Arrange
      await using var context = CreateContext();
      var controller = CreateController(context);

      var dto = new RegisterDTO
      {
        UserName = "MiCha",
        Email = "MiCha@Example.com",
        Password = "Pa$$w0rd"
      };

      // Act
      var result = await controller.Register(dto);

      // Assert: Bei `return user.ToUserDTO(...)` ist bei ActionResult<T> meistens Value gesetzt und Result null
      Assert.Null(result.Result);
      Assert.NotNull(result.Value);

      Assert.Equal("micha", result.Value!.UserName); // DTO sollte lowercase username enthalten (abhängig von ToUserDTO)
      AssertJwt(result.Value.Token);
      
      // Und wirklich in DB gespeichert?
      var saved = await context.Users.SingleOrDefaultAsync(u => u.Email == "micha@example.com");
      Assert.NotNull(saved);
      Assert.Equal("micha", saved!.UserName);
      Assert.Equal("micha@example.com", saved.Email);
      Assert.NotNull(saved.PasswordHash);
      Assert.NotNull(saved.PasswordSalt);
    }

    [Fact]
    public async Task Login_WhenEmailNotFound_ReturnsUnauthorized()
    {
      // Arrange
      await using var context = CreateContext();
      var controller = CreateController(context);

      var dto = new LoginDTO
      {
        Email = "unknown@test.de",
        Password = "Pa$$w0rd"
      };

      // Act
      var result = await controller.Login(dto);

      // Assert
      var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result.Result);
      Assert.Equal("Invalid email address", unauthorized.Value);
    }

    [Fact]
    public async Task Login_WhenPasswordWrong_ReturnsUnauthorized()
    {
      // Arrange
      await using var context = CreateContext();
      context.Users.Add(CreateUser("micha", "micha@test.de", "CorrectPassword"));
      await context.SaveChangesAsync();

      var controller = CreateController(context);

      var dto = new LoginDTO
      {
        Email = "MICHA@TEST.DE",
        Password = "WrongPassword"
      };

      // Act
      var result = await controller.Login(dto);

      // Assert
      var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result.Result);
      Assert.Equal("Invalid password", unauthorized.Value);
    }

    [Fact]
    public async Task Login_WhenValid_ReturnsUserDto()
    {
      // Arrange
      await using var context = CreateContext();
      context.Users.Add(CreateUser("micha", "micha@test.de", "CorrectPassword"));
      await context.SaveChangesAsync();

      var controller = CreateController(context);

      var dto = new LoginDTO
      {
        Email = "micha@test.de",
        Password = "CorrectPassword"
      };

      // Act
      var result = await controller.Login(dto);

      // Assert
      Assert.Null(result.Result);
      Assert.NotNull(result.Value);

      Assert.Equal("micha", result.Value!.UserName);
      AssertJwt(result.Value.Token);
    }
  }
}
