using Lab4.Models;
using Lab4.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lab4.Controllers;

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

    [Route("")]
    [Route("index")]
    public async Task<IActionResult> Index()
    {
        var reviews = await _reviewRepository.GetAllAsync();
        return View(reviews);
    }

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

    [HttpGet]
    [Route("create")]
    public async Task<IActionResult> Create()
    {
        await PopulateDropdownsAsync();
        return View();
    }

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
}
