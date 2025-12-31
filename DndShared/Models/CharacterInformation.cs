namespace DndShared.Models
{
    public enum SkillProficiency
    {
        None = 0,
        Proficiency = 1,
        Expertise = 2
    }

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
        public SkillProficiency Acrobatics { get; set; } = SkillProficiency.None;
        public SkillProficiency AnimalHandling { get; set; } = SkillProficiency.None;
        public SkillProficiency Arcana { get; set; } = SkillProficiency.None;
        public SkillProficiency Athletics { get; set; } = SkillProficiency.None;
        public SkillProficiency Deception { get; set; } = SkillProficiency.None;
        public SkillProficiency History { get; set; } = SkillProficiency.None;
        public SkillProficiency Insight { get; set; } = SkillProficiency.None;
        public SkillProficiency Intimidation { get; set; } = SkillProficiency.None;
        public SkillProficiency Investigation { get; set; } = SkillProficiency.None;
        public SkillProficiency Medicine { get; set; } = SkillProficiency.None;
        public SkillProficiency Nature { get; set; } = SkillProficiency.None;
        public SkillProficiency Perception { get; set; } = SkillProficiency.None;
        public SkillProficiency Performance { get; set; } = SkillProficiency.None;
        public SkillProficiency Persuasion { get; set; } = SkillProficiency.None;
        public SkillProficiency Religion { get; set; } = SkillProficiency.None;
        public SkillProficiency SleightOfHand { get; set; } = SkillProficiency.None;
        public SkillProficiency Stealth { get; set; } = SkillProficiency.None;
        public SkillProficiency Survival { get; set; } = SkillProficiency.None;
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


