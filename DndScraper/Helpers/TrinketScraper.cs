using HtmlAgilityPack;
using DndShared.Models;
using System.Text.RegularExpressions;

namespace DndScraper.Helpers;

public class TrinketScraper
{
    private const int DelayMs = 800;

    public static async Task<List<Trinket>> ScrapeTrinkets2014()
    {
        var candidateUrls = new[]
        {
            "https://dnd5e.wikidot.com/trinkets",
            "https://dnd5e.wikidot.com/equipment:trinket"
        };

        var trinketList = new List<Trinket>();

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

            foreach (var trinketUrl in candidateUrls)
            {
                try
                {
                    Console.WriteLine($"\n=== Scraping trinkets from {trinketUrl} ===");

                    var html = await client.GetStringAsync(trinketUrl);
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
                        if (cells == null || cells.Count < 2) continue;

                        var trinket = new Trinket
                        {
                            Roll = cells[0].InnerText.Trim(),
                            Description = cells[1].InnerText.Trim()
                        };

                        trinketList.Add(trinket);
                        Console.WriteLine($"Found trinket: {trinket.Roll} - {trinket.Description}");
                    }

                    Console.WriteLine($"\n=== Total trinkets found: {trinketList.Count} ===");

                    if (trinketList.Count > 0)
                    {
                        await Task.Delay(DelayMs);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error scraping trinkets from {trinketUrl}: {ex.Message}");
                }
            }
        }

        return trinketList;
    }

    public static async Task<List<Trinket>> ScrapeTrinkets2024()
    {
        string trinketUrl = "http://dnd2024.wikidot.com/equipment:trinket";
        var trinketList = new List<Trinket>();

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

            try
            {
                // Hent hovesiden med listen over trinkets
                var html = await client.GetStringAsync(trinketUrl);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                // Find tabellen på siden
                var table = htmlDoc.DocumentNode.SelectSingleNode("//table[@class='wiki-content-table']");
                
                if (table == null)
                {
                    Console.WriteLine("No table found");
                    return trinketList;
                }

                Console.WriteLine($"\n=== Scraping trinkets from {trinketUrl} ===");

                var rows = table.SelectNodes(".//tr");
                
                if (rows == null || rows.Count < 2)
                {
                    Console.WriteLine($"No rows found in table");
                    return trinketList;
                }

                // Skip header row
                foreach (var row in rows.Skip(1))
                {
                    var cells = row.SelectNodes("td");
                    if (cells == null || cells.Count < 2) continue;

                    var trinket = new Trinket
                    {
                        Roll = cells[0].InnerText.Trim(),
                        Description = cells[1].InnerText.Trim()
                    };

                    trinketList.Add(trinket);
                    Console.WriteLine($"Found trinket: {trinket.Roll} - {trinket.Description}");
                }

                Console.WriteLine($"\n=== Total trinkets found: {trinketList.Count} ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scraping trinkets: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        return trinketList;
    }
}
