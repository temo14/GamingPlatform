namespace Infrastructure.Configuration;

internal class RabbitMQSettings
{
    public string Hostname { get; set; } = null!;
    public string Port { get; set; } = null!;
    public string BetExchange { get; set; } = null!;
    public string PrizeExchange { get; set; } = null!;
}
