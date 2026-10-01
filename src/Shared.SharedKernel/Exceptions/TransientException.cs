using Shared.SharedKernel.Errors;

namespace Shared.SharedKernel.Exceptions;

/// <summary>
/// Временная ошибка, допускающая повторную обработку.
/// </summary>
public class TransientException : Exception
{
    public Error Error { get; } = null!;

    public TransientException(Error error)
        : base(error.GetMessage())
    {
        Error = error;
    }

    public TransientException()
    {
    }

    public TransientException(string message)
        : base(message)
    {
    }

    public TransientException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}