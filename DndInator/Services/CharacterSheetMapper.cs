using System;
using System.Collections.Generic;
using System.Linq;
using DndShared.Models;

namespace DndInator.Services;

public static class CharacterSheetMapper
{
    public static object Map(Character character)
    {
        if (character == null) throw new ArgumentNullException(nameof(character));

        var information = character.Information;
        var baseInfo = information?.BaseInformation;
        var stats = character.Stats;

        var abilityScores = stats != null ? new
        {
            strength = stats.Strength,
            dexterity = stats.Dexterity,
            constitution = stats.Constitution,
            intelligence = stats.Intelligence,
            wisdom = stats.Wisdom,
            charisma = stats.Charisma
        } : null;

        var skills = baseInfo?.Skills != null ? new Dictionary<string, bool>
        {
            ["Acrobatics"] = baseInfo.Skills.Acrobatics,
            ["Animal Handling"] = baseInfo.Skills.AnimalHandling,
            ["Arcana"] = baseInfo.Skills.Arcana,
            ["Athletics"] = baseInfo.Skills.Athletics,
            ["Deception"] = baseInfo.Skills.Deception,
            ["History"] = baseInfo.Skills.History,
            ["Insight"] = baseInfo.Skills.Insight,
            ["Intimidation"] = baseInfo.Skills.Intimidation,
            ["Investigation"] = baseInfo.Skills.Investigation,
            ["Medicine"] = baseInfo.Skills.Medicine,
            ["Nature"] = baseInfo.Skills.Nature,
            ["Perception"] = baseInfo.Skills.Perception,
            ["Performance"] = baseInfo.Skills.Performance,
            ["Persuasion"] = baseInfo.Skills.Persuasion,
            ["Religion"] = baseInfo.Skills.Religion,
            ["Sleight of Hand"] = baseInfo.Skills.SleightOfHand,
            ["Stealth"] = baseInfo.Skills.Stealth,
            ["Survival"] = baseInfo.Skills.Survival
        } : null;

        var spellcasting = baseInfo?.Spellcasting != null ? new
        {
            ability = baseInfo.Spellcasting.SpellcastingAbility,
            modifier = baseInfo.Spellcasting.SpellCastingModifier,
            saveDC = baseInfo.Spellcasting.SpellSaveDC,
            attackBonus = baseInfo.Spellcasting.SpellAttackBonus,
            level1Slots = baseInfo.Spellcasting.SpellSlotsLevel1,
            level2Slots = baseInfo.Spellcasting.SpellSlotsLevel2,
            level3Slots = baseInfo.Spellcasting.SpellSlotsLevel3,
            level4Slots = baseInfo.Spellcasting.SpellSlotsLevel4,
            level5Slots = baseInfo.Spellcasting.SpellSlotsLevel5,
            level6Slots = baseInfo.Spellcasting.SpellSlotsLevel6,
            level7Slots = baseInfo.Spellcasting.SpellSlotsLevel7,
            level8Slots = baseInfo.Spellcasting.SpellSlotsLevel8,
            level9Slots = baseInfo.Spellcasting.SpellSlotsLevel9,
            spells = baseInfo.Spellcasting.Spells?.Select(spell => new
            {
                level = spell.Level,
                name = spell.Name,
                castingTime = spell.CastingTime,
                range = spell.Range,
                components = spell.Components,
                description = spell.Description,
                notes = spell.Description
            }).ToList()
        } : null;

        var equipmentSource = baseInfo?.Equipment;
        var armor = equipmentSource?.Armors?.FirstOrDefault();
        var equipment = equipmentSource != null ? new
        {
            armor = armor != null ? new
            {
                name = armor.Name,
                armorClass = armor.ArmorClass,
                isShield = armor.Name != null && armor.Name.Contains("shield", StringComparison.OrdinalIgnoreCase)
            } : null,
            weapons = equipmentSource.Weapons?.Select(w => new
            {
                name = w.Name,
                attackBonus = string.Empty,
                damage = w.Damage,
                properties = w.Properties
            }).ToList(),
            adventuringGear = equipmentSource.AdventuringGears?.Select(g => new { name = g.Name }).ToList(),
            tools = equipmentSource.Tools?.Select(t => new { name = t.Name }).ToList(),
            magicItems = equipmentSource.MagicItems?.Select(m => new { name = m.Name, requiresAttunement = false }).ToList(),
            mountAndVehicles = equipmentSource.MountsAndVehicles?.Select(m => new { name = m.Name }).ToList(),
            poisons = equipmentSource.Poisons?.Select(p => new { name = p.Name }).ToList(),
            customItems = equipmentSource.CustomItems?.Select(ci => new { name = ci.Name, description = ci.Description, weight = ci.Weight }).ToList()
        } : null;

