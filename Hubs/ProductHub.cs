using Microsoft.AspNetCore.SignalR;

namespace SignalR.Hubs
{
    public class ProductHub : Hub
    {
        public async Task NotifyProductAdded(string name)
        {
            await Clients.All.SendAsync("AddNewProduct", name);
        }
    }
}
