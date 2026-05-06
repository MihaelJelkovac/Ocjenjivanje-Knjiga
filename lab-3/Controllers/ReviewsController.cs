using Lab3.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lab3.Controllers;

[Route("[controller]")]
[Route("recenzije")]
public class ReviewsController : Controller
{
    private readonly IReviewRepository _reviewRepository;

    public ReviewsController(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
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
}
