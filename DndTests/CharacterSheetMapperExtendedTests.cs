using Xunit;
using DndShared.Models;
using DndInator.Services;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DndTests;

public class CharacterSheetMapperExtendedTests
{
    private static object? GetProperty(object obj, string propertyName)
    {
        var type = obj.GetType();
        var property = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        return property?.GetValue(obj);
    }

    [Fact]
    public void Map_SingleClass_ReturnsLevelAsNumber()
    {
        // Arrange
        var character = new Character
        {
            Information = new CharacterInformation
            {
                Name = "Test Character",
                BaseInformation = new ChraracterSheetInformation()
            },
            Classes = new List<CharacterClassLevel>
            {
                new CharacterClassLevel
                {
                    Class = new CharacterClass { Name = "Paladin" },
                    Level = 12
                }
            },
            Stats = new CharacterStats()
        };

        // Act
        var result = CharacterSheetMapper.Map(character);
        var baseInfo = GetProperty(result, "baseInformation");
        var level = GetProperty(baseInfo!, "level");
        var levelDisplay = GetProperty(baseInfo!, "levelDisplay");

        // Assert
        Assert.Equal(12, level);
        Assert.Equal("12", levelDisplay);
    }

    [Fact]
    public void Map_Multiclass_ReturnsLevelDisplayAsSplit()
    {
        // Arrange
        var character = new Character
        {
            Information = new CharacterInformation
            {
                Name = "Test Character",
                BaseInformation = new ChraracterSheetInformation()
            },
            Classes = new List<CharacterClassLevel>
            {
                new CharacterClassLevel
                {
                    Class = new CharacterClass { Name = "Paladin" },
                    Level = 8
                },
                new CharacterClassLevel
                {
                    Class = new CharacterClass { Name = "Rogue" },
                    Level = 4
                }
            },
            Stats = new CharacterStats()
        };

        // Act
        var result = CharacterSheetMapper.Map(character);
        var baseInfo = GetProperty(result, "baseInformation");
        var level = GetProperty(baseInfo!, "level");
        var levelDisplay = GetProperty(baseInfo!, "levelDisplay");

        // Assert
        Assert.Equal(12, level); // Total
        Assert.Equal("8/4", levelDisplay); // Split
    }

    [Fact]
    public void Map_MultipleSubclasses_AggregatesWithSlash()
    {
        // Arrange
        var character = new Character
        {
            Information = new CharacterInformation
            {
                Name = "Test Character",
                BaseInformation = new ChraracterSheetInformation()
            },
            Classes = new List<CharacterClassLevel>
            {
                new CharacterClassLevel
                {
                    Class = new CharacterClass { Name = "Paladin" },
                    Subclass = new Subclass { Name = "Oath of Devotion" },
                    Level = 8
                },
                new CharacterClassLevel
                {
                    Class = new CharacterClass { Name = "Rogue" },
                    Subclass = new Subclass { Name = "Thief" },
                    Level = 4
                }
            },
            Stats = new CharacterStats()
        };

        // Act
        var result = CharacterSheetMapper.Map(character);
        var classInfo = GetProperty(result, "class");
        var subclassName = GetProperty(classInfo!, "subclassName");

        // Assert
        Assert.Equal("Oath of Devotion / Thief", subclassName);
    }

    [Fact]
    public void Map_SubclassWithNull_SkipsNull()
    {
        // Arrange
        var character = new Character
        {
            Information = new CharacterInformation
            {
                Name = "Test Character",
                BaseInformation = new ChraracterSheetInformation()
            },
            Classes = new List<CharacterClassLevel>
            {
                new CharacterClassLevel
                {
                    Class = new CharacterClass { Name = "Paladin" },
                    Subclass = new Subclass { Name = "Oath of Devotion" },
                    Level = 8
                },
                new CharacterClassLevel
                {
                    Class = new CharacterClass { Name = "Fighter" },
                    Subclass = null, // No subclass
                    Level = 4
                }
            },
            Stats = new CharacterStats()
        };

        // Act
        var result = CharacterSheetMapper.Map(character);
        var classInfo = GetProperty(result, "class");
        var subclassName = GetProperty(classInfo!, "subclassName");

        // Assert
        Assert.Equal("Oath of Devotion", subclassName);
    }

