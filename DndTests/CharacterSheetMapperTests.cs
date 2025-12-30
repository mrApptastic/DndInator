using System.Collections.Generic;
using System.Text.Json;
using DndInator.Services;
using DndShared.Models;
using Xunit;

namespace DndTests;

public class CharacterSheetMapperTests
{
    [Fact]
    public void Map_ProducesJsFriendlyShape()
    {
        var character = new Character
        {
            Information = new CharacterInformation
            {
                Name = "Test Rogue",
                Race = "Elf",
                Class = "Rogue",
                Background = "Criminal",
                Subclass = "Assassin",
                Alignment = CharacterAlignment.ChaoticNeutral,
                BaseInformation = new ChraracterSheetInformation
                {
                    Level = 3,
                    ExperiencePoints = 900,
                    ArmorClass = 14,
                    Initiative = 2,
                    Speed = 30,
                    HitPoints = 21,
                    HitDice = 3,
                    ProficiencyBonus = 2,
                    PassivePerception = 13,
                    Languages = new List<string> { "Common", "Elvish" },
                    Tools = new List<string> { "Thieves' Tools" },
                    Skills = new CharacterSkills
                    {
                        Acrobatics = true,
                        Stealth = true
                    },
                    WeaponTraining = new List<string> { "Simple weapons", "Hand crossbows" },
                    ArmorTraining = new List<string> { "Light armor" },
                    Spellcasting = new CharacterSpellcasting
                    {
                        SpellcastingAbility = "Intelligence",
                        SpellCastingModifier = 3,
                        SpellSaveDC = 13,
                        SpellAttackBonus = 5,
                        SpellSlotsLevel1 = 2,
                        Spells = new List<Spell>
                        {
                            new Spell
                            {
                                Level = 1,
                                Name = "Mage Armor",
                                CastingTime = "1 action",
                                Range = "Touch",
                                Components = "V,S,M",
                                Description = "+3 AC"
                            }
                        }
                    },
                    Equipment = new CharacterEquipment
                    {
                        Armors = new List<Armor> { new Armor { Name = "Leather", ArmorClass = "11" } },
                        Weapons = new List<Weapon> { new Weapon { Name = "Dagger", Damage = "1d4", Properties = "Finesse" } },
                        AdventuringGears = new List<AdventuringGear>(),
                        Tools = new List<Tool>(),
                        Wealth = new CharacterWealth { GoldPieces = 10, SilverPieces = 5, CopperPieces = 3 }
                    },
                    Personality = "Curious"
                }
            },
            Race = new Species
            {
                Name = "Elf",
                Speed = "30",
                Size = "Medium",
                Traits = new List<string> { "Darkvision" }
            },
            Class = new CharacterClass
            {
                Name = "Rogue",
                SavingThrowProficiencies = "Dexterity, Intelligence"
            },
            Background = new Background
            {
                Name = "Criminal",
                ToolProficiency = "Thieves' Tools",
                Equipment = "Crowbar",
                SkillProficiencies = new List<string> { "Deception", "Stealth" }
            },
            Stats = new CharacterStats
            {
                Strength = 8,
                Dexterity = 16,
                Constitution = 12,
                Intelligence = 14,
                Wisdom = 10,
                Charisma = 11
            },
            Feats = new List<Feat>
            {
                new Feat { Name = "Alert", Description = "Always ready" }
            }
        };

        var payload = CharacterSheetMapper.Map(character);
        var json = JsonSerializer.Serialize(payload);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.Equal(3, root.GetProperty("baseInformation").GetProperty("level").GetInt32());
        Assert.Equal("Rogue", root.GetProperty("class").GetProperty("name").GetString());
        Assert.Equal("Elf", root.GetProperty("species").GetProperty("name").GetString());

        var abilities = root.GetProperty("abilityScores");
        Assert.Equal(16, abilities.GetProperty("dexterity").GetInt32());

        var skills = root.GetProperty("skills");
        Assert.True(skills.GetProperty("Stealth").GetBoolean());
        Assert.True(skills.GetProperty("Acrobatics").GetBoolean());

        var spellcasting = root.GetProperty("spellcasting");
        Assert.Equal(2, spellcasting.GetProperty("level1Slots").GetInt32());
        Assert.Equal("Mage Armor", spellcasting.GetProperty("spells")[0].GetProperty("name").GetString());

        var equipment = root.GetProperty("equipment");
        Assert.Equal("Leather", equipment.GetProperty("armor").GetProperty("name").GetString());
        Assert.Equal("Dagger", equipment.GetProperty("weapons")[0].GetProperty("name").GetString());

        var wealth = root.GetProperty("wealth");
        Assert.Equal(10, wealth.GetProperty("gold").GetInt32());
        Assert.Equal(5, wealth.GetProperty("silver").GetInt32());
        Assert.Equal(3, wealth.GetProperty("copper").GetInt32());
    }
}
