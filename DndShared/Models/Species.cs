namespace DndShared.Models;

public class Species
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Category { get; set; }
    public string? Source { get; set; }
    public string? Description { get; set; }
    public string? CreatureType { get; set; }
    public string? Size { get; set; }
    public string? Speed { get; set; }
    public List<string> Traits { get; set; }
    public string? FullContent { get; set; }

    public Species()
    {
        Id = Guid.NewGuid();
        Traits = new List<string>();
    }

    public override string ToString()
    {
        return $"{Name} ({Category})";
    }
}
