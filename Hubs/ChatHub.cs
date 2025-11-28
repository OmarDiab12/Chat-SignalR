using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SignalR.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, HashSet<string>> UserConnections = new(StringComparer.OrdinalIgnoreCase);

        public async Task NewMessage(string name, string message)
        {
            await Clients.All.SendAsync("NewMessageNotify", name, message);
        }

        public async Task<IEnumerable<string>> GetOnlineUsers()
        {
            return await Task.FromResult(UserConnections.Keys.OrderBy(x => x).ToList().AsEnumerable());
        }

        public async Task SendPrivateMessage(string recipientUserName, string message)
        {
            if (string.IsNullOrWhiteSpace(recipientUserName))
            {
                throw new HubException("Recipient is required.");
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new HubException("Message cannot be empty.");
            }

            var sender = Context.User?.Identity?.Name;
            if (string.IsNullOrWhiteSpace(sender))
            {
                throw new HubException("User is not authenticated.");
            }

            var trimmedMessage = message.Trim();
            var sentAt = DateTime.UtcNow;

            await Clients.User(recipientUserName).SendAsync("ReceivePrivateMessage", sender, trimmedMessage, sentAt, sender);
            await Clients.Caller.SendAsync("ReceivePrivateMessage", sender, trimmedMessage, sentAt, recipientUserName);
        }

        public override async Task OnConnectedAsync()
        {
            var userName = Context.User?.Identity?.Name;
            if (!string.IsNullOrWhiteSpace(userName))
            {
                var connections = UserConnections.GetOrAdd(userName, _ => new HashSet<string>());
                lock (connections)
                {
                    connections.Add(Context.ConnectionId);
                }

                await NotifyUsersAsync();
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userName = Context.User?.Identity?.Name;
            if (!string.IsNullOrWhiteSpace(userName) && UserConnections.TryGetValue(userName, out var connections))
            {
                lock (connections)
                {
                    connections.Remove(Context.ConnectionId);
                    if (connections.Count == 0)
                    {
                        UserConnections.TryRemove(userName, out _);
                    }
                }

                await NotifyUsersAsync();
            }

            await base.OnDisconnectedAsync(exception);
        }

        private Task NotifyUsersAsync()
        {
            var users = UserConnections.Keys.OrderBy(x => x).ToList();
            return Clients.All.SendAsync("UsersUpdated", users);
        }
    }
}
