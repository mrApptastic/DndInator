using DndScraper.Helpers;
using DndShared.Models;
using System.Text.Json;

namespace DndScraper;

public class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== D&D Scraper ===\n");

        Directory.CreateDirectory("..\\DndInator\\wwwroot\\data\\2014");
        Directory.CreateDirectory("..\\DndInator\\wwwroot\\data\\2024");
        
        // Scrape spells
        Console.WriteLine("1. Scraping spells 2014...");
        var spells2014 = await SpellScraper.ScrapeSpells2014();
        var spellsJson2014 = JsonSerializer.Serialize(spells2014, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("..\\DndInator\\wwwroot\\data\\2014\\spells.json", spellsJson2014);
        Console.WriteLine($"✓ Saved {spells2014.Count} spells to spells.json\n");

        Console.WriteLine("1. Scraping spells 2024...");
        var spells2024 = await SpellScraper.ScrapeSpells2024();
        var spellsJson2024 = JsonSerializer.Serialize(spells2024, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("..\\DndInator\\wwwroot\\data\\2024\\spells.json", spellsJson2024);
        Console.WriteLine($"✓ Saved {spells2024.Count} spells to spells.json\n");
        
        // Scrape lineages
        Console.WriteLine("2. Scraping lineages 2014...");
        var lineages2014 = await LineageScraper.ScrapeLineages();
        var lineagesJson2014 = JsonSerializer.Serialize(lineages2014, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("..\\DndInator\\wwwroot\\data\\2014\\lineages.json", lineagesJson2014);
        Console.WriteLine($"✓ Saved {lineages2014.Count} lineages to lineages.json\n");
        
        // Scrape backgrounds
        Console.WriteLine("3. Scraping backgrounds 2014...");
        var backgrounds2014 = await BackgroundScraper.ScrapeBackgrounds2014();
        var backgroundsJson2014 = JsonSerializer.Serialize(backgrounds2014, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("..\\DndInator\\wwwroot\\data\\2014\\backgrounds.json", backgroundsJson2014);
        Console.WriteLine($"✓ Saved {backgrounds2014.Count} backgrounds to backgrounds.json\n");
        
        Console.WriteLine("3. Scraping backgrounds 2024...");
        var backgrounds2024 = await BackgroundScraper.ScrapeBackgrounds2024();
        var backgroundsJson2024 = JsonSerializer.Serialize(backgrounds2024, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("..\\DndInator\\wwwroot\\data\\2024\\backgrounds.json", backgroundsJson2024);
        Console.WriteLine($"✓ Saved {backgrounds2024.Count} backgrounds to backgrounds.json\n");
        
        // Scrape species
        Console.WriteLine("4. Scraping species 2014...");
        var species2014 = await SpeciesScraper.ScrapeSpecies2014();
        var speciesJson2014 = JsonSerializer.Serialize(species2014, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("..\\DndInator\\wwwroot\\data\\2014\\species.json", speciesJson2014);
        Console.WriteLine($"✓ Saved {species2014.Count} species to species.json\n");

        Console.WriteLine("4. Scraping species 2024...");
        var species2024 = await SpeciesScraper.ScrapeSpecies2024();
        var speciesJson2024 = JsonSerializer.Serialize(species2024, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("..\\DndInator\\wwwroot\\data\\2024\\species.json", speciesJson2024);
        Console.WriteLine($"✓ Saved {species2024.Count} species to species.json\n");

        // Scrape feats
        Console.WriteLine("5. Scraping feats 2014...");
        var feats2014 = await FeatScraper.ScrapeFeats2014();
        var featsJson2014 = JsonSerializer.Serialize(feats2014, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("..\\DndInator\\wwwroot\\data\\2014\\feats.json", featsJson2014);
        Console.WriteLine($"✓ Saved {feats2014.Count} feats to feats.json\n");

        Console.WriteLine("5. Scraping feats 2024...");
        var feats2024 = await FeatScraper.ScrapeFeats2024();
        var featsJson2024 = JsonSerializer.Serialize(feats2024, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("..\\DndInator\\wwwroot\\data\\2024\\feats.json", featsJson2024);
        Console.WriteLine($"✓ Saved {feats2024.Count} feats to feats.json\n");
        
        // Scrape magic items
        Console.WriteLine("6. Scraping magic items 2014...");
        var magicItems2014 = await MagicItemScraper.ScrapeMagicItems2014();
        var magicItemsJson2014 = JsonSerializer.Serialize(magicItems2014, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("..\\DndInator\\wwwroot\\data\\2014\\magic-items.json", magicItemsJson2014);
        Console.WriteLine($"✓ Saved {magicItems2014.Count} magic items to magic-items.json\n");

        Console.WriteLine("6. Scraping magic items 2024...");
        var magicItems2024 = await MagicItemScraper.ScrapeMagicItems2024();
        var magicItemsJson2024 = JsonSerializer.Serialize(magicItems2024, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("..\\DndInator\\wwwroot\\data\\2024\\magic-items.json", magicItemsJson2024);
        Console.WriteLine($"✓ Saved {magicItems2024.Count} magic items to magic-items.json\n");

        // Scrape armor
        Console.WriteLine("7. Scraping armor 2014...");
        var armor2014 = await ArmorScraper.ScrapeArmor2014();
        var armorJson2014 = JsonSerializer.Serialize(armor2014, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("..\\DndInator\\wwwroot\\data\\2014\\armor.json", armorJson2014);
        Console.WriteLine($"✓ Saved {armor2014.Count} armor to armor.json\n");

        Console.WriteLine("7. Scraping armor 2024...");
        var armor2024 = await ArmorScraper.ScrapeArmor2024();
        var armorJson2024 = JsonSerializer.Serialize(armor2024, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("..\\DndInator\\wwwroot\\data\\2024\\armor.json", armorJson2024);
        Console.WriteLine($"✓ Saved {armor2024.Count} armor to armor.json\n");

        // Scrape weapons
        Console.WriteLine("8. Scraping weapons 2014...");
        var weapons2014 = await WeaponScraper.ScrapeWeapons2014();
        var weaponsJson2014 = JsonSerializer.Serialize(weapons2014, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("..\\DndInator\\wwwroot\\data\\2014\\weapons.json", weaponsJson2014);
        Console.WriteLine($"✓ Saved {weapons2014.Count} weapons to weapons.json\n");

        Console.WriteLine("8. Scraping weapons 2024...");
        var weapons2024 = await WeaponScraper.ScrapeWeapons2024();
        var weaponsJson2024 = JsonSerializer.Serialize(weapons2024, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("..\\DndInator\\wwwroot\\data\\2024\\weapons.json", weaponsJson2024);
        Console.WriteLine($"✓ Saved {weapons2024.Count} weapons to weapons.json\n");

        // Scrape adventuring gear
        Console.WriteLine("9. Scraping adventuring gear 2014...");
        var adventuringGear2014 = await AdventuringGearScraper.ScrapeAdventuringGear2014();
        var adventuringGearJson2014 = JsonSerializer.Serialize(adventuringGear2014, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("..\\DndInator\\wwwroot\\data\\2014\\adventuring-gear.json", adventuringGearJson2014);
        Console.WriteLine($"✓ Saved {adventuringGear2014.Count} adventuring gear to adventuring-gear.json\n");

        Console.WriteLine("9. Scraping adventuring gear 2024...");
        var adventuringGear2024 = await AdventuringGearScraper.ScrapeAdventuringGear2024();
        var adventuringGearJson2024 = JsonSerializer.Serialize(adventuringGear2024, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("..\\DndInator\\wwwroot\\data\\2024\\adventuring-gear.json", adventuringGearJson2024);
        Console.WriteLine($"✓ Saved {adventuringGear2024.Count} adventuring gear to adventuring-gear.json\n");

        // Scrape mounts and vehicles
        Console.WriteLine("10. Scraping mounts and vehicles 2014...");
        var mountsAndVehicles2014 = await MountAndVehicleScraper.ScrapeMountsAndVehicles2014();
        var mountsAndVehiclesJson2014 = JsonSerializer.Serialize(mountsAndVehicles2014, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("..\\DndInator\\wwwroot\\data\\2014\\mounts-and-vehicles.json", mountsAndVehiclesJson2014);
        Console.WriteLine($"✓ Saved {mountsAndVehicles2014.Count} mounts and vehicles to mounts-and-vehicles.json\n");

        Console.WriteLine("10. Scraping mounts and vehicles 2024...");
        var mountsAndVehicles2024 = await MountAndVehicleScraper.ScrapeMountsAndVehicles2024();
        var mountsAndVehiclesJson2024 = JsonSerializer.Serialize(mountsAndVehicles2024, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("..\\DndInator\\wwwroot\\data\\2024\\mounts-and-vehicles.json", mountsAndVehiclesJson2024);
        Console.WriteLine($"✓ Saved {mountsAndVehicles2024.Count} mounts and vehicles to mounts-and-vehicles.json\n");

        // Scrape poisons
        Console.WriteLine("11. Scraping poisons 2014...");
        var poisons2014 = await PoisonScraper.ScrapePoisons2014();
        var poisonsJson2014 = JsonSerializer.Serialize(poisons2014, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("..\\DndInator\\wwwroot\\data\\2014\\poisons.json", poisonsJson2014);
        Console.WriteLine($"✓ Saved {poisons2014.Count} poisons to poisons.json\n");

        Console.WriteLine("11. Scraping poisons 2024...");
        var poisons2024 = await PoisonScraper.ScrapePoisons2024();
        var poisonsJson2024 = JsonSerializer.Serialize(poisons2024, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("..\\DndInator\\wwwroot\\data\\2024\\poisons.json", poisonsJson2024);
        Console.WriteLine($"✓ Saved {poisons2024.Count} poisons to poisons.json\n");

        // Scrape trinkets
        Console.WriteLine("12. Scraping trinkets 2014...");
        var trinkets2014 = await TrinketScraper.ScrapeTrinkets2014();
        var trinketsJson2014 = JsonSerializer.Serialize(trinkets2014, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("..\\DndInator\\wwwroot\\data\\2014\\trinkets.json", trinketsJson2014);
        Console.WriteLine($"✓ Saved {trinkets2014.Count} trinkets to trinkets.json\n");

        Console.WriteLine("12. Scraping trinkets 2024...");
        var trinkets2024 = await TrinketScraper.ScrapeTrinkets2024();
        var trinketsJson2024 = JsonSerializer.Serialize(trinkets2024, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("..\\DndInator\\wwwroot\\data\\2024\\trinkets.json", trinketsJson2024);
        Console.WriteLine($"✓ Saved {trinkets2024.Count} trinkets to trinkets.json\n");

        // Scrape tools
        Console.WriteLine("13. Scraping tools 2014...");
        var tools2014 = await ToolScraper.ScrapeTools2014();
        var toolsJson2014 = JsonSerializer.Serialize(tools2014, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("..\\DndInator\\wwwroot\\data\\2014\\tools.json", toolsJson2014);
        Console.WriteLine($"✓ Saved {tools2014.Count} tools to tools.json\n");

        Console.WriteLine("13. Scraping tools 2024...");
        var tools2024 = await ToolScraper.ScrapeTools2024();
        var toolsJson2024 = JsonSerializer.Serialize(tools2024, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("..\\DndInator\\wwwroot\\data\\2024\\tools.json", toolsJson2024);
        Console.WriteLine($"✓ Saved {tools2024.Count} tools to tools.json\n");

        // Scrape character classes
        Console.WriteLine("14. Scraping character classes 2014...");
        var classes2014 = await CharacterClassScraper.ScrapeClasses2014();
        var classesJson2014 = JsonSerializer.Serialize(classes2014, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("..\\DndInator\\wwwroot\\data\\2014\\classes.json", classesJson2014);
        Console.WriteLine($"✓ Saved {classes2014.Count} character classes to classes.json\n");

        Console.WriteLine("14. Scraping character classes 2024...");
        var classes2024 = await CharacterClassScraper.ScrapeClasses2024();
        var classesJson2024 = JsonSerializer.Serialize(classes2024, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("..\\DndInator\\wwwroot\\data\\2024\\classes.json", classesJson2024);
        Console.WriteLine($"✓ Saved {classes2024.Count} character classes to classes.json\n");

        // Scrape subclasses
        Console.WriteLine("15. Scraping subclasses 2014...");
        var subclasses2014 = await CharacterClassScraper.ScrapeSubclasses2014();
        var subclassesJson2014 = JsonSerializer.Serialize(subclasses2014, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("..\\DndInator\\wwwroot\\data\\2014\\subclasses.json", subclassesJson2014);
        Console.WriteLine($"✓ Saved {subclasses2014.Count} subclasses to subclasses.json\n");

        Console.WriteLine("15. Scraping subclasses 2024...");
        var subclasses2024 = await CharacterClassScraper.ScrapeSubclasses2024();
        var subclassesJson2024 = JsonSerializer.Serialize(subclasses2024, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("..\\DndInator\\wwwroot\\data\\2024\\subclasses.json", subclassesJson2024);
        Console.WriteLine($"✓ Saved {subclasses2024.Count} subclasses to subclasses.json\n");

        Console.WriteLine("=== Scraping Complete ===");
    }
}
