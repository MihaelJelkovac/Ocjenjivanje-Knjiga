using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Lab5.Services;

public class AIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly ILogger<AIService> _logger;

    private const string Model = "mistral-small-latest";
    private const string ApiUrl = "https://api.mistral.ai/v1/chat/completions";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public AIService(string apiKey, ILogger<AIService> logger, HttpClient httpClient)
    {
        _apiKey = apiKey;
        _logger = logger;
        _httpClient = httpClient;
    }

    private bool HasValidApiKey => !string.IsNullOrWhiteSpace(_apiKey) &&
        _apiKey != "dummy-key-for-now" && !_apiKey.Contains("YOUR_") && _apiKey.Length > 20;

    public async Task<ReviewData> ExtractReviewFromPromptAsync(string prompt)
    {
        _logger.LogInformation("🤖 AI: Parsiranje promptja: {Prompt}", prompt);

        if (!HasValidApiKey)
        {
            _logger.LogWarning("⚠️ Mistral API key nije postavljen - koristim lokalni fallback parser");
            return ParseReviewFromText(prompt);
        }

        const string systemPrompt = """
            Ti si asistent koji parsira recenzije knjiga napisane na hrvatskom ili engleskom jeziku.
            Vrati ISKLJUČIVO validan JSON objekt (bez markdown code blocka, bez dodatnog teksta) u formatu:
            {"bookTitle": "naziv knjige", "score": broj_od_1_do_5, "sentiment": "Enthusiastic ili Positive ili Neutral ili Critical", "isRecommended": true ili false, "comment": "kratki komentar do 200 znakova"}
            """;

        try
        {
            var responseText = await CallMistralAsync(systemPrompt, prompt);
            var reviewData = JsonSerializer.Deserialize<ReviewData>(responseText, JsonOptions)
                ?? throw new InvalidOperationException("Mistral nije vratio valjani JSON");

            _logger.LogInformation("✅ AI: Uspješno parsirano - {BookTitle}, Ocjena: {Score}",
                reviewData.BookTitle, reviewData.Score);

            return reviewData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Greška pri pozivu Mistral API-ja, koristim lokalni fallback parser");
            return ParseReviewFromText(prompt);
        }
    }

    public async Task<BookData> ExtractBookFromPromptAsync(string prompt)
    {
        _logger.LogInformation("🤖 AI: Parsiranje knjige iz promptja: {Prompt}", prompt);

        if (!HasValidApiKey)
        {
            _logger.LogWarning("⚠️ Mistral API key nije postavljen - koristim lokalni fallback parser");
            return ParseBookFromText(prompt);
        }

        const string systemPrompt = """
            Ti si asistent koji parsira podatke o knjizi iz prirodnog jezika (hrvatski ili engleski).
            Vrati ISKLJUČIVO validan JSON objekt (bez markdown code blocka, bez dodatnog teksta) u formatu:
            {"title": "naslov knjige", "authorName": "ime i prezime autora", "isbn": "ISBN ili 000-0000000000 ako nije naveden", "pageCount": broj_stranica, "language": "eng ili hrv"}
            """;

        try
        {
            var responseText = await CallMistralAsync(systemPrompt, prompt);
            var bookData = JsonSerializer.Deserialize<BookData>(responseText, JsonOptions)
                ?? throw new InvalidOperationException("Mistral nije vratio valjani JSON");

            _logger.LogInformation("✅ AI: Knjiga parsirana - {Title}, Autor: {Author}",
                bookData.Title, bookData.AuthorName);

            return bookData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Greška pri pozivu Mistral API-ja, koristim lokalni fallback parser");
            return ParseBookFromText(prompt);
        }
    }

    private async Task<string> CallMistralAsync(string systemPrompt, string userPrompt)
    {
        var requestBody = new
        {
            model = Model,
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userPrompt }
            },
            response_format = new { type = "json_object" }
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, ApiUrl);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        var responseJson = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("❌ Mistral API vratio {StatusCode}: {Body}", response.StatusCode, responseJson);
            response.EnsureSuccessStatusCode();
        }

        _logger.LogInformation("🤖 Mistral sirovi odgovor: {Response}", responseJson);

        using var doc = JsonDocument.Parse(responseJson);
        var text = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? "{}";

        return text.Replace("```json", "").Replace("```", "").Trim();
    }

    /// <summary>
    /// Fallback parser koji se koristi kada Mistral API nije dostupan (nema key-a ili poziv je pao).
    /// Izvlači informacije iz teksta na osnovu ključnih riječi.
    /// </summary>
    private ReviewData ParseReviewFromText(string text)
    {
        var data = new ReviewData();

        text = text.ToLower();

        if (text.Contains("za "))
        {
            var parts = text.Split(" za ");
            if (parts.Length > 1)
            {
                data.BookTitle = ExtractBookName(parts[1]);
            }
        }
        else
        {
            data.BookTitle = ExtractFirstNoun(text);
        }

        data.Score = ExtractScore(text);
        data.Sentiment = ExtractSentiment(text);

        data.IsRecommended = text.Contains("preporuka") ||
                           text.Contains("preporučujem") ||
                           text.Contains("odličan") ||
                           text.Contains("fantastičan") ||
                           data.Score >= 4;

        data.Comment = text.Length > 200 ? text.Substring(0, 197) + "..." : text;

        return data;
    }

    private BookData ParseBookFromText(string text)
    {
        var data = new BookData();

        text = text.ToLower();

        data.Title = ExtractFirstNoun(text);

        if (text.Contains("autor:") || text.Contains("od "))
        {
            var parts = text.Split(" od ");
            if (parts.Length > 1)
            {
                data.AuthorName = ExtractAuthorName(parts[1]);
            }
        }

        data.ISBN = ExtractISBN(text) ?? "000-0000000000";
        data.PageCount = ExtractPageCount(text);
        data.Language = "eng";

        return data;
    }

    private string ExtractBookName(string text)
    {
        var words = text.Split(' ');
        var name = string.Join(" ", words.Take(Math.Min(4, words.Length)));
        return name.Trim(',', '.', '!', '?').ToTitleCase();
    }

    private string ExtractFirstNoun(string text)
    {
        var words = text.Split(new[] { " ", ",", ".", "!", "?" }, StringSplitOptions.RemoveEmptyEntries);
        var noun = words.FirstOrDefault(w => w.Length > 2 && !IsCommonWord(w)) ?? words.FirstOrDefault() ?? "Unknown";
        return noun.ToTitleCase();
    }

    private string ExtractAuthorName(string text)
    {
        var parts = text.Split(new[] { " ", ",", "." }, StringSplitOptions.RemoveEmptyEntries);
        return string.Join(" ", parts.Take(2)).ToTitleCase();
    }

    private string? ExtractISBN(string text)
    {
        var pattern = @"\d{3}-\d{10}|\d{10}";
        var match = System.Text.RegularExpressions.Regex.Match(text, pattern);
        return match.Success ? match.Value : null;
    }

    private int ExtractScore(string text)
    {
        foreach (var match in System.Text.RegularExpressions.Regex.Matches(text, @"\b[1-5]\b").Cast<System.Text.RegularExpressions.Match>())
        {
            if (int.TryParse(match.Value, out int score))
                return score;
        }

        if (text.Contains("odličan") || text.Contains("fantastičan") || text.Contains("super"))
            return 5;
        if (text.Contains("dobar") || text.Contains("solidan"))
            return 4;
        if (text.Contains("ok") || text.Contains("zadovoljava"))
            return 3;
        if (text.Contains("loš") || text.Contains("razočaran"))
            return 2;
        if (text.Contains("teribilan") || text.Contains("grozno"))
            return 1;

        return 3;
    }

    private string ExtractSentiment(string text)
    {
        if (text.Contains("odličan") || text.Contains("fantastičan") ||
            text.Contains("super") || text.Contains("amazing"))
            return "Enthusiastic";

        if (text.Contains("dobar") || text.Contains("sviđa") || text.Contains("solidan"))
            return "Positive";

        if (text.Contains("loš") || text.Contains("razočaran") || text.Contains("nema"))
            return "Critical";

        return "Neutral";
    }

    private int ExtractPageCount(string text)
    {
        var matches = System.Text.RegularExpressions.Regex.Matches(text, @"\b\d{2,4}\b");
        foreach (var match in matches.Cast<System.Text.RegularExpressions.Match>())
        {
            if (int.TryParse(match.Value, out int pages) && pages > 50 && pages < 10000)
                return pages;
        }
        return 300;
    }

    private bool IsCommonWord(string word)
    {
        var common = new[] { "the", "a", "an", "and", "or", "but", "in", "on", "at", "je", "je", "je" };
        return common.Contains(word);
    }
}

public static class StringExtensions
{
    public static string ToTitleCase(this string text)
    {
        return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());
    }
}
