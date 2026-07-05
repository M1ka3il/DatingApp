using API.Controllers;
using API.Data;
using API.DTO;
using API.Entities;
using API.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

public class MembersControllerTests
{
    private static AppDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;

        return new AppDbContext(options);
    }

    private static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(
            cfg => cfg.AddProfile<AutoMapperProfiles>(),
            NullLoggerFactory.Instance);
        return config.CreateMapper();
    }

    private AppUser CreateUser(Guid? id = null, AppUser? user = null)
    {
        return new AppUser
        {
            Id = id ?? Guid.NewGuid(),
            UserName = "jane",
            Email = "jane@test.com",
            PasswordHash = new byte[] { 1, 2, 3 },
            PasswordSalt = new byte[] { 4, 5, 6 }
        };
    }

    [Fact]
    public async Task GetMembers_ReturnsAllUsers()
    {
        // Arrange
        await using var context = CreateContext(nameof(GetMembers_ReturnsAllUsers));

        context.Users.AddRange(
            CreateUser(Guid.NewGuid()),
            CreateUser(Guid.NewGuid())
        );
        await context.SaveChangesAsync();

        var controller = new MembersController(context, CreateMapper());

        // Act
        ActionResult<IReadOnlyList<MemberDTO>> result = await controller.GetMembers();

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value!.Count);
    }

    [Fact]
    public async Task GetMemberByID_WhenExists_ReturnsUser()
    {
        // Arrange
        await using var context = CreateContext(nameof(GetMemberByID_WhenExists_ReturnsUser));

        var id = Guid.NewGuid();
        var user = CreateUser(id);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var controller = new MembersController(context, CreateMapper());

        // Act
        ActionResult<MemberDTO> result = await controller.GetMemberByID(id);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(id, result.Value!.Id);
        Assert.Equal("jane", result.Value.UserName);
    }

    [Fact]
    public async Task GetMemberByID_WhenMissing_ReturnsNotFound()
    {
        // Arrange
        await using var context = CreateContext(nameof(GetMemberByID_WhenMissing_ReturnsNotFound));
        var controller = new MembersController(context, CreateMapper());

        // Act
        ActionResult<MemberDTO> result = await controller.GetMemberByID(Guid.NewGuid());

        // Assert
        Assert.Null(result.Value);
        Assert.IsType<NotFoundResult>(result.Result);
    }
}
