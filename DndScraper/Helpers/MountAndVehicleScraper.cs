using HtmlAgilityPack;
using DndShared.Models;
using System.Text.RegularExpressions;

namespace DndScraper.Helpers;

public class MountAndVehicleScraper
{
    private const int DelayMs = 800;

    public static async Task<List<MountAndVehicle>> ScrapeMountsAndVehicles2014()
    {
        var candidateUrls = new[]
        {
            "https://dnd5e.wikidot.com/mounts-vehicles",
            "https://dnd5e.wikidot.com/equipment:mounts-and-vehicles"
        };

        var mountsAndVehicles = new List<MountAndVehicle>();

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

            foreach (var url in candidateUrls)
            {
                try
                {
                    Console.WriteLine($"\n=== Scraping mounts and vehicles from {url} ===");

                    var html = await client.GetStringAsync(url);
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
                        string category = "Unknown";
                        var prevNode = table.PreviousSibling;
                        while (prevNode != null)
                        {
                            if (prevNode.Name == "h1" || prevNode.Name == "h2" || prevNode.Name == "h3" || 
                                prevNode.Name == "h4" || prevNode.Name == "h5" || prevNode.Name == "h6")
                            {
                                var headerText = prevNode.InnerText.Trim();

                                if (headerText.Contains("Mounts"))
                                {
                                    category = "Mounts and Animals";
                                }
                                else if (headerText.Contains("Saddles"))
                                {
                                    category = "Saddles";
                                }
                                else if (headerText.Contains("Tack"))
                                {
                                    category = "Tack and Drawn Vehicles";
                                }
                                else if (headerText.Contains("Airborne") || headerText.Contains("Waterborne"))
                                {
                                    category = "Airborne and Waterborne Vehicles";
                                }
                                else if (headerText.Contains("Eberron"))
                                {
                                    category = "Eberron Airships";
                                }

                                break;
                            }
                            prevNode = prevNode.PreviousSibling;
                        }

                        var rows = table.SelectNodes(".//tr");

                        if (rows == null || rows.Count < 2)
                        {
                            Console.WriteLine($"No rows found in table for {category}");
                            continue;
                        }

                        var headers = rows[0].SelectNodes("th");
                        if (headers == null || headers.Count == 0) continue;

                        foreach (var row in rows.Skip(1))
                        {
                            var cells = row.SelectNodes("td");
                            if (cells == null) continue;

                            var item = new MountAndVehicle
                            {
                                Category = category
                            };

                            if (category == "Mounts and Animals" && cells.Count >= 3)
                            {
                                item.Name = cells[0].InnerText.Trim();
                                item.CarryingCapacity = cells[1].InnerText.Trim();
                                item.Cost = cells[2].InnerText.Trim();
                            }
                            else if (category == "Saddles" && cells.Count >= 3)
                            {
                                item.Name = cells[0].InnerText.Trim();
                                item.Weight = cells[1].InnerText.Trim();
                                item.Cost = cells[2].InnerText.Trim();
                            }
                            else if (category == "Tack and Drawn Vehicles" && cells.Count >= 3)
                            {
                                item.Name = cells[0].InnerText.Trim();
                                item.Weight = cells[1].InnerText.Trim();
                                item.Cost = cells[2].InnerText.Trim();
                            }
                            else if ((category == "Airborne and Waterborne Vehicles" || category == "Eberron Airships") && cells.Count >= 8)
                            {
                                item.Name = cells[0].InnerText.Trim();
                                item.Speed = cells[1].InnerText.Trim();
                                item.Crew = cells[2].InnerText.Trim();
                                item.Passengers = cells[3].InnerText.Trim();
                                item.Cargo = cells[4].InnerText.Trim();
                                item.AC = cells[5].InnerText.Trim();
                                item.HP = cells[6].InnerText.Trim();
                                item.DamageThreshold = cells[7].InnerText.Trim();
                                item.Cost = cells[8].InnerText.Trim();
                            }
                            else
                            {
                                continue;
                            }

                            if (item.Weight == "—" || item.Weight == "-") item.Weight = null;
                            if (item.DamageThreshold == "—" || item.DamageThreshold == "-") item.DamageThreshold = null;
                            if (item.Passengers == "—" || item.Passengers == "-") item.Passengers = null;
                            if (item.Cargo == "—" || item.Cargo == "-") item.Cargo = null;

                            mountsAndVehicles.Add(item);
                            Console.WriteLine($"Found: {item.Name} ({item.Category}, {item.Cost})");
                        }
                    }

                    Console.WriteLine($"\n=== Total mounts and vehicles found: {mountsAndVehicles.Count} ===");

                    if (mountsAndVehicles.Count > 0)
                    {
                        await Task.Delay(DelayMs);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error scraping mounts and vehicles from {url}: {ex.Message}");
                }
            }
        }

