namespace API.Entities;

public class Message
{
  public int Id { get; set; }
  public required string Content { get; set; }
  public DateTime? DateRead { get; set; }
  public DateTime MessageSent { get; set; } = DateTime.UtcNow;

  // Soft-delete flags: the message is only removed once both parties delete it.
  public bool SenderDeleted { get; set; }
  public bool RecipientDeleted { get; set; }

  // Sender
  public Guid SenderId { get; set; }
  public required string SenderUsername { get; set; }
  public AppUser Sender { get; set; } = null!;

  // Recipient
  public Guid RecipientId { get; set; }
  public required string RecipientUsername { get; set; }
  public AppUser Recipient { get; set; } = null!;
}
