using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AppConsorciosMvp.SignalR
{
    [Authorize]
    public class ChatHub : Hub
    {
        public async Task JoinThread(string threadId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"thread:{threadId}");
        }

        public async Task LeaveThread(string threadId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"thread:{threadId}");
        }
    }
}
