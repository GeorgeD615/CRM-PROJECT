using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Behaviours;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Core;

/// <summary>
/// Регистрация сервисов слоя Core в DI-контейнере. Валидаторы и handler-ы находятся
/// сканированием сборки — добавление нового feature-среза не требует правки этого файла.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddValidatorsFromAssembly(assembly);

        services.Scan(scan => scan.FromAssemblies(assembly)
            .AddClasses(classes => classes.AssignableToAny(
                typeof(ICommandHandler<,>),
                typeof(ICommandHandler<>),
                typeof(IQueryHandler<,>)))
            .AsSelfWithInterfaces()
            .WithScopedLifetime());

        services.TryDecorate(typeof(ICommandHandler<,>), typeof(ValidationBehaviour<,>));
        services.TryDecorate(typeof(ICommandHandler<>), typeof(ValidationBehaviour<>));

        return services;
    }
}
