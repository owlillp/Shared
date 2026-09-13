using System.Reflection;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Shared.SharedKernel.Abstractions;

namespace Shared.Core.Database;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDapperJson(this IServiceCollection services, params Assembly[] assemblies)
    {
        foreach (Assembly assembly in assemblies)
        {
            var jsonTypes = assembly.GetTypes()
                .Where(t => t.IsClass && typeof(IDapperJson).IsAssignableFrom(t))
                .ToList();

            foreach (Type type in jsonTypes)
            {
                Type handlerType = typeof(JsonTypeHandler<>).MakeGenericType(type);
                object? handler = Activator.CreateInstance(handlerType);
                SqlMapper.AddTypeHandler(type, handler as SqlMapper.ITypeHandler ?? throw new InvalidOperationException());
            }
        }

        return services;
    }
}