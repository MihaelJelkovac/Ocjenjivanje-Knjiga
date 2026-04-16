using Lab2.ViewModels;

namespace Lab2.Services;

public interface IBookCatalogService
{
    Task<BookDashboardViewModel> GetDashboardAsync();
}