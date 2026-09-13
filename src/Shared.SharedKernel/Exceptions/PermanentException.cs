using Shared.SharedKernel.Errors;

namespace Shared.SharedKernel.Exceptions;

public class PermanentException : Exception
{
    public Error Error { get; } = null!;

    public PermanentException(Error error)
        : base(error.GetMessage())
    {
        Error = error;
    }

    public PermanentException()
    {
    }

    public PermanentException(string message)
        : base(message)
    {
    }

    public PermanentException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}