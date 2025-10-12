using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTO;

public class RegisterDTO
{
  [Required]
  [DataType(DataType.Text)]
  public string UserName { get; set; } = "";

  [Required]
  [EmailAddress]
  [DataType(DataType.EmailAddress)]
  public string Email { get; set; } = "";

  [Required]
  [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
  [DataType(DataType.Password)]
  public string Password { get; set; } = "";
}
