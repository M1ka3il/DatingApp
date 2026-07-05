using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

[Authorize]
public class PresenceHub(PresenceTracker tracker) : Hub
{
  public override async Task OnConnectedAsync()
  {
    var username = Context.User!.GetUsername();
    var isOnline = await tracker.UserConnected(username, Context.ConnectionId);

    if (isOnline)
      await Clients.Others.SendAsync("UserIsOnline", username);

    var currentUsers = await tracker.GetOnlineUsers();
    await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
  }

  public override async Task OnDisconnectedAsync(Exception? exception)
  {
    var username = Context.User!.GetUsername();
    var isOffline = await tracker.UserDisconnected(username, Context.ConnectionId);

    if (isOffline)
      await Clients.Others.SendAsync("UserIsOffline", username);

    await base.OnDisconnectedAsync(exception);
  }
}
