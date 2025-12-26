namespace DndShared.Models
{
    public class CharacterInformation
    {
        public string? Name { get; set; }
        public string? Race { get; set; }
        public string? Class { get; set; }
        public string? Subclass { get; set; }
        public string? Background { get; set; }
        public ChraracterSheetInformation? BaseInformation { get; set; }

        public CharacterAlignment? Alignment { get; set; }
        
    }

    public class ChraracterSheetInformation
    {
        public int? Level { get; set; }
        public int? ExperiencePoints { get; set; }
        public int? ArmorClass { get; set; }
        public int? Initiative { get; set; }
        public int? Speed { get; set; }
        public int? HitPoints { get; set; }
        public int? HitDice { get; set; }
        public int? ProficiencyBonus { get; set; }
        public int? PassivePerception { get; set; }
        public List<string>? Languages { get; set; }
        public List<string>? Tools { get; set; }
        public CharacterSkills? Skills { get; set; }
        public List<string>? SavingThrows { get; set; }
        public List<string>? WeaponTraining { get; set; }
        public List<string>? ArmorTraining { get; set; }
        public bool HeroicInspiration { get; set; }
        public List<string>? ClassFeatures { get; set; }
        public List<string>? SpecialTraits { get; set; }
        public List<CharacterWeapon>? Weapons { get; set; }
        public CharacterEquipment? Equipment { get; set; }
        public CharacterSpellcasting? Spellcasting { get; set; }
        public string? Personality { get; set; }
        public string? CharacterPortrait { get; set; }

    }

    public class CharacterSkills
    {
        public bool Acrobatics { get; set; }
        public bool AnimalHandling { get; set; }
        public bool Arcana { get; set; }
        public bool Athletics { get; set; }
        public bool Deception { get; set; }
        public bool History { get; set; }
        public bool Insight { get; set; }
        public bool Intimidation { get; set; }
        public bool Investigation { get; set; }
        public bool Medicine { get; set; }
        public bool Nature { get; set; }
        public bool Perception { get; set; }
        public bool Performance { get; set; }
        public bool Persuasion { get; set; }
        public bool Religion { get; set; }
        public bool SleightOfHand { get; set; }
        public bool Stealth { get; set; }
        public bool Survival { get; set; }
    }

    public class CharacterWeapon
    {
        public string? Name { get; set; }
        public string? AttackBonus { get; set; }
        public string? Damage { get; set; }
        public string? Notes { get; set; }
    }

    public class CharacterSpellcasting
    {
        public string? SpellcastingAbility { get; set; }
        public int? SpellCastingModifier { get; set; }
        public int? SpellSaveDC { get; set; }
        public int? SpellAttackBonus { get; set; }
        public int? SpellSlotsLevel1 { get; set; }
        public int? SpellSlotsLevel2 { get; set; }
        public int? SpellSlotsLevel3 { get; set; }
        public int? SpellSlotsLevel4 { get; set; }
        public int? SpellSlotsLevel5 { get; set; }
        public int? SpellSlotsLevel6 { get; set; }
        public int? SpellSlotsLevel7 { get; set; }
        public int? SpellSlotsLevel8 { get; set; }
        public int? SpellSlotsLevel9 { get; set; }
        public List<Spell>? Spells { get; set; }
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


