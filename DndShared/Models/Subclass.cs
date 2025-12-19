namespace DndShared.Models;

public class Subclass
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? ParentClass { get; set; }
    public string? Source { get; set; }
    public string? Description { get; set; }
    public string? FullContent { get; set; }

    public Subclass()
    {
        Id = Guid.NewGuid();
    }

    public override string ToString()
    {
        return $"{Name} ({ParentClass})";
    }
}
