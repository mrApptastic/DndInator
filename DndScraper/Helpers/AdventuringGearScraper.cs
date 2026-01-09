using HtmlAgilityPack;
using DndShared.Models;
using System.Text.RegularExpressions;

namespace DndScraper.Helpers;

public class AdventuringGearScraper
{
    private const int DelayMs = 800; // lidt pænere mod serveren

    public static async Task<List<AdventuringGear>> ScrapeAdventuringGear2014()
    {
        var candidateUrls = new[]
        {
            "https://dnd5e.wikidot.com/adventuring-gear",
            "https://dnd5e.wikidot.com/equipment:adventuring-gear"
        };

        var gearList = new List<AdventuringGear>();

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

            foreach (var gearUrl in candidateUrls)
            {
                try
                {
                    Console.WriteLine($"\n=== Scraping adventuring gear from {gearUrl} ===");

                    var html = await client.GetStringAsync(gearUrl);
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(html);

                    var table = htmlDoc.DocumentNode.SelectSingleNode("//table[@class='wiki-content-table']");

                    if (table == null)
                    {
                        Console.WriteLine("No table found");
                        continue;
                    }

                    var rows = table.SelectNodes(".//tr");

                    if (rows == null || rows.Count < 2)
                    {
                        Console.WriteLine("No rows found in table");
                        continue;
                    }

                    foreach (var row in rows.Skip(1))
                    {
                        var cells = row.SelectNodes("td");
                        if (cells == null || cells.Count < 4) continue;

                        var gear = new AdventuringGear
                        {
                            Name = cells[0].InnerText.Trim(),
                            Weight = cells[1].InnerText.Trim(),
                            Cost = cells[2].InnerText.Trim(),
                            Function = cells[3].InnerText.Trim()
                        };

                        if (gear.Weight == "—" || gear.Weight == "-") gear.Weight = null;

                        gearList.Add(gear);
                        Console.WriteLine($"Found gear: {gear.Name} (Cost: {gear.Cost})");
                    }

                    Console.WriteLine($"\n=== Total adventuring gear found: {gearList.Count} ===");

                    if (gearList.Count > 0)
                    {
                        await Task.Delay(DelayMs);
                        break; // success, undgå flere kandidater
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error scraping adventuring gear from {gearUrl}: {ex.Message}");
                }
            }
        }

        return gearList;
    }

    public static async Task<List<AdventuringGear>> ScrapeAdventuringGear2024()
    {
        string gearUrl = "http://dnd2024.wikidot.com/equipment:adventuring-gear";
        var gearList = new List<AdventuringGear>();

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

            try
            {
                // Hent hovesiden med listen over adventuring gear
                var html = await client.GetStringAsync(gearUrl);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                // Find tabellen på siden
                var table = htmlDoc.DocumentNode.SelectSingleNode("//table[@class='wiki-content-table']");
                
                if (table == null)
                {
                    Console.WriteLine("No table found");
                    return gearList;
                }

                Console.WriteLine($"\n=== Scraping adventuring gear from {gearUrl} ===");

                var rows = table.SelectNodes(".//tr");
                
                if (rows == null || rows.Count < 2)
                {
                    Console.WriteLine($"No rows found in table");
                    return gearList;
                }

                // Skip header row
                foreach (var row in rows.Skip(1))
                {
                    var cells = row.SelectNodes("td");
                    if (cells == null || cells.Count < 4) continue;

                    var gear = new AdventuringGear
                    {
                        Name = cells[0].InnerText.Trim(),
                        Weight = cells[1].InnerText.Trim(),
                        Cost = cells[2].InnerText.Trim(),
                        Function = cells[3].InnerText.Trim()
                    };

                    // Rens data
                    if (gear.Weight == "—" || gear.Weight == "-") gear.Weight = null;
                    
                    gearList.Add(gear);
                    Console.WriteLine($"Found gear: {gear.Name} (Cost: {gear.Cost})");
                }

                Console.WriteLine($"\n=== Total adventuring gear found: {gearList.Count} ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scraping adventuring gear: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        return gearList;
    }
}
