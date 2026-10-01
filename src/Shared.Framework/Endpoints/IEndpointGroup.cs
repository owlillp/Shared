using Microsoft.AspNetCore.Routing;

namespace Shared.Framework.Endpoints;

public interface IEndpointGroup
{
    RouteGroupBuilder MapGroup(IEndpointRouteBuilder app);
}