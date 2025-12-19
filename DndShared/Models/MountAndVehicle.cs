namespace DndShared.Models;

public class MountAndVehicle
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Category { get; set; }
    public string? CarryingCapacity { get; set; }
    public string? Weight { get; set; }
    public string? Speed { get; set; }
    public string? Crew { get; set; }
    public string? Passengers { get; set; }
    public string? Cargo { get; set; }
    public string? AC { get; set; }
    public string? HP { get; set; }
    public string? DamageThreshold { get; set; }
    public string? Cost { get; set; }

    public MountAndVehicle()
    {
        Id = Guid.NewGuid();
    }

    public override string ToString()
    {
        return $"{Name} ({Category}, {Cost})";
    }
}