        return mountsAndVehicles;
    }

    public static async Task<List<MountAndVehicle>> ScrapeMountsAndVehicles2024()
    {
        string url = "http://dnd2024.wikidot.com/equipment:mounts-and-vehicles";
        var mountsAndVehicles = new List<MountAndVehicle>();

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

            try
            {
                // Hent hovesiden med listen over mounts og vehicles
                var html = await client.GetStringAsync(url);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                // Find alle tabeller på siden
                var tables = htmlDoc.DocumentNode.SelectNodes("//table[@class='wiki-content-table']");
                
                if (tables == null)
                {
                    Console.WriteLine("No tables found");
                    return mountsAndVehicles;
                }

                Console.WriteLine($"\n=== Scraping mounts and vehicles from {url} ===");

                foreach (var table in tables)
                {
                    // Find category fra heading før tabellen
                    string category = "Unknown";
                    var prevNode = table.PreviousSibling;
                    while (prevNode != null)
                    {
                        if (prevNode.Name == "h1" || prevNode.Name == "h2" || prevNode.Name == "h3" || 
                            prevNode.Name == "h4" || prevNode.Name == "h5" || prevNode.Name == "h6")
                        {
                            var headerText = prevNode.InnerText.Trim();
                            
                            if (headerText.Contains("Mounts and Other Animals"))
                            {
                                category = "Mounts and Animals";
                            }
                            else if (headerText.Contains("Saddles"))
                            {
                                category = "Saddles";
                            }
                            else if (headerText.Contains("Tack, Harness, and Drawn Vehicles"))
                            {
                                category = "Tack and Drawn Vehicles";
                            }
                            else if (headerText.Contains("Airborne and Waterborne Vehicles"))
                            {
                                category = "Airborne and Waterborne Vehicles";
                            }
                            else if (headerText.Contains("Eberron"))
                            {
                                category = "Eberron Airships";
                            }
                            
                            break;
                        }
                        prevNode = prevNode.PreviousSibling;
                    }

                    Console.WriteLine($"\n--- Processing: {category} ---");

                    var rows = table.SelectNodes(".//tr");
                    
                    if (rows == null || rows.Count < 2)
                    {
                        Console.WriteLine($"No rows found in table");
                        continue;
                    }

                    // Check header row to determine table type
                    var headers = rows[0].SelectNodes("th");
                    if (headers == null || headers.Count == 0) continue;

                    var firstHeader = headers[0].InnerText.Trim();

                    // Skip header row and process data
                    foreach (var row in rows.Skip(1))
                    {
                        var cells = row.SelectNodes("td");
                        if (cells == null) continue;

                        var item = new MountAndVehicle
                        {
                            Category = category
                        };

                        // Parse based on table structure
                        if (category == "Mounts and Animals" && cells.Count >= 3)
                        {
                            // Item | Carrying Capacity | Cost
                            item.Name = cells[0].InnerText.Trim();
                            item.CarryingCapacity = cells[1].InnerText.Trim();
                            item.Cost = cells[2].InnerText.Trim();
                        }
                        else if (category == "Saddles" && cells.Count >= 3)
                        {
                            // Saddle | Weight | Cost
                            item.Name = cells[0].InnerText.Trim();
                            item.Weight = cells[1].InnerText.Trim();
                            item.Cost = cells[2].InnerText.Trim();
                        }
                        else if (category == "Tack and Drawn Vehicles" && cells.Count >= 3)
                        {
                            // Item | Weight | Cost
                            item.Name = cells[0].InnerText.Trim();
                            item.Weight = cells[1].InnerText.Trim();
                            item.Cost = cells[2].InnerText.Trim();
                        }
                        else if ((category == "Airborne and Waterborne Vehicles" || category == "Eberron Airships") && cells.Count >= 8)
                        {
                            // Ship | Speed | Crew | Passengers | Cargo (Tons) | AC | HP | Damage Threshold | Cost
                            item.Name = cells[0].InnerText.Trim();
                            item.Speed = cells[1].InnerText.Trim();
                            item.Crew = cells[2].InnerText.Trim();
                            item.Passengers = cells[3].InnerText.Trim();
                            item.Cargo = cells[4].InnerText.Trim();
                            item.AC = cells[5].InnerText.Trim();
                            item.HP = cells[6].InnerText.Trim();
                            item.DamageThreshold = cells[7].InnerText.Trim();
                            item.Cost = cells[8].InnerText.Trim();
                        }
                        else
                        {
                            continue;
                        }

                        // Rens data
                        if (item.Weight == "—" || item.Weight == "-") item.Weight = null;
                        if (item.DamageThreshold == "—" || item.DamageThreshold == "-") item.DamageThreshold = null;
                        if (item.Passengers == "—" || item.Passengers == "-") item.Passengers = null;
                        if (item.Cargo == "—" || item.Cargo == "-") item.Cargo = null;

                        mountsAndVehicles.Add(item);
                        Console.WriteLine($"Found: {item.Name} ({item.Category}, {item.Cost})");
                    }
                }

                Console.WriteLine($"\n=== Total mounts and vehicles found: {mountsAndVehicles.Count} ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scraping mounts and vehicles: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        return mountsAndVehicles;
    }
}
