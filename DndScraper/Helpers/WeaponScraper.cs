using HtmlAgilityPack;
using DndShared.Models;
using System.Text.RegularExpressions;

namespace DndScraper.Helpers;

public class WeaponScraper
{
    private const int DelayMs = 800;

    public static async Task<List<Weapon>> ScrapeWeapons2014()
    {
        var candidateUrls = new[]
        {
            "https://dnd5e.wikidot.com/weapons",
            "https://dnd5e.wikidot.com/equipment:weapon"
        };

        var weaponList = new List<Weapon>();

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

            foreach (var weaponUrl in candidateUrls)
            {
                try
                {
                    Console.WriteLine($"\n=== Scraping weapons from {weaponUrl} ===");

                    var html = await client.GetStringAsync(weaponUrl);
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(html);

                    var tables = htmlDoc.DocumentNode.SelectNodes("//table[@class='wiki-content-table']");

                    if (tables == null)
                    {
                        Console.WriteLine("No tables found");
                        continue;
                    }

                    foreach (var table in tables)
                    {
                        string currentCategory = "Unknown";
                        string currentType = "Unknown";

                        var firstRow = table.SelectSingleNode(".//tr[1]");
                        if (firstRow != null)
                        {
                            var headerText = firstRow.InnerText.Trim();

                            if (headerText.Contains("Simple Melee", StringComparison.OrdinalIgnoreCase))
                            {
                                currentCategory = "Simple";
                                currentType = "Melee";
                            }
                            else if (headerText.Contains("Simple Ranged", StringComparison.OrdinalIgnoreCase))
                            {
                                currentCategory = "Simple";
                                currentType = "Ranged";
                            }
                            else if (headerText.Contains("Martial Melee", StringComparison.OrdinalIgnoreCase))
                            {
                                currentCategory = "Martial";
                                currentType = "Melee";
                            }
                            else if (headerText.Contains("Martial Ranged", StringComparison.OrdinalIgnoreCase))
                            {
                                currentCategory = "Martial";
                                currentType = "Ranged";
                            }
                        }

                        if (currentCategory == "Unknown" || currentType == "Unknown")
                        {
                            var prevNode = table.PreviousSibling;
                            bool foundH1 = false;

                            while (prevNode != null && !foundH1)
                            {
                                if (prevNode.NodeType == HtmlNodeType.Element && prevNode.Name.ToLower() == "h1")
                                {
                                    var h1Text = prevNode.InnerText.Trim();

                                    if (h1Text.Contains("Simple Weapons", StringComparison.OrdinalIgnoreCase))
                                    {
                                        currentCategory = "Simple";
                                    }
                                    else if (h1Text.Contains("Martial Weapons", StringComparison.OrdinalIgnoreCase))
                                    {
                                        currentCategory = "Martial";
                                    }

                                    foundH1 = true;
                                }
                                prevNode = prevNode.PreviousSibling;
                            }
                        }

                        var headers = table.SelectNodes(".//tr[1]/th");
                        if (headers != null && headers.Count > 0)
                        {
                            var firstHeader = headers[0].InnerText.Trim();
                            if (firstHeader == "Type")
                            {
                                continue; // ammunition table
                            }
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

                            if (weapon.Properties == "—" || weapon.Properties == "-") weapon.Properties = null;
                            if (weapon.Weight == "—" || weapon.Weight == "-") weapon.Weight = null;

                            weaponList.Add(weapon);
                            Console.WriteLine($"Found weapon: {weapon.Name} (Damage: {weapon.Damage}, {weapon.Category} {weapon.Type})");
                        }
                    }

                    Console.WriteLine($"\n=== Total weapons found: {weaponList.Count} ===");

                    if (weaponList.Count > 0)
                    {
                        await Task.Delay(DelayMs);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error scraping weapons from {weaponUrl}: {ex.Message}");
                }
            }
        }

        return weaponList;
    }

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

                // Loop through hver tabel
                foreach (var table in tables)
                {
                    // Nulstil for hver tabel
                    string currentCategory = "Unknown";
                    string currentType = "Unknown";
                    
                    // Tjek først for table caption eller header row som har type information
                    var firstRow = table.SelectSingleNode(".//tr[1]");
                    if (firstRow != null)
                    {
                        var headerText = firstRow.InnerText.Trim();
                        
                        // Parse fra header row
                        if (headerText.Contains("Simple Melee", StringComparison.OrdinalIgnoreCase))
                        {
                            currentCategory = "Simple";
                            currentType = "Melee";
                        }
                        else if (headerText.Contains("Simple Ranged", StringComparison.OrdinalIgnoreCase))
                        {
                            currentCategory = "Simple";
                            currentType = "Ranged";
                        }
                        else if (headerText.Contains("Martial Melee", StringComparison.OrdinalIgnoreCase))
                        {
                            currentCategory = "Martial";
                            currentType = "Melee";
                        }
                        else if (headerText.Contains("Martial Ranged", StringComparison.OrdinalIgnoreCase))
                        {
                            currentCategory = "Martial";
                            currentType = "Ranged";
                        }
                    }
                    
                    // Hvis vi stadig ikke har fundet kategori og type, søg baglæns efter h1
                    if (currentCategory == "Unknown" || currentType == "Unknown")
                    {
                        var prevNode = table.PreviousSibling;
                        bool foundH1 = false;
                        
                        while (prevNode != null && !foundH1)
                        {
                            if (prevNode.NodeType == HtmlNodeType.Element && prevNode.Name.ToLower() == "h1")
                            {
                                var h1Text = prevNode.InnerText.Trim();
                                
                                if (h1Text.Contains("Simple Weapons", StringComparison.OrdinalIgnoreCase))
                                {
                                    currentCategory = "Simple";
                                }
                                else if (h1Text.Contains("Martial Weapons", StringComparison.OrdinalIgnoreCase))
                                {
                                    currentCategory = "Martial";
                                }
                                
                                foundH1 = true;
                            }
                            prevNode = prevNode.PreviousSibling;
                        }
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
