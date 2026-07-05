namespace API.DTO;

public class MessageDTO
{
  public int Id { get; set; }
  public Guid SenderId { get; set; }
  public required string SenderUsername { get; set; }
  public string? SenderPhotoUrl { get; set; }
  public Guid RecipientId { get; set; }
  public required string RecipientUsername { get; set; }
  public string? RecipientPhotoUrl { get; set; }
  public required string Content { get; set; }
  public DateTime? DateRead { get; set; }
  public DateTime MessageSent { get; set; }
}
