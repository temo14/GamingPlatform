using Infrastructure.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Application.Services;
using System.Text.Json;
using Domain.Models;

namespace Infrastructure.BackgroundServices;

internal class PrizeSubscriber : BackgroundService
{
    private readonly RabbitMQSettings _settings;
    private IConnection? _connection;
    private IChannel? _channel;
    private string? _queueName;
    private readonly Lazy<Task> _initializeTask;
    private readonly IPrizeNotificationService _notificationService;

    public PrizeSubscriber(IOptions<RabbitMQSettings> options, IPrizeNotificationService notificationService)
    {
        _settings = options.Value;
        _initializeTask = new Lazy<Task>(InitializeRabbitMQ);
        _notificationService = notificationService;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        await _initializeTask.Value;

        var consumer = new AsyncEventingBasicConsumer(_channel!);
        consumer.ReceivedAsync += async (ch, ea) =>
        {
            Console.WriteLine("-->Prize Event Received!");

            var body = ea.Body.ToArray();
            var notificationMessage = Encoding.UTF8.GetString(body);

            var prizeNotification = JsonSerializer.Deserialize<PrizeNotification>(notificationMessage);
            if (prizeNotification != null)
            {
                Console.WriteLine($"Processing prize notification for player {prizeNotification.PlayerId}, amount: {prizeNotification.Amount}");
                await _notificationService.SendPrizeNotification(prizeNotification.PlayerId.ToString(), prizeNotification.Amount);
            }
            await Task.CompletedTask;
        };

        await _channel!.BasicConsumeAsync(queue: _queueName!, autoAck: true, consumer: consumer);
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
    private async Task InitializeRabbitMQ()
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.Hostname,
            Port = int.Parse(_settings.Port)
        };

        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await _channel.ExchangeDeclareAsync(exchange: _settings.PrizeExchange, type: ExchangeType.Fanout);

        var queue = await _channel.QueueDeclareAsync();
        _queueName = queue.QueueName;
        await _channel.QueueBindAsync(queue: _queueName, exchange: _settings.PrizeExchange, string.Empty);
    }
}
