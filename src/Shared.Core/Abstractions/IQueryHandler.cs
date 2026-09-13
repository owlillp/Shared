using CSharpFunctionalExtensions;
using Shared.SharedKernel.Errors;

namespace Shared.Core.Abstractions;

public interface IQueryHandler<TResponse, in TQuery>
    where TQuery : IQuery
{
    public Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken = default);
}

public interface IQueryHandlerWithResult<TResponse, in TQuery>
    where TQuery : IQuery
{
    public Task<Result<TResponse, Error>> Handle(TQuery query, CancellationToken cancellationToken = default);
}