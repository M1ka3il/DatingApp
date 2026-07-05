namespace API.Entities;

public class Photo
{
  public int Id { get; set; }
  public required string Url { get; set; }
  public bool IsMain { get; set; }

  // Set once the photo lives in cloud storage; null for locally stored files.
  public string? PublicId { get; set; }

  // Navigation properties
  public Guid AppUserId { get; set; }
  public AppUser AppUser { get; set; } = null!;
}
