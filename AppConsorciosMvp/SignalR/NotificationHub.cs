using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AppConsorciosMvp.SignalR
{
    [Authorize]
    public class NotificationHub : Hub
    {
    }
}
