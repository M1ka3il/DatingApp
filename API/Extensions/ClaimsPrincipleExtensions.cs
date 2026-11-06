using System.Security.Claims;

namespace API.Extensions;

public static class ClaimsPrincipleExtensions
{
  public static Guid GetUserId(this ClaimsPrincipal user)
  {
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new Exception("Cannot get user id from token");
    return Guid.Parse(userId);
  }

  public static string GetUsername(this ClaimsPrincipal user)
  {
    return user.FindFirstValue(ClaimTypes.Name)
        ?? throw new Exception("Cannot get username from token");
  }
}
