using API.Controllers;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    private AppUser CreateUser(Guid? id = null)
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

        var controller = new MembersController(context);

        // Act
        ActionResult<IReadOnlyList<AppUser>> result = await controller.GetMembers();

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

        var controller = new MembersController(context);

        // Act
        ActionResult<AppUser> result = await controller.GetMemberByID(id);

        // Assert
        Assert.NotNull(result.Value);
        Assert.Equal(id, result.Value!.Id);
        Assert.Equal("jane", result.Value.UserName);
        Assert.Equal("jane@test.com", result.Value.Email);
    }

    [Fact]
    public async Task GetMemberByID_WhenMissing_ReturnsNotFound()
    {
        // Arrange
        await using var context = CreateContext(nameof(GetMemberByID_WhenMissing_ReturnsNotFound));
        var controller = new MembersController(context);

        // Act
        ActionResult<AppUser> result = await controller.GetMemberByID(Guid.NewGuid());

        // Assert
        Assert.Null(result.Value);
        Assert.IsType<NotFoundResult>(result.Result);
    }
}
