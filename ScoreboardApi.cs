using System.Net.Http.Json;
using System.Text.Json;

namespace NetworkAssignment;

public class ScoreboardApi
{
    private const string SubmitUrl = "https://hooks.zapier.com/hooks/catch/8338993/ujs9jj9/";
    private const string ScoreboardUrl = "https://script.google.com/macros/s/AKfycbys5aEPMvNCutyhNYYCcQcCjzsi2UtqNspmKyCH-AicJxJbCJMrAoT0LUaYaXhTWA8n/exec";

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
        var response = await _client.PostAsJsonAsync(SubmitUrl, entry);
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<ScoreEntry>> GetScoreboard()
    {
        var response = await _client.GetAsync(ScoreboardUrl);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var scores = JsonSerializer.Deserialize<List<ScoreEntry>>(json, options);
        return scores ?? new List<ScoreEntry>();
    }
}