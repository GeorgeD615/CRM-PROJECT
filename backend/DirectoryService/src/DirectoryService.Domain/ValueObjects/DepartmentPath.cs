namespace DirectoryService.Domain.ValueObjects;

public sealed record DepartmentPath
{
    public const char Separator = '/';

    private DepartmentPath(string value) => Value = value;

    public string Value { get; }

    public static DepartmentPath Create(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        string[] segments = value.Split(Separator);

        if (segments.Length == 0)
            throw new ArgumentException("Department path must not be empty.", nameof(value));

        // Путь восстанавливается из уже валидного (сохранённого) значения — slug'и заведомо корректны.
        DepartmentPath path = CreateRoot(DepartmentSlug.Create(segments[0]).Value);

        foreach (string segment in segments.Skip(1))
            path = path.CreateChild(DepartmentSlug.Create(segment).Value);

        return path;
    }

    public static DepartmentPath CreateRoot(DepartmentSlug slug)
    {
        ArgumentNullException.ThrowIfNull(slug);

        return new DepartmentPath(slug.Value);
    }

    public DepartmentPath CreateChild(DepartmentSlug slug)
    {
        ArgumentNullException.ThrowIfNull(slug);

        return new DepartmentPath($"{Value}{Separator}{slug.Value}");
    }
}
