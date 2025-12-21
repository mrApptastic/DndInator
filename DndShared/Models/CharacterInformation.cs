namespace DndShared.Models
{
    public class CharacterInformation
    {
        public string? Name { get; set; }
        public string? Race { get; set; }
        public string? Class { get; set; }
        public string? Background { get; set; }
        public CharacterAlignment? Alignment { get; set; }
        
    }

    public enum CharacterAlignment
    {
        LawfulGood,
        NeutralGood,
        ChaoticGood,
        LawfulNeutral,
        TrueNeutral,
        ChaoticNeutral,
        LawfulEvil,
        NeutralEvil,
        ChaoticEvil
    }
}


