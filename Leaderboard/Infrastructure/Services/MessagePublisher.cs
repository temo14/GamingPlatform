using Application.Services;
using Domain.Entities;
using Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Services
{
    internal class MessagePublisher(
        IOptions<RabbitMQSettings> options,
        ILogger<MessagePublisher> logger) : IMessagePublisher, IAsyncDisposable
    {
        private RabbitMQSettings _settings = options.Value;
        private readonly ConnectionFactory factory = new()
        {
            HostName = options.Value.Hostname,
            Port = int.Parse(options.Value.Port)
        };
        private IConnection? _connection;
        private IChannel? _channel;
        public async Task InitializeAsync()
        {
            try
            {
                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                await _channel.ExchangeDeclareAsync(exchange: _settings.PrizeExchange, type: ExchangeType.Fanout);

                logger.LogInformation("--> Connected to Prize MessageBus");
            }
            catch (Exception ex)
            {
                logger.LogError($"--> Could not connect to the Message Bus: {ex.Message}");
            }
        }
        public async Task PublishMessageAsync(Prize message)
        {
            if (_connection == null || _channel == null)
            {
                logger.LogWarning("--> Connection or channel is not initialized.");
                return;
            }

            if (_connection.IsOpen)
            {
                logger.LogInformation("-->Prize RabbitMQ Connection Open, sending message...");
                var messageJson = JsonSerializer.Serialize(message);
                await SendMessage(messageJson);
            }
            else
            {
                logger.LogWarning("-->Prize RabbitMQ connection is closed, not sending");
            }
        }
        private async Task SendMessage(string message)
        {
            if (_channel == null)
            {
                logger.LogWarning("-->Prize Channel is not initialized.");
                return;
            }

            var body = Encoding.UTF8.GetBytes(message);

            await _channel.BasicPublishAsync(exchange: _settings.PrizeExchange, routingKey: string.Empty, body: body);

            logger.LogWarning($"--> We have sent {message}");
        }
        public async ValueTask DisposeAsync()
        {
            logger.LogWarning("Prize MessageBus Disposed");
            if (_channel != null && _channel.IsOpen)
            {
                await _channel.CloseAsync();
            }

            if (_connection != null && _connection.IsOpen)
            {
                await _connection.CloseAsync();
            }
        }
    }
}
