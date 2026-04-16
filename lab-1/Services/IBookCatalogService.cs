using Lab1.ViewModels;

namespace Lab1.Services;

public interface IBookCatalogService
{
    Task<BookDashboardViewModel> GetDashboardAsync();
}