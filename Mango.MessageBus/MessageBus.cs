using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace Mango.MessageBus
{
    public class MessageBus: IMessageBus
    {
        private readonly string? _connectionString;
        private readonly string _connectionStringAzure;
        public MessageBus(string? connectString)
        {
            _connectionString = connectString;
        }

        public MessageBus()
        {
            _connectionString= Environment.GetEnvironmentVariable("ServiceBusConnectionString") ?? string.Empty;
        }

        public async Task PublishMessage(object message, string topicQueueName)
        {
            //await using var client = new ServiceBusClient(_connectionString);
            await using var client = new ServiceBusClient(_connectionStringAzure);

            ServiceBusSender sender = client.CreateSender(topicQueueName);

            var jsonMessage = JsonConvert.SerializeObject(message);

            ServiceBusMessage sbMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString()
            };

            await sender.SendMessageAsync(sbMessage);
            await client.DisposeAsync();
        }
    }
}
