using Azure.Messaging.ServiceBus;
using Mango.Services.EmailAPI.Message;
using Mango.Services.EmailAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer: IAzureServiceBusConsumer
    {
        private readonly string _serviceBusConnectonString;
        private readonly string _emailCartQueue;
        private readonly string _registerUserQueue;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EmailService _emailService;

        private readonly string _orderCreatedTopic;
        private readonly string _orderCreatedEmailSubscription;

        private ServiceBusProcessor _emailOrderPlacedProcessor;

        private ServiceBusProcessor _emailCartProcessor;
        private ServiceBusProcessor _registerUserProcessor;

        private CartDTO _cart;

        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {
            _cart = new CartDTO();
            _cart.CartHeader = new CartHeaderDTO() { Email = "" };

            _emailService = emailService;
            _configuration = configuration;
            
            //_serviceBusConnectonString = _configuration["ServiceBusConnectionString"] ?? string.Empty;
            _serviceBusConnectonString = Environment.GetEnvironmentVariable("SERVICE_BUS_CONN_STRING", EnvironmentVariableTarget.User) ?? string.Empty;

            var client = new ServiceBusClient(_serviceBusConnectonString);

            _emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue") ?? string.Empty;
            _registerUserQueue = _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue") ?? string.Empty;
            _emailCartProcessor = client.CreateProcessor(_emailCartQueue);
            _registerUserProcessor = client.CreateProcessor(_registerUserQueue);


            _orderCreatedTopic = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic") ?? string.Empty;
            _orderCreatedEmailSubscription = _configuration.GetValue<string>("TopicAndQueueNames:OrderCreatedRewardsSubscription") ?? string.Empty;
            _emailOrderPlacedProcessor = client.CreateProcessor(_orderCreatedTopic, _orderCreatedEmailSubscription);
        }

        public async Task Start()
        {
            _emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
            _emailCartProcessor.ProcessErrorAsync += ErrorHandler;

            await _emailCartProcessor.StartProcessingAsync();

            _emailOrderPlacedProcessor.ProcessMessageAsync += OnOrderPlacedRequestReceived;
            _emailOrderPlacedProcessor.ProcessErrorAsync += ErrorHandler;

            await _emailOrderPlacedProcessor.StartProcessingAsync();

            _registerUserProcessor.ProcessMessageAsync += OnRegisterUserRequestReceived;
            _registerUserProcessor.ProcessErrorAsync += ErrorHandler;

            await _registerUserProcessor.StartProcessingAsync();
        }

        public async Task Stop()
        {
            await _emailCartProcessor.StopProcessingAsync();
            await _emailCartProcessor.DisposeAsync();

            await _registerUserProcessor.StopProcessingAsync();
            await _registerUserProcessor.DisposeAsync();

            await _registerUserProcessor.StopProcessingAsync();
            await _registerUserProcessor.DisposeAsync();
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.Message.ToString());
            return Task.CompletedTask;
        }

        private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            _cart = JsonConvert.DeserializeObject<CartDTO>(body);

            try
            {
                await _emailService.EmailCartAndLog(_cart);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception)
            {
                await args.CompleteMessageAsync(null);
            }
        }


        private async Task OnRegisterUserRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            string email = JsonConvert.DeserializeObject<string>(body) ?? string.Empty;

            try
            {
                await _emailService.RegisterUserEmailAndLog(email);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception)
            {
                await args.CompleteMessageAsync(null);
            }
        }

        private async Task OnOrderPlacedRequestReceived(ProcessMessageEventArgs args)
        {
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            RewardsMessage rewardsMessage = JsonConvert.DeserializeObject<RewardsMessage>(body);

            try
            {
                var user = _httpContextAccessor.HttpContext?.User;
                rewardsMessage.UserEmail = user.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;


                await _emailService.LogOrderPlaced(rewardsMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception)
            {
                await args.CompleteMessageAsync(null);
            }
        }
    }
}