    [Fact]
    public void Map_ClassFeatures_ExposesFromOverrides()
    {
        // Arrange
        var character = new Character
        {
            Information = new CharacterInformation
            {
                Name = "Test Character",
                BaseInformation = new ChraracterSheetInformation
                {
                    ClassFeatures = new List<string>
                    {
                        "Divine Sense",
                        "Lay on Hands",
                        "Sneak Attack"
                    }
                }
            },
            Classes = new List<CharacterClassLevel>
            {
                new CharacterClassLevel { Class = new CharacterClass { Name = "Paladin" }, Level = 5 }
            },
            Stats = new CharacterStats()
        };

        // Act
        var result = CharacterSheetMapper.Map(character);
        var classInfo = GetProperty(result, "class");
        var features = GetProperty(classInfo!, "features") as string;

        // Assert
        Assert.NotNull(features);
        Assert.Contains("Divine Sense", features!);
        Assert.Contains("Lay on Hands", features!);
        Assert.Contains("Sneak Attack", features!);
    }

    [Fact]
    public void Map_SpecialTraits_ExposedSeparately()
    {
        // Arrange
        var character = new Character
        {
            Information = new CharacterInformation
            {
                Name = "Test Character",
                BaseInformation = new ChraracterSheetInformation
                {
                    SpecialTraits = new List<string>
                    {
                        "Darkvision",
                        "Fey Ancestry"
                    }
                }
            },
            Classes = new List<CharacterClassLevel>
            {
                new CharacterClassLevel { Class = new CharacterClass { Name = "Rogue" }, Level = 3 }
            },
            Stats = new CharacterStats()
        };

        // Act
        var result = CharacterSheetMapper.Map(character);
        var traits = GetProperty(result, "specialTraits") as List<string>;

        // Assert
        Assert.NotNull(traits);
        Assert.Equal(2, traits!.Count);
        Assert.Contains("Darkvision", traits);
        Assert.Contains("Fey Ancestry", traits);
    }

    [Fact]
    public void Map_AllEquipmentCategories_IncludedInOutput()
    {
        // Arrange
        var character = new Character
        {
            Information = new CharacterInformation
            {
                Name = "Test Character",
                BaseInformation = new ChraracterSheetInformation
                {
                    Equipment = new CharacterEquipment
                    {
                        Armors = new List<Armor> { new Armor { Name = "Chainmail" } },
                        Weapons = new List<Weapon> { new Weapon { Name = "Longsword" } },
                        AdventuringGears = new List<AdventuringGear> { new AdventuringGear { Name = "Rope" } },
                        Tools = new List<Tool> { new Tool { Name = "Thieves' Tools" } },
                        MagicItems = new List<MagicItem> { new MagicItem { Name = "Cloak of Protection" } },
                        MountsAndVehicles = new List<MountAndVehicle> { new MountAndVehicle { Name = "Horse" } },
                        Poisons = new List<Poison> { new Poison { Name = "Basic Poison" } }
                    }
                }
            },
            Classes = new List<CharacterClassLevel>
            {
                new CharacterClassLevel { Class = new CharacterClass { Name = "Rogue" }, Level = 5 }
            },
            Stats = new CharacterStats()
        };

        // Act
        var result = CharacterSheetMapper.Map(character);
        var equipment = GetProperty(result, "equipment");

        // Assert
        Assert.NotNull(equipment);
        Assert.NotNull(GetProperty(equipment!, "armor"));
        Assert.NotNull(GetProperty(equipment!, "weapons"));
        Assert.NotNull(GetProperty(equipment!, "adventuringGear"));
        Assert.NotNull(GetProperty(equipment!, "tools"));
        Assert.NotNull(GetProperty(equipment!, "magicItems"));
        Assert.NotNull(GetProperty(equipment!, "mountAndVehicles"));
        Assert.NotNull(GetProperty(equipment!, "poisons"));
    }
}
