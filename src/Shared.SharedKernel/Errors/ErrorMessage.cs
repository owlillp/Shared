namespace Shared.SharedKernel.Errors;

/// <summary>
/// Сообщение ошибки: код, текст и опциональное имя поля.
/// </summary>
public record ErrorMessage(string Code, string Message, string? InvalidField = null);