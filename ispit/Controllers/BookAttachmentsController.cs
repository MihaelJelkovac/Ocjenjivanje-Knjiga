using Lab5.Data;
using Lab5.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lab5.Controllers;

[Route("Books/Attachments")]
public class BookAttachmentsController : Controller
{
    private readonly CatalogDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public BookAttachmentsController(CatalogDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [HttpGet("{bookId:int}")]
    public async Task<IActionResult> List(int bookId)
    {
        var attachments = await _context.Attachments
            .Where(a => a.BookId == bookId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        return PartialView("~/Views/Books/_AttachmentList.cshtml", attachments);
    }

    [HttpPost("Upload/{bookId:int}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Upload(int bookId, IFormFile file)
    {
        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId && b.DeletedAt == null);
        if (book is null)
        {
            return NotFound();
        }

        if (file is null || file.Length == 0)
        {
            return BadRequest(new { message = "Datoteka nije poslana." });
        }

        var uploadsRoot = Path.Combine(_environment.WebRootPath, "uploads", "books", bookId.ToString());
        Directory.CreateDirectory(uploadsRoot);

        var storedFileName = $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}";
        var physicalPath = Path.Combine(uploadsRoot, storedFileName);
        await using (var stream = System.IO.File.Create(physicalPath))
        {
            await file.CopyToAsync(stream);
        }

        var attachment = new Attachment
        {
            BookId = bookId,
            FileName = file.FileName,
            FilePath = $"/uploads/books/{bookId}/{storedFileName}",
            ContentType = file.ContentType ?? string.Empty,
            FileSize = file.Length,
            CreatedAt = DateTime.UtcNow
        };

        _context.Attachments.Add(attachment);
        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }

    [HttpPost("Delete/{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var attachment = await _context.Attachments.FirstOrDefaultAsync(a => a.Id == id);
        if (attachment is null)
        {
            return NotFound();
        }

        var physicalPath = Path.Combine(_environment.WebRootPath, attachment.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (System.IO.File.Exists(physicalPath))
        {
            System.IO.File.Delete(physicalPath);
        }

        _context.Attachments.Remove(attachment);
        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }
}