using System;

namespace API.DTO;

public class MemberDTO
{
  public Guid Id { get; set; }
  public required string UserName { get; set; }
  public string? ImageUrl { get; set; }
  public List<PhotoDTO> Photos { get; set; } = [];
}
