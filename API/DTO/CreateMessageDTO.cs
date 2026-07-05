namespace API.DTO;

public class CreateMessageDTO
{
  public Guid RecipientId { get; set; }
  public required string Content { get; set; }
}
