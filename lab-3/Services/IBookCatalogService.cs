using Lab3.ViewModels;

namespace Lab3.Services;

public interface IBookCatalogService
{
    Task<BookDashboardViewModel> GetDashboardAsync();
}
