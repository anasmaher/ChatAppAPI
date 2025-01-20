using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatAppAPI.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
    }
}
