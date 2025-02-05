using Azure.Messaging.ServiceBus;
using Mango.Services.RewardAPI.Message;
using Mango.Services.RewardAPI.Services;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.RewardAPI.Messaging
{
    public class AzureServiceBusConsumer: IAzureServiceBusConsumer
    {
        private readonly string _serviceBusConnectonString;
        private readonly string _orderCreatedTopic;
        private readonly string _orderCreatedRewardsSubscription;
        private readonly IConfiguration _configuration;
        private readonly RewardsService _rewardsService;

        private ServiceBusProcessor _rewardsProcessor;

        public AzureServiceBusConsumer(IConfiguration configuration, RewardsService rewardsService)
        {
            _rewardsService = rewardsService;
            _configuration = configuration;

            //_serviceBusConnectonString = _configuration["ServiceBusConnectionString"] ?? string.Empty;
            _serviceBusConnectonString = Environment.GetEnvironmentVariable("SERVICE_BUS_CONN_STRING", EnvironmentVariableTarget.User) ?? string.Empty;

            _orderCreatedTopic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic") ?? string.Empty;
            _orderCreatedRewardsSubscription = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedRewardsSubscription") ?? string.Empty;

            var client = new ServiceBusClient(_serviceBusConnectonString);
            _rewardsProcessor = client.CreateProcessor(_orderCreatedTopic, _orderCreatedRewardsSubscription);
        }

        public async Task Start()
        {
            _rewardsProcessor.ProcessMessageAsync += OnNewOrderRewardsRequestReceived;
            _rewardsProcessor.ProcessErrorAsync += ErrorHandler;

            await _rewardsProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _rewardsProcessor.StopProcessingAsync();
            await _rewardsProcessor.DisposeAsync();
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.Message.ToString());
            return Task.CompletedTask;
        }

        private async Task OnNewOrderRewardsRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            RewardsMessage rewards = JsonConvert.DeserializeObject<RewardsMessage>(body);

            try
            {
                await _rewardsService.UpdateRewards(rewards);
                await args.CompleteMessageAsync(args.Message);

            }
            catch (Exception ex)
            {
                await args.CompleteMessageAsync(null);
            }
        }
    }
}
