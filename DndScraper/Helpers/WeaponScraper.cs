using HtmlAgilityPack;
using DndShared.Models;
using System.Text.RegularExpressions;

namespace DndScraper.Helpers;

public class WeaponScraper
{
    public static async Task<List<Weapon>> ScrapeWeapons2024()
    {
        string weaponUrl = "http://dnd2024.wikidot.com/equipment:weapon";
        var weaponList = new List<Weapon>();

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

            try
            {
                // Hent hovesiden med listen over weapons
                var html = await client.GetStringAsync(weaponUrl);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                // Find alle tabeller på siden
                var tables = htmlDoc.DocumentNode.SelectNodes("//table[@class='wiki-content-table']");
                
                if (tables == null)
                {
                    Console.WriteLine("No tables found");
                    return weaponList;
                }

                Console.WriteLine($"\n=== Scraping weapons from {weaponUrl} ===");

                string currentCategory = "Unknown";
                string currentType = "Unknown";

                // Loop through hver tabel
                foreach (var table in tables)
                {
                    // Find category og type fra heading før tabellen
                    var prevNode = table.PreviousSibling;
                    while (prevNode != null)
                    {
                        if (prevNode.Name == "h1" || prevNode.Name == "h2" || prevNode.Name == "h3" || 
                            prevNode.Name == "h4" || prevNode.Name == "h5" || prevNode.Name == "h6" ||
                            prevNode.Name == "p")
                        {
                            var headerText = prevNode.InnerText.Trim();
                            
                            // Parse category
                            if (headerText.Contains("Simple Weapons"))
                            {
                                currentCategory = "Simple";
                            }
                            else if (headerText.Contains("Martial Weapons"))
                            {
                                currentCategory = "Martial";
                            }
                            
                            // Parse type (Melee eller Ranged)
                            if (headerText.Contains("Melee"))
                            {
                                currentType = "Melee";
                                break;
                            }
                            else if (headerText.Contains("Ranged"))
                            {
                                currentType = "Ranged";
                                break;
                            }
                        }
                        prevNode = prevNode.PreviousSibling;
                    }

                    // Tjek om det er ammunition-tabellen
                    var headers = table.SelectNodes(".//tr[1]/th");
                    if (headers != null && headers.Count > 0)
                    {
                        var firstHeader = headers[0].InnerText.Trim();
                        if (firstHeader == "Type")
                        {
                            // Dette er ammunition-tabellen, skip den
                            continue;
                        }
                    }

                    Console.WriteLine($"\n--- Processing: {currentCategory} {currentType} Weapons ---");

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

                        var weapon = new Weapon
                        {
                            Category = currentCategory,
                            Type = currentType,
                            Name = cells[0].InnerText.Trim(),
                            Damage = cells[1].InnerText.Trim(),
                            Properties = cells[2].InnerText.Trim(),
                            Mastery = cells[3].InnerText.Trim(),
                            Weight = cells[4].InnerText.Trim(),
                            Cost = cells[5].InnerText.Trim()
                        };

                        // Rens data
                        if (weapon.Properties == "—" || weapon.Properties == "-") weapon.Properties = null;
                        if (weapon.Weight == "—" || weapon.Weight == "-") weapon.Weight = null;

                        weaponList.Add(weapon);
                        Console.WriteLine($"Found weapon: {weapon.Name} (Damage: {weapon.Damage}, {weapon.Category} {weapon.Type})");
                    }
                }

                Console.WriteLine($"\n=== Total weapons found: {weaponList.Count} ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scraping weapons: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        return weaponList;
    }
}
