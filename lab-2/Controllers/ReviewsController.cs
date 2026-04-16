using Lab2.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.Controllers;

public class ReviewsController : Controller
{
    private readonly IReviewRepository _reviewRepository;

    public ReviewsController(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<IActionResult> Index()
    {
        var reviews = await _reviewRepository.GetAllAsync();
        return View(reviews);
    }

    public async Task<IActionResult> Details(int id)
    {
        var review = await _reviewRepository.GetByIdAsync(id);
        if (review is null)
        {
            return NotFound();
        }

        return View(review);
    }
}
