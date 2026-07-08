using System.Text.Json;

namespace Lab5.Services;

public class AIService : IAIService
{
    private readonly string _apiKey;
    private readonly ILogger<AIService> _logger;

    public AIService(string apiKey, ILogger<AIService> logger)
    {
        _apiKey = apiKey;
        _logger = logger;
    }

    /// <summary>
    /// Ekstrahira podatke recenzije iz teksta upita.
    /// Za sada koristi dummy implementaciju - trebat će Claude API key
    /// </summary>
    public async Task<ReviewData> ExtractReviewFromPromptAsync(string prompt)
    {
        try
        {
            _logger.LogInformation("🤖 AI: Parsiranje promptja: {Prompt}", prompt);

            // Dummy implementacija - parsira očite znakove iz teksta
            var reviewData = ParseReviewFromText(prompt);

            _logger.LogInformation("✅ AI: Uspješno parsirano - {BookTitle}, Ocjena: {Score}",
                reviewData.BookTitle, reviewData.Score);

            return await Task.FromResult(reviewData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ AI Greška pri parsiranju");
            throw;
        }
    }

    /// <summary>
    /// Ekstrahira podatke knjige iz teksta upita.
    /// Za sada koristi dummy implementaciju - trebat će Claude API key
    /// </summary>
    public async Task<BookData> ExtractBookFromPromptAsync(string prompt)
    {
        try
        {
            _logger.LogInformation("🤖 AI: Parsiranje knjige iz promptja: {Prompt}", prompt);

            var bookData = ParseBookFromText(prompt);

            _logger.LogInformation("✅ AI: Knjiga parsirana - {Title}, Autor: {Author}",
                bookData.Title, bookData.AuthorName);

            return await Task.FromResult(bookData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ AI Greška pri parsiranju knjige");
            throw;
        }
    }

    /// <summary>
    /// Dummy parser - izvlači informacije iz teksta na osnovu ključnih riječi
    /// NAPOMENA: Ovo će biti zamijenjeno s Claude API-jem kada se vrati API key
    /// </summary>
    private ReviewData ParseReviewFromText(string text)
    {
        var data = new ReviewData();

        text = text.ToLower();

        // Ekstrahiraj naslov knjige - očekuje format: "Knjiga: XYZ" ili samo prvi dio
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

        // Ekstrahiraj ocjenu (1-5)
        data.Score = ExtractScore(text);

        // Ekstrahiraj sentiment
        data.Sentiment = ExtractSentiment(text);

        // Preporuka
        data.IsRecommended = text.Contains("preporuka") ||
                           text.Contains("preporučujem") ||
                           text.Contains("odličan") ||
                           text.Contains("fantastičan") ||
                           data.Score >= 4;

        // Komentar - uzmi prvi dio teksta
        data.Comment = text.Length > 200 ? text.Substring(0, 197) + "..." : text;

        return data;
    }

    /// <summary>
    /// Dummy parser za knjige
    /// </summary>
    private BookData ParseBookFromText(string text)
    {
        var data = new BookData();

        text = text.ToLower();

        // Ekstrahiraj naslov
        data.Title = ExtractFirstNoun(text);

        // Ekstrahiraj autora
        if (text.Contains("autor:") || text.Contains("od "))
        {
            var parts = text.Split(" od ");
            if (parts.Length > 1)
            {
                data.AuthorName = ExtractAuthorName(parts[1]);
            }
        }

        // Ekstrahiraj ISBN
        data.ISBN = ExtractISBN(text) ?? "000-0000000000";

        // Ekstrahiraj broj stranica
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
        // Traži brojeve 1-5
        foreach (var match in System.Text.RegularExpressions.Regex.Matches(text, @"\b[1-5]\b").Cast<System.Text.RegularExpressions.Match>())
        {
            if (int.TryParse(match.Value, out int score))
                return score;
        }

        // Infer iz sentimenta
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

        return 3; // Default
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
        return 300; // Default
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
