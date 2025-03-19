using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace SignalRTestClient;

class Program
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private static readonly string _hubServiceUrl = "http://localhost:5000";
    private static readonly string _gameServiceUrl = "http://localhost:7000";
    private static readonly string _leaderboardServiceUrl = "http://localhost:8080";
    private static readonly string _signalRHubUrl = $"{_hubServiceUrl}/hubs/notifications";
    private static string _authToken;

    static async Task Main(string[] args)
    {
        Console.WriteLine("SignalR Test Client");
        Console.Write("Enter player Username: ");
        string username = Console.ReadLine();

        Console.Write("Enter player Password: ");
        string password = Console.ReadLine();
        await Authenticate(username, password);

        var connection = new HubConnectionBuilder()
            .WithUrl(_signalRHubUrl, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(_authToken);
            })
            .WithAutomaticReconnect()
            .Build();

        connection.On<string, decimal>("ReceivePrizeNotification", (message, amount) =>
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nCONGRATULATIONS!!! PRIZE NOTIFICATION: {message} - Amount: ${amount}");
            Console.ResetColor();
        });

        try
        {
            await connection.StartAsync();
            Console.WriteLine("Connected to SignalR hub!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error connecting to hub: {ex.Message}");
        }

        await MenuLoop();
    }

    private static async Task Authenticate(string username, string password)
    {
        var loginData = new { Username = username, Password = password };
        var content = new StringContent(JsonConvert.SerializeObject(loginData), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{_hubServiceUrl}/api/auth/login", content);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Authentication failed: {response.StatusCode}");
        }

        var responseJson = await response.Content.ReadAsStringAsync();
        var authResult = JsonConvert.DeserializeObject<AuthResponse>(responseJson);

        _authToken = authResult.Token;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
    }

    private static async Task MenuLoop()
    {
        while (true)
        {
            Console.WriteLine("\nChoose an action:");
            Console.WriteLine("1. Play a game round");
            Console.WriteLine("2. View leaderboard");
            Console.WriteLine("3. Exit");
            Console.Write("Enter choice: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    await PlayGameRound();
                    break;
                case "2":
                    await ViewLeaderboard();
                    break;
                case "3":
                    Console.WriteLine("Exiting...");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    break;
            }
        }
    }

    private static async Task PlayGameRound()
    {
        var response = await _httpClient.PostAsync($"{_gameServiceUrl}/api/game/play?betAmount={new Random().Next(10, 100)}", null);

        if (response.IsSuccessStatusCode)
        {
            var result = JsonConvert.DeserializeObject<PlayResponse>(await response.Content.ReadAsStringAsync());
            if (result.IsWin)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"Game played: Bet ${result.BetAmount} - WIN! Amount: ${result.WinAmount}");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Game played: Bet ${result.BetAmount} - LOSS");
                Console.ResetColor();
            }
        }
        else
        {
            Console.WriteLine($"❌ Failed to play game round: {response.StatusCode}");
        }
    }

    private static async Task ViewLeaderboard()
    {
        var response = await _httpClient.GetAsync($"{_leaderboardServiceUrl}/api/leaderboard/current");

        if (response.IsSuccessStatusCode)
        {
            var leaderboard = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IEnumerable<LeaderboardResponse>>(leaderboard).ToList();

            for (int i = 0; i < result.Count(); i++)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine($"Rank - {result[i].Rank}\tPlayerName - {result[i].PlayerName}\tTotalBets - {result[i].TotalBets}");
                Console.ResetColor();
            }
        }
        else
        {
            Console.WriteLine($"❌ Failed to fetch leaderboard: {response.StatusCode}");
        }
    }
}

public class AuthResponse
{
    public string Token { get; set; }
    public string PlayerId { get; set; }
}
public class PlayResponse
{
    public int BetAmount { get; set; }
    public bool IsWin { get; set; }
    public int WinAmount { get; set; }
}
public class LeaderboardResponse
{
    public Guid PlayerId { get; set; }
    public string PlayerName { get; set; }
    public int TotalBets { get; set; }
    public int Rank { get; set; }
}
