namespace DndShared.Models;

public class CharacterClass
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Source { get; set; }
    public string? Description { get; set; }
    public string? PrimaryAbility { get; set; }
    public string? HitPointDie { get; set; }
    public string? SavingThrowProficiencies { get; set; }
    public string? SkillProficiencies { get; set; }
    public string? WeaponProficiencies { get; set; }
    public string? ArmorTraining { get; set; }
    public string? ToolProficiencies { get; set; }
    public string? StartingEquipment { get; set; }
    public List<string> SubclassNames { get; set; }
    public List<Subclass> Subclasses { get; set; }
    public string? FullContent { get; set; }

    public CharacterClass()
    {
        Id = Guid.NewGuid();
        SubclassNames = new List<string>();
        Subclasses = new List<Subclass>();
    }

    public override string ToString()
    {
        return $"{Name} (Primary: {PrimaryAbility})";
    }
}
