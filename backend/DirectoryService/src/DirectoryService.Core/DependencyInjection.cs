using DirectoryService.Core.Departments;
using DirectoryService.Core.Locations;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Core;

/// <summary>
/// Регистрация сервисов слоя Core в DI-контейнере.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<CreateLocationHandler>();
        services.AddScoped<UpdateLocationHandler>();
        services.AddScoped<CreateDepartmentHandler>();
        services.AddScoped<UpdateDepartmentHandler>();
        services.AddScoped<AttachLocationHandler>();
        services.AddScoped<DetachLocationHandler>();

        return services;
    }
}
