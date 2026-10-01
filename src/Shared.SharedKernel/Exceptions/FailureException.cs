using Shared.SharedKernel.Errors;

namespace Shared.SharedKernel.Exceptions;

/// <summary>
/// Исключение внутренней ошибки (HTTP 500), оборачивающее доменный <see cref="Error"/>.
/// </summary>
public class FailureException : Exception
{
    public Error Error { get; } = null!;

    public FailureException(Error error)
        : base(error.GetMessage())
    {
        Error = error;
    }

    public FailureException()
    {
    }

    public FailureException(string message)
        : base(message)
    {
    }

    public FailureException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}