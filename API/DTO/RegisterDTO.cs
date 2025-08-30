using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTO;

public class RegisterDTO
{
  [Required]
  public string Username { get; set; }
  [Required]
  [EmailAddress]
  public string Email { get; set; }

  [Required]
  [MinLength(4, ErrorMessage = "Password must be at least 4 characters")]
  public string Password { get; set; }

}