        var wealthSource = equipmentSource?.Wealth;
        var wealth = wealthSource != null ? new
        {
            copper = wealthSource.CopperPieces,
            silver = wealthSource.SilverPieces,
            electrum = wealthSource.ElectrumPieces,
            gold = wealthSource.GoldPieces,
            platinum = wealthSource.PlatinumPieces
        } : null;

        var proficiencies = baseInfo?.WeaponTraining != null || baseInfo?.Tools != null ? new
        {
            weapons = baseInfo?.WeaponTraining ?? new List<string>(),
            tools = baseInfo?.Tools ?? new List<string>()
        } : null;

        var species = character.Race != null ? new
        {
            name = character.Race.Name,
            speed = character.Race.Speed,
            size = character.Race.Size,
            traits = character.Race.Traits
        } : null;

        var baseInformation = baseInfo != null
            ? (object)new
            {
                characterName = information?.Name,
                name = information?.Name,
                level = baseInfo.Level ?? 1,
                experiencePoints = baseInfo.ExperiencePoints ?? 0,
                armorClass = baseInfo.ArmorClass,
                initiative = baseInfo.Initiative ?? stats?.GetDexterityModifier(),
                speed = baseInfo.Speed ?? ParseSpeed(species?.speed) ?? 30,
                hitPoints = baseInfo.HitPoints,
                hitDice = baseInfo.HitDice,
                proficiencyBonus = baseInfo.ProficiencyBonus,
                passivePerception = baseInfo.PassivePerception ?? (stats != null ? 10 + stats.GetWisdomModifier() : null),
                languages = baseInfo.Languages ?? new List<string>(),
                tools = baseInfo.Tools ?? new List<string>(),
                personalityTraits = baseInfo.Personality,
                characterPortrait = baseInfo.CharacterPortrait
            }
            : information != null
                ? (object)new
                {
                    characterName = information.Name,
                    name = information.Name,
                    level = 1,
                    experiencePoints = 0,
                    armorClass = (int?)null,
                    initiative = (int?)null,
                    speed = 30,
                    hitPoints = (int?)null,
                    hitDice = (int?)null,
                    proficiencyBonus = (int?)null,
                    passivePerception = (int?)null,
                    languages = new List<string>(),
                    tools = new List<string>(),
                    personalityTraits = information.BaseInformation?.Personality,
                    characterPortrait = information.BaseInformation?.CharacterPortrait
                }
                : null;

        var classPayload = character.Class != null ? new
        {
            name = character.Class.Name,
            subclassName = information?.Subclass,
            savingThrowProficiencies = SplitCsv(character.Class.SavingThrowProficiencies),
            skillProficiencies = character.Class.SkillProficiencies,
            weaponProficiencies = SplitCsv(character.Class.WeaponProficiencies),
            armorTraining = SplitCsv(character.Class.ArmorTraining),
            toolProficiencies = SplitCsv(character.Class.ToolProficiencies),
            startingEquipment = character.Class.StartingEquipment,
            source = character.Class.Source
        } : null;

        var background = character.Background != null ? new
        {
            name = character.Background.Name,
            description = character.Background.Description,
            skillProficiencies = character.Background.SkillProficiencies ?? new List<string>(),
            toolProficiency = character.Background.ToolProficiency,
            equipment = character.Background.Equipment,
            source = character.Background.Source
        } : null;

        var feats = character.Feats?.Select(f => new
        {
            name = f.Name,
            description = f.Description,
            benefits = f.Benefits ?? new List<string>()
        }).ToList();

        return new
        {
            baseInformation,
            information = information != null ? new
            {
                name = information.Name,
                race = information.Race,
                @class = information.Class,
                background = information.Background,
                alignment = information.Alignment?.ToString(),
                subclass = information.Subclass
            } : null,
            background,
            @class = classPayload,
            species,
            race = species,
            abilityScores,
            stats = abilityScores,
            skills,
            spellcasting,
            equipment,
            wealth,
            proficiencies,
            languages = baseInfo?.Languages ?? new List<string>(),
            feats,
            characterPortrait = baseInfo?.CharacterPortrait ?? information?.BaseInformation?.CharacterPortrait
        };
    }

    private static List<string> SplitCsv(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? new List<string>()
            : value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(v => v.Trim())
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .ToList();
    }

    private static int? ParseSpeed(string? speed)
    {
        if (string.IsNullOrWhiteSpace(speed)) return null;
        var numeric = new string(speed.TakeWhile(char.IsDigit).ToArray());
        return int.TryParse(numeric, out var value) ? value : null;
    }
}
