using CSharpFunctionalExtensions;
using DirectoryService.Shared;
using FluentValidation;

namespace DirectoryService.Core.Validations;

/// <summary>
/// Мост между FluentValidation и доменными фабриками Value Object-ов: правило поля переиспользует
/// доменную проверку вместо дублирования, а её <see cref="Failure"/> переносится в validation result
/// (каждый <see cref="Error"/> — отдельным failure). Доменный Error переносится через сообщение
/// в сериализованном виде и восстанавливается в <c>ValidationExtensions.ToErrors</c>.
/// </summary>
public static class CustomValidators
{
    public static IRuleBuilderOptionsConditions<T, TElement> MustBeValueObject<T, TElement, TValueObject>(
        this IRuleBuilder<T, TElement> ruleBuilder,
        Func<TElement, Result<TValueObject, Failure>> factoryMethod)
    {
        return ruleBuilder.Custom((value, context) =>
        {
            Result<TValueObject, Failure> result = factoryMethod(value);

            if (result.IsSuccess)
                return;

            foreach (Error error in result.Error)
                context.AddFailure(error.Serialize());
        });
    }

    public static IRuleBuilderOptions<T, TProperty> WithError<T, TProperty>(
        this IRuleBuilderOptions<T, TProperty> rule, Error error)
    {
        return rule.WithMessage(error.Serialize());
    }
}
