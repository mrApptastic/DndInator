namespace DndShared.Models;

public class Background
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Category { get; set; }
    public string? Source { get; set; }
    public string? Description { get; set; }
    public List<string> AbilityScores { get; set; }
    public string? Feat { get; set; }
    public List<string> SkillProficiencies { get; set; }
    public string? ToolProficiency { get; set; }
    public string? Equipment { get; set; }
    public string? FullContent { get; set; }

    public Background()
    {
        Id = Guid.NewGuid();
        AbilityScores = new List<string>();
        SkillProficiencies = new List<string>();
    }

    public override string ToString()
    {
        return $"{Name} ({Category})";
    }
}
