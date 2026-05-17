using Lab4.ViewModels;

namespace Lab4.Services;

public interface IBookCatalogService
{
    Task<BookDashboardViewModel> GetDashboardAsync();
}
