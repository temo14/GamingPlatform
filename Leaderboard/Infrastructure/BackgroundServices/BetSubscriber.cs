using Application.Services;
using Infrastructure.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Infrastructure.BackgroundServices;

internal class BetSubscriber : BackgroundService
{
    private readonly RabbitMQSettings _settings;
    private IConnection? _connection;
    private IChannel? _channel;
    private string? _queueName;
    private readonly Lazy<Task> _initializeTask;
    private readonly IBetEventProcessor _eventProcessor;
    private readonly ILogger<BetSubscriber> _logger;

    public BetSubscriber(IOptions<RabbitMQSettings> options, IBetEventProcessor eventProcessor, ILogger<BetSubscriber> logger)
    {
        _settings = options.Value;
        _initializeTask = new Lazy<Task>(InitializeRabbitMQ);
        _eventProcessor = eventProcessor;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        await _initializeTask.Value;

        var consumer = new AsyncEventingBasicConsumer(_channel!);
        consumer.ReceivedAsync += async (ch, ea) =>
        {
            _logger.LogInformation("-->Bet Event Received!");

            var body = ea.Body.ToArray();
            var notificationMessage = Encoding.UTF8.GetString(body);

            await _eventProcessor.ProcessBetEventAsync(notificationMessage);

            await Task.CompletedTask;
        };

        await _channel!.BasicConsumeAsync(queue: _queueName!, autoAck: true, consumer: consumer);
    }

    public async ValueTask DisposeAsync()
    {
        _logger.LogInformation("MessageBus Disposed");
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

        await _channel.ExchangeDeclareAsync(exchange: _settings.BetExchange, type: ExchangeType.Fanout);

        var queue = await _channel.QueueDeclareAsync();
        _queueName = queue.QueueName;
        await _channel.QueueBindAsync(queue: _queueName, exchange: _settings.BetExchange, string.Empty);

        _logger.LogInformation("--> Listening on the Message Bus...");

        _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdown;
    }
    private Task RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        _logger.LogInformation("--> Connection Shutdown");
        return Task.CompletedTask;
    }
}
