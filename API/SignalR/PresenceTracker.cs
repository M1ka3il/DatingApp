using System.Collections.Concurrent;

namespace API.SignalR;

// Tracks which users are currently connected (a user may have several connections/tabs).
public class PresenceTracker
{
  private static readonly ConcurrentDictionary<string, List<string>> OnlineUsers = new();

  public Task<bool> UserConnected(string username, string connectionId)
  {
    var isOnline = false;
    lock (OnlineUsers)
    {
      if (OnlineUsers.TryGetValue(username, out var connections))
      {
        connections.Add(connectionId);
      }
      else
      {
        OnlineUsers[username] = [connectionId];
        isOnline = true;
      }
    }
    return Task.FromResult(isOnline);
  }

  public Task<bool> UserDisconnected(string username, string connectionId)
  {
    var isOffline = false;
    lock (OnlineUsers)
    {
      if (!OnlineUsers.TryGetValue(username, out var connections))
        return Task.FromResult(isOffline);

      connections.Remove(connectionId);
      if (connections.Count == 0)
      {
        OnlineUsers.TryRemove(username, out _);
        isOffline = true;
      }
    }
    return Task.FromResult(isOffline);
  }

  public Task<string[]> GetOnlineUsers()
  {
    string[] onlineUsers;
    lock (OnlineUsers)
    {
      onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
    }
    return Task.FromResult(onlineUsers);
  }

  public Task<List<string>> GetConnectionsForUser(string username)
  {
    List<string> connectionIds;
    lock (OnlineUsers)
    {
      connectionIds = OnlineUsers.GetValueOrDefault(username) ?? [];
    }
    return Task.FromResult(connectionIds);
  }
}
