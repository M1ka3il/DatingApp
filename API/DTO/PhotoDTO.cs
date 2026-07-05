namespace API.DTO;

public class PhotoDTO
{
  public int Id { get; set; }
  public required string Url { get; set; }
  public bool IsMain { get; set; }
}
