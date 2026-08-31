using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;

namespace DirectoryService.Infrastructure.Postgres.Configurations;

/// <summary>
/// Конвертер value object ↔ примитив, который дополнительно пропускает значение,
/// уже приведённое к провайдерному типу.
/// </summary>
public class ValueObjectConverter<TModel, TProvider>(
    Expression<Func<TModel, TProvider>> convertToProvider,
    Expression<Func<TProvider, TModel>> convertFromProvider)
    : ValueConverter<TModel, TProvider>(convertToProvider, convertFromProvider)
{
    public override Func<object?, object?> ConvertToProvider =>
        value => value is TProvider provider ? provider : base.ConvertToProvider(value);
}
