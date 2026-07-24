using System.Collections;

namespace DirectoryService.Shared;

/// <summary>
/// Непустая коллекция ошибок
/// </summary>
public sealed class Failure : IEnumerable<Error>
{
    private readonly IReadOnlyList<Error> _errors;

    public Failure(IEnumerable<Error> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        _errors = errors.ToList();

        if (_errors.Count == 0)
        {
            throw new ArgumentException("Failure must contain at least one error.", nameof(errors));
        }
    }

    public IEnumerator<Error> GetEnumerator() => _errors.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static implicit operator Failure(Error error) => new([error]);

    public static implicit operator Failure(Error[] errors) => new(errors);

    public static Failure FromError(Error error) => new([error]);

    public static Failure FromErrorArray(Error[] errors) => new(errors);

    public static Failure From(params Error[] errors) => new(errors);
}
