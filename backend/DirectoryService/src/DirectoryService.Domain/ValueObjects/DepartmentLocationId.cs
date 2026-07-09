namespace DirectoryService.Domain.ValueObjects;

public sealed record DepartmentLocationId
{
    private DepartmentLocationId(Guid value) => Value = value;

    public Guid Value { get; }

    public static DepartmentLocationId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("DepartmentLocation id must not be empty.", nameof(value));

        return new DepartmentLocationId(value);
    }
}
