using HtmlAgilityPack;
using DndShared.Models;
using System.Text.RegularExpressions;

namespace DndScraper.Helpers;

public class ArmorScraper
{
    public static async Task<List<Armor>> ScrapeArmor2024()
    {
        string armorUrl = "http://dnd2024.wikidot.com/equipment:armor";
        var armorList = new List<Armor>();

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

            try
            {
                // Hent hovesiden med listen over armor
                var html = await client.GetStringAsync(armorUrl);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                // Find alle tabeller på siden
                var tables = htmlDoc.DocumentNode.SelectNodes("//table[@class='wiki-content-table']");
                
                if (tables == null)
                {
                    Console.WriteLine("No tables found");
                    return armorList;
                }

                Console.WriteLine($"\n=== Scraping armor from {armorUrl} ===");

                // Loop through hver tabel
                foreach (var table in tables)
                {
                    // Find category fra heading før tabellen
                    string category = "Unknown";
                    string donTime = "";
                    string doffTime = "";
                    
                    var prevNode = table.PreviousSibling;
                    while (prevNode != null)
                    {
                        if (prevNode.Name == "h6")
                        {
                            var categoryText = prevNode.InnerText.Trim();
                            
                            // Parse category og don/doff time
                            if (categoryText.Contains("Light Armor"))
                            {
                                category = "Light Armor";
                                donTime = "1 Minute";
                                doffTime = "1 Minute";
                            }
                            else if (categoryText.Contains("Medium Armor"))
                            {
                                category = "Medium Armor";
                                donTime = "5 Minutes";
                                doffTime = "1 Minute";
                            }
                            else if (categoryText.Contains("Heavy Armor"))
                            {
                                category = "Heavy Armor";
                                donTime = "10 Minutes";
                                doffTime = "5 Minutes";
                            }
                            else if (categoryText.Contains("Shield"))
                            {
                                category = "Shield";
                                donTime = "Utilize Action";
                                doffTime = "Utilize Action";
                            }
                            
                            break;
                        }
                        prevNode = prevNode.PreviousSibling;
                    }

                    Console.WriteLine($"\n--- Processing category: {category} ---");

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
                        if (cells == null || cells.Count < 6) continue;

                        var armor = new Armor
                        {
                            Category = category,
                            DonTime = donTime,
                            DoffTime = doffTime,
                            Name = cells[0].InnerText.Trim(),
                            ArmorClass = cells[1].InnerText.Trim(),
                            Strength = cells[2].InnerText.Trim(),
                            Stealth = cells[3].InnerText.Trim(),
                            Weight = cells[4].InnerText.Trim(),
                            Cost = cells[5].InnerText.Trim()
                        };

                        // Rens data
                        if (armor.Strength == "-") armor.Strength = null;
                        if (armor.Stealth == "-") armor.Stealth = null;

                        armorList.Add(armor);
                        Console.WriteLine($"Found armor: {armor.Name} (AC: {armor.ArmorClass}, Category: {armor.Category})");
                    }
                }

                Console.WriteLine($"\n=== Total armor found: {armorList.Count} ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scraping armor: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        return armorList;
    }
}
