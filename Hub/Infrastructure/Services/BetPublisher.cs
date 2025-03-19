using Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using Application.Features.Bet.DTOs;
using Application.Services;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

internal class BetPublisher(
    IOptions<RabbitMQSettings> options,
    ILogger<BetPublisher> logger) : IMessagePublisher, IAsyncDisposable
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

            await _channel.ExchangeDeclareAsync(exchange: _settings.BetExchange, type: ExchangeType.Fanout);

            logger.LogInformation("--> Connected to Bet MessageBus");
        }
        catch (Exception ex)
        {
            logger.LogError($"--> Could not connect to the Message Bus: {ex.Message}");
        }
    }

    public async Task PublishMessageAsync(BetPlacedEventDto eventDto)
    {
        if (_connection == null || _channel == null)
        {
            logger.LogInformation("--> Connection or channel is not initialized.");
            return;
        }

        var message = JsonSerializer.Serialize(eventDto);

        if (_connection.IsOpen)
        {
            logger.LogInformation("--> RabbitMQ Connection Open, sending message...");
            await SendMessage(message);
        }
        else
        {
            logger.LogWarning("--> RabbitMQ connection is closed, not sending");
        }
    }
    public async ValueTask DisposeAsync()
    {
        if (_channel != null && _channel.IsOpen)
        {
            await _channel.CloseAsync();
        }

        if (_connection != null && _connection.IsOpen)
        {
            await _connection.CloseAsync();
        }
    }
    private async Task SendMessage(string message)
    {
        if (_channel == null)
        {
            logger.LogWarning("--> Channel is not initialized.");
            return;
        }

        var body = Encoding.UTF8.GetBytes(message);

        await _channel.BasicPublishAsync(exchange: _settings.BetExchange, routingKey: string.Empty, body: body);

        logger.LogInformation($"--> We have sent {message}");
    }
}
