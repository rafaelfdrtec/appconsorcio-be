using System.Text.Json;
using Azure.Storage.Queues;

namespace AppConsorciosMvp.Notifications
{
    public class AzureStorageNotificationQueue : INotificationQueue
    {
        private readonly QueueClient _queueClient;

        public AzureStorageNotificationQueue(IConfiguration configuration)
        {
            var conn = configuration["Queue:Conn"] ?? configuration.GetConnectionString("AzureQueue");
            if (string.IsNullOrWhiteSpace(conn))
                throw new InvalidOperationException("Config da fila (Queue:Conn ou ConnectionStrings:AzureQueue) não encontrada.");

            var queueName = configuration["Queue:Name"] ?? "notifications";
            _queueClient = new QueueClient(conn, queueName);
            _queueClient.CreateIfNotExists();
        }

        public async Task EnqueueAsync(NotificationJob job)
        {
            var json = JsonSerializer.Serialize(job);
            await _queueClient.SendMessageAsync(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json)));
        }
    }
}
