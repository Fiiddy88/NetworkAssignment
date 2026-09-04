using System.Text.Json;
using NetworkAssignment;

var api = new ScoreboardApi();
var rng = new Random();
bool running = true;

while (running)
{
    Console.WriteLine();
    Console.WriteLine("=== ONLINE SCOREBOARD GAME ===");
    Console.WriteLine();
    Console.WriteLine("1. Play Game");
    Console.WriteLine("2. View Scoreboard");
    Console.WriteLine("3. Personal Best");
    Console.WriteLine("4. Exit");
    Console.WriteLine();
    Console.Write("Choose: ");

    string? choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            await PlayGame();
            break;
        case "2":
            await ViewScoreboard();
            break;
        case "3":
            await PersonalBest();
            break;
        case "4":
            running = false;
            break;
        default:
            Console.WriteLine("Invalid choice, try again.");
            break;
    }
}

async Task PlayGame()
{
    Console.WriteLine();
    string name = ReadValidName();

    int score = 0;
    const int rounds = 5;

    Console.WriteLine();
    Console.WriteLine($"Guess a number between 1 and 3. You have {rounds} tries.");

    for (int round = 1; round <= rounds; round++)
    {
        int answer = rng.Next(1, 4); // 1, 2, or 3
        int guess = ReadValidGuess(round);

        if (guess == answer)
        {
            score += 10;
            Console.WriteLine($"Correct! The number was {answer}. (+10 points)");
        }
        else
        {
            Console.WriteLine($"Wrong. The number was {answer}.");
        }
    }

    Console.WriteLine();
    Console.WriteLine($"Game over! Final score: {score}");
    Console.WriteLine();
    Console.WriteLine("Submitting...");

    try
    {
        await api.SubmitScore(new ScoreEntry { Name = name, Score = score });
        Console.WriteLine("Score submitted!");
    }
    catch (HttpRequestException)
    {
        Console.WriteLine("Could not connect to the server.");
    }
    catch (TaskCanceledException)
    {
        Console.WriteLine("The request timed out. Please try again.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Something went wrong: {ex.Message}");
    }
}

async Task ViewScoreboard()
{
    Console.WriteLine();
    Console.WriteLine("Loading scoreboard...");

    try
    {
        var scores = await api.GetScoreboard();
        var top = scores.OrderByDescending(s => s.Score).Take(10).ToList();

        Console.WriteLine();
        Console.WriteLine("=== LEADERBOARD (TOP 10) ===");
        Console.WriteLine();

        if (top.Count == 0)
        {
            Console.WriteLine("No scores yet.");
            return;
        }

        for (int i = 0; i < top.Count; i++)
        {
            Console.WriteLine($"{i + 1,2}. {top[i].Name,-12} {top[i].Score}");
        }
    }
    catch (HttpRequestException)
    {
        Console.WriteLine("Could not connect to the server.");
    }
    catch (TaskCanceledException)
    {
        Console.WriteLine("The request timed out. Please try again.");
    }
    catch (JsonException)
    {
        Console.WriteLine("Received unexpected data from the server.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Something went wrong: {ex.Message}");
    }
}

async Task PersonalBest()
{
    Console.WriteLine();
    string name = ReadValidName();

    Console.WriteLine();
    Console.WriteLine("Loading scoreboard...");

    try
    {
        var scores = await api.GetScoreboard();
        var personalScores = scores
            .Where(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase))
            .ToList();

        Console.WriteLine();

        if (personalScores.Count == 0)
        {
            Console.WriteLine($"No scores found for {name}.");
            return;
        }

        int best = personalScores.Max(s => s.Score);
        Console.WriteLine($"{name}'s personal best: {best}");
    }
    catch (HttpRequestException)
    {
        Console.WriteLine("Could not connect to the server.");
    }
    catch (TaskCanceledException)
    {
        Console.WriteLine("The request timed out. Please try again.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Something went wrong: {ex.Message}");
    }
}

string ReadValidName()
{
    while (true)
    {
        Console.Write("Name: ");
        string? input = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(input))
        {
            return input.Trim();
        }

        Console.WriteLine("Name cannot be blank. Try again.");
    }
}

int ReadValidGuess(int round)
{
    while (true)
    {
        Console.Write($"Round {round} - Guess (1-3): ");
        string? input = Console.ReadLine();

        if (int.TryParse(input, out int guess) && guess is >= 1 and <= 3)
        {
            return guess;
        }

        Console.WriteLine("Please enter 1, 2, or 3.");
    }
}