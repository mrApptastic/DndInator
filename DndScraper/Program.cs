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
        Console.WriteLine("1. Scraping spells...");
        var spells = await SpellScraper.ScrapeSpells();
        var spellsJson = JsonSerializer.Serialize(spells, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("..\\DndInator\\wwwroot\\data\\spells.json", spellsJson);
        Console.WriteLine($"✓ Saved {spells.Count} spells to spells.json\n");
        
        // Scrape lineages
        Console.WriteLine("2. Scraping lineages...");
        var lineages = await LineageScraper.ScrapeLineages();
        var lineagesJson = JsonSerializer.Serialize(lineages, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("..\\DndInator\\wwwroot\\data\\lineages.json", lineagesJson);
        Console.WriteLine($"✓ Saved {lineages.Count} lineages to lineages.json\n");
        
        Console.WriteLine("=== Scraping Complete ===");
    }
}
