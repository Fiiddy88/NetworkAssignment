using System.Net.Http.Json;
using System.Text.Json;

namespace NetworkAssignment;

public class ScoreboardApi
{
    private const string BaseUrl = "https://schoolgame-6eaf3-default-rtdb.europe-west1.firebasedatabase.app/";
    private const string ScoresPath = "scores.json";

    private readonly HttpClient _client;

    public ScoreboardApi()
    {
        _client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10)
        };
    }

    public async Task SubmitScore(ScoreEntry entry)
    {
        var response = await _client.PostAsJsonAsync(BaseUrl + ScoresPath, entry);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<ScoreEntry>> GetScoreboard()
    {
        var response = await _client.GetAsync(BaseUrl + ScoresPath);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        
        if (json == "null")
        {
            return new List<ScoreEntry>();
        }

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var scoresDict = JsonSerializer.Deserialize<Dictionary<string, ScoreEntry>>(json, options);
        return scoresDict?.Values.ToList() ?? new List<ScoreEntry>();
    }
}