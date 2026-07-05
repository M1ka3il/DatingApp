namespace API.Helpers;

public class MessageParams : PaginationParams
{
  // "Inbox" (default), "Outbox" or "Unread".
  public string Container { get; set; } = "Inbox";
}
