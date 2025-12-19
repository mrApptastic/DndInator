namespace DndShared.Models;

public class Trinket
{
    public Guid Id { get; set; }
    public string? Roll { get; set; }
    public string? Description { get; set; }

    public Trinket()
    {
        Id = Guid.NewGuid();
    }

    public override string ToString()
    {
        return $"{Roll}: {Description}";
    }
}
