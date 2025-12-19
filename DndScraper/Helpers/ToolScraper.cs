using HtmlAgilityPack;
using DndShared.Models;
using System.Text.RegularExpressions;

namespace DndScraper.Helpers;

public class ToolScraper
{
    public static async Task<List<Tool>> ScrapeTools2024()
    {
        string toolUrl = "http://dnd2024.wikidot.com/equipment:tool";
        var toolList = new List<Tool>();

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

            try
            {
                // Hent hovesiden med listen over tools
                var html = await client.GetStringAsync(toolUrl);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                // Find alle tabeller på siden
                var tables = htmlDoc.DocumentNode.SelectNodes("//table[@class='wiki-content-table']");
                
                if (tables == null)
                {
                    Console.WriteLine("No tables found");
                    return toolList;
                }

                Console.WriteLine($"\n=== Scraping tools from {toolUrl} ===");

                foreach (var table in tables)
                {
                    // Find category fra header row
                    var headerRow = table.SelectSingleNode(".//tr[1]");
                    if (headerRow == null) continue;

                    var headers = headerRow.SelectNodes("th");
                    if (headers == null || headers.Count < 4) continue;

                    var firstHeader = headers[0].InnerText.Trim();
                    string category = "Unknown";

                    if (firstHeader.Contains("Artisan"))
                    {
                        category = "Artisan Tools";
                    }
                    else if (firstHeader.Contains("Other"))
                    {
                        category = "Other Tools";
                    }
                    else if (firstHeader.Contains("Gaming"))
                    {
                        category = "Gaming Sets";
                    }
                    else if (firstHeader.Contains("Musical"))
                    {
                        category = "Musical Instruments";
                    }

                    Console.WriteLine($"\n--- Processing: {category} ---");

                    var rows = table.SelectNodes(".//tr");
                    
                    if (rows == null || rows.Count < 2)
                    {
                        Console.WriteLine($"No rows found in table");
                        continue;
                    }

                    // Skip header row
                    foreach (var row in rows.Skip(1))
                    {
                        var cells = row.SelectNodes("td");
                        if (cells == null || cells.Count < 4) continue;

                        var tool = new Tool
                        {
                            Category = category,
                            Name = cells[0].InnerText.Trim(),
                            Ability = cells[1].InnerText.Trim(),
                            Weight = cells[2].InnerText.Trim(),
                            Cost = cells[3].InnerText.Trim()
                        };

                        // Rens data
                        if (tool.Weight == "—" || tool.Weight == "-") tool.Weight = null;

                        toolList.Add(tool);
                        Console.WriteLine($"Found tool: {tool.Name} ({tool.Category}, {tool.Cost})");
                    }
                }

                Console.WriteLine($"\n=== Total tools found: {toolList.Count} ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scraping tools: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        return toolList;
    }
}
