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
    public List<string> Subclasses { get; set; }
    public string? FullContent { get; set; }

    public CharacterClass()
    {
        Id = Guid.NewGuid();
        Subclasses = new List<string>();
    }

    public override string ToString()
    {
        return $"{Name} (Primary: {PrimaryAbility})";
    }
}
