namespace DirectoryService.Domain.ValueObjects;

public sealed record DepartmentPositionId
{
    private DepartmentPositionId(Guid value) => Value = value;

    public Guid Value { get; }

    public static DepartmentPositionId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("DepartmentPosition id must not be empty.", nameof(value));

        return new DepartmentPositionId(value);
    }
}
