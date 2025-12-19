using HtmlAgilityPack;
using DndShared.Models;
using System.Text.RegularExpressions;

namespace DndScraper.Helpers;

public class PoisonScraper
{
    public static async Task<List<Poison>> ScrapePoisons2024()
    {
        string poisonUrl = "http://dnd2024.wikidot.com/equipment:poison";
        var poisonList = new List<Poison>();

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

            try
            {
                // Hent hovesiden med listen over poisons
                var html = await client.GetStringAsync(poisonUrl);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                // Find tabellen på siden
                var table = htmlDoc.DocumentNode.SelectSingleNode("//table[@class='wiki-content-table']");
                
                if (table == null)
                {
                    Console.WriteLine("No table found");
                    return poisonList;
                }

                Console.WriteLine($"\n=== Scraping poisons from {poisonUrl} ===");

                var rows = table.SelectNodes(".//tr");
                
                if (rows == null || rows.Count < 2)
                {
                    Console.WriteLine($"No rows found in table");
                    return poisonList;
                }

                // Skip header row
                foreach (var row in rows.Skip(1))
                {
                    var cells = row.SelectNodes("td");
                    if (cells == null || cells.Count < 4) continue;

                    var poison = new Poison
                    {
                        Name = cells[0].InnerText.Trim(),
                        Type = cells[1].InnerText.Trim(),
                        Cost = cells[2].InnerText.Trim(),
                        Effect = cells[3].InnerText.Trim()
                    };

                    poisonList.Add(poison);
                    Console.WriteLine($"Found poison: {poison.Name} ({poison.Type}, {poison.Cost})");
                }

                Console.WriteLine($"\n=== Total poisons found: {poisonList.Count} ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scraping poisons: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        return poisonList;
    }
}
