namespace DndShared.Models
{
    public class CharacterStats
    {
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Constitution { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Charisma { get; set; }

        // Helper methods for calculating ability modifiers
        public int GetStrengthModifier() => GetAbilityModifier(Strength);
        public int GetDexterityModifier() => GetAbilityModifier(Dexterity);
        public int GetConstitutionModifier() => GetAbilityModifier(Constitution);
        public int GetIntelligenceModifier() => GetAbilityModifier(Intelligence);
        public int GetWisdomModifier() => GetAbilityModifier(Wisdom);
        public int GetCharismaModifier() => GetAbilityModifier(Charisma);

        public static int GetAbilityModifier(int abilityScore)
        {
            return (abilityScore - 10) / 2;
        }

        public string GetModifierString(int abilityScore)
        {
            int modifier = GetAbilityModifier(abilityScore);
            return modifier >= 0 ? $"+{modifier}" : modifier.ToString();
        }
    }
}