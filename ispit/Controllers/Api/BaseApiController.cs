using Microsoft.AspNetCore.Mvc;

namespace Lab5.Controllers.Api;

/// <summary>
/// Bazna klasa za sve API controllere - centralizira zajedničku logiku
/// </summary>
public abstract class BaseApiController : ControllerBase
{
    protected readonly ILogger Logger;

    protected BaseApiController(ILogger logger)
    {
        Logger = logger;
    }

    /// <summary>
    /// Normalizira i primjenjuje query string filter na kolekciju
    /// </summary>
    protected IEnumerable<T> ApplyQueryFilter<T>(
        IEnumerable<T> items,
        string? query,
        Func<T, string[]> fieldsSelector) where T : class
    {
        if (string.IsNullOrWhiteSpace(query))
            return items;

        var normalized = query.Trim();
        return items.Where(item =>
            fieldsSelector(item).Any(field =>
                field.Contains(normalized, StringComparison.OrdinalIgnoreCase)));
    }
}
