using Microsoft.AspNetCore.SignalR;

namespace SignalR.Hubs
{
    public class ChatHub : Hub
    {
        public async Task NewMessage(string name, string message)
        {
            // logic
            await Clients.All.SendAsync("NewMessageNotify", name, message);
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
    }
}
