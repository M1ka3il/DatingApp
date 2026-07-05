using API.Data;
using API.DTO;
using API.Entities;
using API.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace API.SignalR;

[Authorize]
public class MessageHub(AppDbContext context, IMapper mapper, PresenceTracker tracker) : Hub
{
  public override async Task OnConnectedAsync()
  {
    var httpContext = Context.GetHttpContext();
    var otherUser = httpContext?.Request.Query["user"].ToString();
    if (string.IsNullOrEmpty(otherUser))
      throw new HubException("Other user not provided");

    var currentUserId = Context.User!.GetUserId();
    var otherUserId = Guid.Parse(otherUser);

    var groupName = GetGroupName(currentUserId, otherUserId);
    await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

    var messages = await GetMessageThread(currentUserId, otherUserId);
    await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
  }

  public async Task SendMessage(CreateMessageDTO createMessageDto)
  {
    var senderId = Context.User!.GetUserId();
    if (senderId == createMessageDto.RecipientId)
      throw new HubException("You cannot message yourself");

    var sender = await context.Users.FindAsync(senderId);
    var recipient = await context.Users.FindAsync(createMessageDto.RecipientId);
    if (sender == null || recipient == null)
      throw new HubException("Cannot send message at this time");

    var message = new Message
    {
      SenderId = sender.Id,
      SenderUsername = sender.UserName,
      RecipientId = recipient.Id,
      RecipientUsername = recipient.UserName,
      Content = createMessageDto.Content,
    };

    var groupName = GetGroupName(sender.Id, recipient.Id);

    // Mark read immediately if the recipient is currently viewing the thread.
    var connections = await tracker.GetConnectionsForUser(recipient.UserName);

    context.Messages.Add(message);
    await context.SaveChangesAsync();

    var messageDto = await context.Messages
        .Where(m => m.Id == message.Id)
        .ProjectTo<MessageDTO>(mapper.ConfigurationProvider)
        .SingleAsync();

    await Clients.Group(groupName).SendAsync("NewMessage", messageDto);
  }

  public override async Task OnDisconnectedAsync(Exception? exception)
  {
    await base.OnDisconnectedAsync(exception);
  }

  private static string GetGroupName(Guid a, Guid b)
  {
    // Deterministic group name regardless of who connects first.
    var ordered = new[] { a, b }.OrderBy(x => x).Select(x => x.ToString());
    return string.Join("-", ordered);
  }

  private async Task<List<MessageDTO>> GetMessageThread(Guid currentUserId, Guid otherUserId)
  {
    var unread = await context.Messages
        .Where(m => m.RecipientId == currentUserId
            && m.SenderId == otherUserId
            && m.DateRead == null)
        .ToListAsync();

    if (unread.Count != 0)
    {
      unread.ForEach(m => m.DateRead = DateTime.UtcNow);
      await context.SaveChangesAsync();
    }

    return await context.Messages
        .Where(m =>
            (m.RecipientId == currentUserId && !m.RecipientDeleted && m.SenderId == otherUserId) ||
            (m.SenderId == currentUserId && !m.SenderDeleted && m.RecipientId == otherUserId))
        .OrderBy(m => m.MessageSent)
        .ProjectTo<MessageDTO>(mapper.ConfigurationProvider)
        .ToListAsync();
  }
}
