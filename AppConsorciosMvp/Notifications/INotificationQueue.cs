namespace AppConsorciosMvp.Notifications
{
    public interface INotificationQueue
    {
        Task EnqueueAsync(NotificationJob job);
    }

    public record NotificationJob(int UserId, string Type, string PayloadJson);
}
