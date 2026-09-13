using System.Data.Common;
using CSharpFunctionalExtensions;
using Shared.SharedKernel.Errors;

namespace Shared.Core.Database;

public interface ITransactionManager : IAsyncDisposable
{
    Task<UnitResult<Error>> BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task<UnitResult<Error>> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<UnitResult<Error>> CommitTransactionAsync(CancellationToken cancellationToken = default);

    DbConnection GetDbConnection();
}