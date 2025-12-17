namespace DndShared.Models;

public class Lineage
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Content { get; set; }
    public List<Subrace> Subraces { get; set; }

    public Lineage()
    {
        Id = Guid.NewGuid();
        Subraces = new List<Subrace>();
    }

    public override string ToString()
    {
        return $"{Name} ({Subraces.Count} subraces)";
    }
}

public class Subrace
{
    public string? Name { get; set; }
    public string? Content { get; set; }
}
