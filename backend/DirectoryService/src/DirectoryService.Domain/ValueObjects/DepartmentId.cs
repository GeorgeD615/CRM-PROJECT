namespace DirectoryService.Domain.ValueObjects;

public sealed record DepartmentId
{
    private DepartmentId(Guid value) => Value = value;

    public Guid Value { get; }

    public static DepartmentId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Department id must not be empty.", nameof(value));

        return new DepartmentId(value);
    }
}
