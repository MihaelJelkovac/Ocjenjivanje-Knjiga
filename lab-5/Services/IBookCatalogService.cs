using Lab5.ViewModels;

namespace Lab5.Services;

public interface IBookCatalogService
{
    Task<BookDashboardViewModel> GetDashboardAsync();
}

