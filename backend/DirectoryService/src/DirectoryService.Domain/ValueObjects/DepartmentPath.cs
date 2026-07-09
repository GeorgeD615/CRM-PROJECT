namespace DirectoryService.Domain.ValueObjects;

public sealed record DepartmentPath
{
    public const char Separator = '/';

    private DepartmentPath(string value) => Value = value;

    public string Value { get; }

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
