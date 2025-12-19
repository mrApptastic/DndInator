namespace DndShared.Models;

public class Weapon
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Category { get; set; }
    public string? Type { get; set; }
    public string? Damage { get; set; }
    public string? Properties { get; set; }
    public string? Mastery { get; set; }
    public string? Weight { get; set; }
    public string? Cost { get; set; }

    public Weapon()
    {
        Id = Guid.NewGuid();
    }

    public override string ToString()
    {
        return $"{Name} ({Damage}, {Category} {Type})";
    }
}
