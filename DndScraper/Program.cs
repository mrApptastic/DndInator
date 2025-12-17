using DndScraper.Helpers;
using DndShared.Models;
using System.Text.Json;

namespace DndScraper;

public class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== D&D Scraper ===\n");
        
        // Scrape spells
        // Console.WriteLine("1. Scraping spells 2014...");
        // var spells2014 = await SpellScraper.ScrapeSpells2014();
        // var spellsJson2014 = JsonSerializer.Serialize(spells2014, new JsonSerializerOptions { WriteIndented = true });
        // File.WriteAllText("..\\DndInator\\wwwroot\\data\\2014\\spells.json", spellsJson2014);
        // Console.WriteLine($"✓ Saved {spells2014.Count} spells to spells.json\n");

        // Console.WriteLine("1. Scraping spells 2024...");
        // var spells2024 = await SpellScraper.ScrapeSpells2024();
        // var spellsJson2024 = JsonSerializer.Serialize(spells2024, new JsonSerializerOptions { WriteIndented = true });
        // File.WriteAllText("..\\DndInator\\wwwroot\\data\\2024\\spells.json", spellsJson2024);
        // Console.WriteLine($"✓ Saved {spells2024.Count} spells to spells.json\n");
        
        // // Scrape lineages
        // Console.WriteLine("2. Scraping lineages 2014...");
        // var lineages2014 = await LineageScraper.ScrapeLineages();
        // var lineagesJson2014 = JsonSerializer.Serialize(lineages2014, new JsonSerializerOptions { WriteIndented = true });
        // File.WriteAllText("..\\DndInator\\wwwroot\\data\\2014\\lineages.json", lineagesJson2014);
        // Console.WriteLine($"✓ Saved {lineages2014.Count} lineages to lineages.json\n");
        
        // // Scrape backgrounds
        // Console.WriteLine("3. Scraping backgrounds 2024...");
        // var backgrounds2024 = await BackgroundScraper.ScrapeBackgrounds2024();
        // var backgroundsJson2024 = JsonSerializer.Serialize(backgrounds2024, new JsonSerializerOptions { WriteIndented = true });
        // File.WriteAllText("..\\DndInator\\wwwroot\\data\\2024\\backgrounds.json", backgroundsJson2024);
        // Console.WriteLine($"✓ Saved {backgrounds2024.Count} backgrounds to backgrounds.json\n");
        
        // // Scrape species
        // Console.WriteLine("4. Scraping species 2024...");
        // var species2024 = await SpeciesScraper.ScrapeSpecies2024();
        // var speciesJson2024 = JsonSerializer.Serialize(species2024, new JsonSerializerOptions { WriteIndented = true });
        // File.WriteAllText("..\\DndInator\\wwwroot\\data\\2024\\species.json", speciesJson2024);
        // Console.WriteLine($"✓ Saved {species2024.Count} species to species.json\n");

        // // Scrape feats
        // Console.WriteLine("5. Scraping feats 2024...");
        // var feats2024 = await FeatScraper.ScrapeFeats2024();
        // var featsJson2024 = JsonSerializer.Serialize(feats2024, new JsonSerializerOptions { WriteIndented = true });
        // File.WriteAllText("..\\DndInator\\wwwroot\\data\\2024\\feats.json", featsJson2024);
        // Console.WriteLine($"✓ Saved {feats2024.Count} feats to feats.json\n");
        
        // Scrape magic items
        Console.WriteLine("6. Scraping magic items 2024...");
        var magicItems2024 = await MagicItemScraper.ScrapeMagicItems2024();
        var magicItemsJson2024 = JsonSerializer.Serialize(magicItems2024, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("..\\DndInator\\wwwroot\\data\\2024\\magic-items.json", magicItemsJson2024);
        Console.WriteLine($"✓ Saved {magicItems2024.Count} magic items to magic-items.json\n");

        Console.WriteLine("=== Scraping Complete ===");
    }
}
