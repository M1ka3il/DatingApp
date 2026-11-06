using API.Data;
using API.DTO;
using API.Entities;
using API.Extensions;
using API.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class MessagesController(AppDbContext context, IMapper mapper) : BaseAPIController
{
  [HttpPost]
  public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO createMessageDto)
  {
    var senderId = User.GetUserId();
    if (senderId == createMessageDto.RecipientId)
      return BadRequest("You cannot message yourself");

    var sender = await context.Users.FindAsync(senderId);
    var recipient = await context.Users.FindAsync(createMessageDto.RecipientId);
    if (sender == null || recipient == null)
      return BadRequest("Cannot send message at this time");

    var message = new Message
    {
      SenderId = sender.Id,
      SenderUsername = sender.UserName,
      RecipientId = recipient.Id,
      RecipientUsername = recipient.UserName,
      Content = createMessageDto.Content,
    };

    context.Messages.Add(message);
    await context.SaveChangesAsync();

    return await MapMessage(message.Id);
  }

  [HttpGet]
  public async Task<ActionResult<IReadOnlyList<MessageDTO>>> GetMessagesForUser(
      [FromQuery] MessageParams messageParams)
  {
    var userId = User.GetUserId();

    var query = context.Messages.OrderByDescending(m => m.MessageSent).AsQueryable();

    query = messageParams.Container switch
    {
      "Outbox" => query.Where(m => m.SenderId == userId && !m.SenderDeleted),
      "Unread" => query.Where(m => m.RecipientId == userId && !m.RecipientDeleted && m.DateRead == null),
      _ => query.Where(m => m.RecipientId == userId && !m.RecipientDeleted),
    };

    var messages = await PagedList<MessageDTO>.CreateAsync(
        query.ProjectTo<MessageDTO>(mapper.ConfigurationProvider),
        messageParams.PageNumber,
        messageParams.PageSize);

    Response.AddPaginationHeader(messages);

    return messages;
  }

  [HttpGet("thread/{userId}")]
  public async Task<ActionResult<IReadOnlyList<MessageDTO>>> GetMessageThread(Guid userId)
  {
    var currentUserId = User.GetUserId();

    var unread = await context.Messages
        .Where(m => m.RecipientId == currentUserId
            && m.SenderId == userId
            && m.DateRead == null)
        .ToListAsync();

    if (unread.Count != 0)
    {
      unread.ForEach(m => m.DateRead = DateTime.UtcNow);
      await context.SaveChangesAsync();
    }

    var messages = await context.Messages
        .Where(m =>
            (m.RecipientId == currentUserId && !m.RecipientDeleted && m.SenderId == userId) ||
            (m.SenderId == currentUserId && !m.SenderDeleted && m.RecipientId == userId))
        .OrderBy(m => m.MessageSent)
        .ProjectTo<MessageDTO>(mapper.ConfigurationProvider)
        .ToListAsync();

    return messages;
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult> DeleteMessage(int id)
  {
    var userId = User.GetUserId();
    var message = await context.Messages.FindAsync(id);
    if (message == null) return NotFound();

    if (message.SenderId != userId && message.RecipientId != userId)
      return Forbid();

    if (message.SenderId == userId) message.SenderDeleted = true;
    if (message.RecipientId == userId) message.RecipientDeleted = true;

    // Once both parties have deleted it, remove it for good.
    if (message.SenderDeleted && message.RecipientDeleted)
      context.Messages.Remove(message);

    await context.SaveChangesAsync();
    return Ok();
  }

  private async Task<MessageDTO> MapMessage(int id)
  {
    return await context.Messages
        .Where(m => m.Id == id)
        .ProjectTo<MessageDTO>(mapper.ConfigurationProvider)
        .SingleAsync();
  }
}
