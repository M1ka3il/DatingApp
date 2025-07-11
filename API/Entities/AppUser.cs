using System;

namespace API.Entities;

public class AppUser
{
  public required Guid Id { get; set; } = Guid.NewGuid();
  public required string UserName { get; set; }
  public required string EMail { get; set; } 
  
}
