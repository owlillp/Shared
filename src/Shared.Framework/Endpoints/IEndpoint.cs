using Microsoft.AspNetCore.Routing;

namespace Shared.Framework.Endpoints;

/// <summary>
/// Контракт endpoint-а: описывает, как зарегистрировать свои маршруты.
/// </summary>
public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}