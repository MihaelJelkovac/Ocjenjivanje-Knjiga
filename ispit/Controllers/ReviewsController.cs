using Lab5.Models;
using Lab5.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lab5.Controllers;

[Route("[controller]")]
[Route("recenzije")]
public class ReviewsController : Controller
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAIService _aiService;
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<ReviewsController> _logger;

    public ReviewsController(
        IReviewRepository reviewRepository,
        IBookRepository bookRepository,
        IUserRepository userRepository,
        IAIService aiService,
        UserManager<AppUser> userManager,
        ILogger<ReviewsController> logger)
    {
        _reviewRepository = reviewRepository;
        _bookRepository = bookRepository;
        _userRepository = userRepository;
        _aiService = aiService;
        _userManager = userManager;
        _logger = logger;
    }

    [AllowAnonymous]
    [Route("")]
    [Route("index")]
    public async Task<IActionResult> Index()
    {
        var reviews = await _reviewRepository.GetAllAsync();
        return View(reviews);
    }

    [AllowAnonymous]
    [Route("{id:int}")]
    [Route("prikaz/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var review = await _reviewRepository.GetByIdAsync(id);
        if (review is null)
        {
            return NotFound();
        }

        return View(review);
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpGet]
    [Route("create")]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View();
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Create(Review model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync();
            return View(model);
        }

        try
        {
            await _reviewRepository.CreateAsync(model);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "Greška pri spremanju: " + ex.Message);
            await PopulateDropdownsAsync();
            return View(model);
        }
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpGet]
    [Route("edit/{id:int}")]
    [ActionName("Edit")]
    public async Task<IActionResult> EditGet(int id)
    {
        var review = await _reviewRepository.GetByIdAsync(id);
        if (review == null)
        {
            return NotFound();
        }

        await PopulateDropdownsAsync(review);
        return View("Edit", review);
    }

    [Authorize(Roles = "Admin,Manager")]
    [HttpPost]
    [Route("edit/{id:int}")]
    [ActionName("Edit")]
    public async Task<IActionResult> EditPost(int id)
    {
        var review = await _reviewRepository.GetByIdAsync(id);
        if (review == null)
        {
            return NotFound();
        }

        var updateOk = await TryUpdateModelAsync(review);

        if (!updateOk || !ModelState.IsValid)
        {
            await PopulateDropdownsAsync(review);
            return View("Edit", review);
        }

        try
        {
            var success = await _reviewRepository.UpdateAsync(review);
            if (!success)
            {
                throw new Exception("Neuspješno ažuriranje");
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "Greška pri ažuriranju: " + ex.Message);
            await PopulateDropdownsAsync(review);
            return View("Edit", review);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [Route("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var success = await _reviewRepository.DeleteAsync(id);

            if (!success)
            {
                return Json(new { success = false, message = "Recenzija nije pronađena" });
            }

            return Json(new { success = true, message = "Recenzija je uspješno obrisana" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Greška pri brisanju: " + ex.Message });
        }
    }

    private async Task PopulateDropdownsAsync(Review? review = null)
    {
        var books = await _bookRepository.GetAllAsync();
        ViewBag.Books = books.Select(b => new SelectListItem
        {
            Value = b.Id.ToString(),
            Text = b.Title,
            Selected = review != null && review.BookId == b.Id
        }).ToList();

        var users = await _userRepository.GetAllAsync();
        ViewBag.Users = users.Select(u => new SelectListItem
        {
            Value = u.Id.ToString(),
            Text = u.FullName,
            Selected = review != null && review.UserId == u.Id
        }).ToList();

        ViewBag.Sentiments = Enum.GetValues(typeof(ReviewSentiment))
            .Cast<ReviewSentiment>()
            .Select(s => new SelectListItem
            {
                Value = s.ToString(),
                Text = s.ToString(),
                Selected = review != null && review.Sentiment == s
            })
            .ToList();
    }

    [AllowAnonymous]
    [Route("search")]
    public async Task<IActionResult> Search(string query)
    {
        var results = await _reviewRepository.GetAllAsync();
        return Json(results
            .Where(r =>
                r.Title.Contains(query ?? string.Empty, StringComparison.OrdinalIgnoreCase) ||
                (r.Book != null && r.Book.Title.Contains(query ?? string.Empty, StringComparison.OrdinalIgnoreCase)) ||
                (r.User != null && r.User.FullName.Contains(query ?? string.Empty, StringComparison.OrdinalIgnoreCase)))
            .Take(50)
            .Select(r => new { id = r.Id, text = r.Title }));
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("create-from-ai")]
    public IActionResult CreateFromAI()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("create-from-ai")]
    public async Task<IActionResult> CreateFromAIPost([FromBody] AIPromptRequest request)
    {
        try
        {
            _logger.LogInformation("🤖 Primljen AI prompt: {Prompt}", request.Prompt);

            if (string.IsNullOrWhiteSpace(request.Prompt))
                return BadRequest(new { error = "Prompt ne može biti prazan" });

            // 1. AI parsira prompt
            var reviewData = await _aiService.ExtractReviewFromPromptAsync(request.Prompt);

            // 2. Pronađi knjiga
            var books = await _bookRepository.GetAllAsync();
            var book = books.FirstOrDefault(b =>
                b.Title.Contains(reviewData.BookTitle, StringComparison.OrdinalIgnoreCase));

            if (book == null)
            {
                _logger.LogWarning("⚠️ Knjiga nije pronađena: {BookTitle}", reviewData.BookTitle);
                return BadRequest(new
                {
                    error = $"Knjiga '{reviewData.BookTitle}' nije pronađena",
                    suggestion = "Dostupne knjige: Harry Potter, A Game of Thrones, The Fellowship of the Ring"
                });
            }

            // 3. Pronađi korisnika ili koristi default-a
            var users = await _userRepository.GetAllAsync();
            var defaultUser = users.FirstOrDefault();
            int userId = defaultUser?.Id ?? 1;

            if (defaultUser == null)
            {
                _logger.LogWarning("⚠️ Nema dostupnih korisnika za recenziju");
                return BadRequest(new { error = "Trebate biti ulogirani ili korisnik mora postojati" });
            }

            // 4. Kreiraj recenziju
            var review = new Review
            {
                BookId = book.Id,
                UserId = userId,
                Score = reviewData.Score,
                Comment = reviewData.Comment,
                IsRecommended = reviewData.IsRecommended,
                Title = reviewData.Comment.Length > 50
                    ? reviewData.Comment.Substring(0, 47) + "..."
                    : reviewData.Comment,
                Sentiment = Enum.Parse<ReviewSentiment>(reviewData.Sentiment, ignoreCase: true),
                ReviewedAt = DateTime.UtcNow
            };

            await _reviewRepository.CreateAsync(review);

            _logger.LogInformation("✅ Recenzija kreirana kroz AI: {BookTitle}, Score: {Score}",
                book.Title, reviewData.Score);

            return Ok(new
            {
                success = true,
                reviewId = review.Id,
                message = $"✅ Recenzija kreirana za '{book.Title}'",
                review = new
                {
                    bookTitle = book.Title,
                    score = review.Score,
                    comment = review.Comment,
                    sentiment = review.Sentiment.ToString()
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Greška pri AI kreiranju recenzije");
            return BadRequest(new { error = ex.Message });
        }
    }
}

public class AIPromptRequest
{
    public string Prompt { get; set; } = string.Empty;
}

