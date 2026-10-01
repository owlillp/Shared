namespace Shared.SharedKernel.Responses;

/// <summary>
/// Постраничный ответ со списком элементов и метаданными страницы.
/// </summary>
public record PaginationResponse<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages);