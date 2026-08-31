using DirectoryService.Contracts.Locations;
using DirectoryService.Core.Abstractions;
using DirectoryService.Core.Behaviours;
using DirectoryService.Core.Locations.GetTopLocations;
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

        // Топ локаций существует в двух реализациях сразу, поэтому выбор активной делается
        // вручную ниже, а из сканирования обе исключаются.
        Type[] manuallyRegisteredHandlers =
        [
            typeof(GetTopLocationsEfCoreHandler),
            typeof(GetTopLocationsDapperHandler),
        ];

        services.Scan(scan => scan.FromAssemblies(assembly)
            .AddClasses(classes => classes
                .AssignableToAny(
                    typeof(ICommandHandler<,>),
                    typeof(ICommandHandler<>),
                    typeof(IQueryHandler<,>))
                .Where(type => !manuallyRegisteredHandlers.Contains(type)))
            .AsSelfWithInterfaces()
            .WithScopedLifetime());

        // Переключение реализации топ-локаций: закомментируйте активную строку и раскомментируйте вторую.
        //services.AddScoped<
        //    IQueryHandler<IReadOnlyCollection<GetTopLocationDto>, GetTopLocationsQuery>,
        //    GetTopLocationsEfCoreHandler>();

        services.AddScoped<
            IQueryHandler<IReadOnlyCollection<GetTopLocationDto>, GetTopLocationsQuery>,
            GetTopLocationsDapperHandler>();

        services.TryDecorate(typeof(ICommandHandler<,>), typeof(ValidationBehaviour<,>));
        services.TryDecorate(typeof(ICommandHandler<>), typeof(ValidationBehaviour<>));
        services.TryDecorate(typeof(IQueryHandler<,>), typeof(QueryValidationBehaviour<,>));

        return services;
    }
}
