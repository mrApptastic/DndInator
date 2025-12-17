namespace DndShared.Models;

public class Feat
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Category { get; set; }
    public string? Source { get; set; }
    public string? Description { get; set; }
    public string? Level { get; set; }
    public string? Prerequisite { get; set; }
    public string? Repeatable { get; set; }
    public List<string> Benefits { get; set; }
    public string? FullContent { get; set; }

    public Feat()
    {
        Id = Guid.NewGuid();
        Benefits = new List<string>();
    }

    public override string ToString()
    {
        return $"{Name} ({Category})";
    }
}
