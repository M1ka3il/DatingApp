using System;

namespace API.DTO;

public class UserDTO
{
  public required Guid id { get; set; }
  public required string UserName { get; set; }
  public required string Email { get; set; }
  public string? ImageUrl { get; set; }
  public required string Token { get; set; }

}

