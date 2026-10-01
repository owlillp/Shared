using CSharpFunctionalExtensions;
using Shared.SharedKernel.Errors;

namespace Shared.Core.Abstractions;

/// <summary>
/// Маркерный интерфейс команды — операции, изменяющей состояние.
/// </summary>
public interface ICommand;

/// <summary>
/// Обработчик команды, возвращающий результат или доменную ошибку.
/// </summary>
public interface ICommandHandler<TResponse, in TCommand>
    where TCommand : ICommand
{
    Task<Result<TResponse, Error>> Handle(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Обработчик команды без полезного результата (только успех или ошибка).
/// </summary>
public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task<UnitResult<Error>> Handle(TCommand command, CancellationToken cancellationToken = default);
}