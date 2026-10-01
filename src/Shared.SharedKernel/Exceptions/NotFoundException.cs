using Shared.SharedKernel.Errors;

namespace Shared.SharedKernel.Exceptions;

/// <summary>
/// Исключение отсутствия сущности (HTTP 404), оборачивающее доменный <see cref="Error"/>.
/// </summary>
public class NotFoundException : Exception
{
    public Error Error { get; } = null!;

    public NotFoundException(Error error)
        : base(error.GetMessage())
    {
        Error = error;
    }

    public NotFoundException()
    {
    }

    public NotFoundException(string message)
        : base(message)
    {
    }

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}