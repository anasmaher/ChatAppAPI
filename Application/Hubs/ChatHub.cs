using Microsoft.AspNetCore.SignalR;

namespace ChatAppAPI.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMsg(string userId, string msg)
        {
            await Clients.All.SendAsync("ReceiveMessage", userId, msg);
        }

        public async Task JoinedChat(string userId, string msg)
        {
            await Clients.Others.SendAsync("ReceiveMessage", userId, msg);
        }
    }
}
