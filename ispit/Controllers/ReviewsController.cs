using Lab5.Models;
using Lab5.Services;
using Microsoft.AspNetCore.Authorization;
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

    public ReviewsController(
        IReviewRepository reviewRepository,
        IBookRepository bookRepository,
        IUserRepository userRepository)
    {
        _reviewRepository = reviewRepository;
        _bookRepository = bookRepository;
        _userRepository = userRepository;
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
}

