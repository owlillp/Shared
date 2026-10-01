using CSharpFunctionalExtensions;
using Shared.SharedKernel.Errors;

namespace Shared.Core.Abstractions;

/// <summary>
/// Маркерный интерфейс запроса — операции чтения, не изменяющей состояние.
/// </summary>
public interface IQuery;

/// <summary>
/// Обработчик запроса, возвращающий результат или доменную ошибку.
/// </summary>
public interface IQueryHandler<TResponse, in TQuery>
    where TQuery : IQuery
{
    Task<Result<TResponse, Error>> Handle(TQuery query, CancellationToken cancellationToken = default);
}
