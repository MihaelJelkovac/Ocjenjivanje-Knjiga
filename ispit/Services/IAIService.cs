namespace Lab5.Services;

public interface IAIService
{
    Task<ReviewData> ExtractReviewFromPromptAsync(string prompt);
    Task<BookData> ExtractBookFromPromptAsync(string prompt);
}

public class ReviewData
{
    public string BookTitle { get; set; } = string.Empty;
    public int Score { get; set; }
    public string Sentiment { get; set; } = "Neutral";
    public bool IsRecommended { get; set; }
    public string Comment { get; set; } = string.Empty;
}

public class BookData
{
    public string Title { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public int PageCount { get; set; }
    public string Language { get; set; } = "eng";
}
