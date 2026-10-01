using System.Reflection;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Shared.SharedKernel;
using Shared.SharedKernel.Errors;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace Shared.Framework.Endpoints;

/// <summary>
/// IResult для <see cref="UnitResult{Error}"/>: успешный Envelope или ответ с ошибкой и нужным статусом.
/// </summary>
public sealed class EndpointResult(UnitResult<Error> result) : IResult, IEndpointMetadataProvider
{
    private readonly IResult _result = result.IsSuccess
        ? new SuccessResult()
        : new ErrorResult(result.Error);

    public Task ExecuteAsync(HttpContext httpContext) =>
        _result.ExecuteAsync(httpContext);

    public static implicit operator EndpointResult(UnitResult<Error> result) => new(result);

    static void IEndpointMetadataProvider.PopulateMetadata(MethodInfo method, EndpointBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(method);
        ArgumentNullException.ThrowIfNull(builder);

        builder.Metadata.Add(new ProducesResponseTypeMetadata(200, typeof(Envelope), ["application/json"]));

        builder.Metadata.Add(new ProducesResponseTypeMetadata(400, typeof(Envelope), ["application/json"]));
        builder.Metadata.Add(new ProducesResponseTypeMetadata(500, typeof(Envelope), ["application/json"]));
    }

    public static EndpointResult ToEndpointResult(UnitResult<Error> result) => new(result);
}

/// <summary>
/// IResult для <see cref="Result{TValue, Error}"/> или значения: успешный Envelope&lt;T&gt; либо ответ с ошибкой.
/// </summary>
public sealed class EndpointResult<TValue> : IResult, IEndpointMetadataProvider
{
    private readonly IResult _result;

    public EndpointResult(Result<TValue, Error> result)
    {
        _result = result.IsSuccess
            ? new SuccessResult<TValue>(result.Value)
            : new ErrorResult(result.Error);
    }

    public EndpointResult(TValue? result)
    {
        _result = new SuccessResult<TValue?>(result);
    }

    public Task ExecuteAsync(HttpContext httpContext) =>
        _result.ExecuteAsync(httpContext);

    public static implicit operator EndpointResult<TValue>(Result<TValue, Error> result) => new(result);

    public static implicit operator EndpointResult<TValue>(TValue? result) => new(result);

    static void IEndpointMetadataProvider.PopulateMetadata(MethodInfo method, EndpointBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(method);
        ArgumentNullException.ThrowIfNull(builder);

        builder.Metadata.Add(new ProducesResponseTypeMetadata(200, typeof(Envelope<TValue>), ["application/json"]));

        builder.Metadata.Add(new ProducesResponseTypeMetadata(400, typeof(Envelope), ["application/json"]));
        builder.Metadata.Add(new ProducesResponseTypeMetadata(500, typeof(Envelope), ["application/json"]));
    }

    public EndpointResult<TValue> ToEndpointResult(Result<TValue, Error> result) => new(result);
}