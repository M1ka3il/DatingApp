using System;

namespace API.Entities;

public class AppUser
{
  // Es kann zu Problemen beim Abruf der bestehenden Einträge kommen, da ich dort eine String-ID verwendet habe. Muss noch deletet werden aus der SQLite tabelle
  // public required string Id { get; set; }
  public  Guid Id { get; set; } = Guid.NewGuid();
  public required string UserName { get; set; }
  public required string Email { get; set; }
  public required byte[] PasswordHash { get; set; }
  public required byte[] PasswordSalt { get; set; }
}
