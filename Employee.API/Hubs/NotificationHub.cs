using Microsoft.AspNetCore.SignalR;

namespace Employee.API.Hubs
{
    public class NotificationHub : Hub
    {
        // Hub methods for client-server communication
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
