using HtmlAgilityPack;
using DndShared.Models;
using System.Text.RegularExpressions;

namespace DndScraper.Helpers;

public class SpeciesScraper
{
    public static async Task<List<Species>> ScrapeSpecies2024()
    {
        string speciesUrl = "http://dnd2024.wikidot.com/species:all";
        var speciesList = new List<Species>();

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

            try
            {
                // Hent hovesiden med listen over species
                var html = await client.GetStringAsync(speciesUrl);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                // Find alle tabs (kategorier)
                var tabNavigation = htmlDoc.DocumentNode.SelectSingleNode("//ul[@class='yui-nav']");
                var tabs = tabNavigation?.SelectNodes(".//li");
                
                if (tabs == null)
                {
                    Console.WriteLine("No tabs found");
                    return speciesList;
                }

                // Find yui-content container
                var yuiContent = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='yui-content']");
                if (yuiContent == null)
                {
                    Console.WriteLine("No yui-content found");
                    return speciesList;
                }

                // Loop through hver tab (kategori)
                for (int tabIndex = 0; tabIndex < tabs.Count; tabIndex++)
                {
                    var categoryName = tabs[tabIndex].InnerText.Trim();
                    var tabId = $"wiki-tab-0-{tabIndex}";
                    var tabDiv = yuiContent.SelectSingleNode($".//div[@id='{tabId}']");
                    
                    if (tabDiv == null)
                    {
                        Console.WriteLine($"Tab {tabId} not found");
                        continue;
                    }

                    Console.WriteLine($"\n=== Scraping category: {categoryName} ===");

                    var rows = tabDiv.SelectNodes(".//table[@class='wiki-content-table']//tr");
                    
                    if (rows == null)
                    {
                        Console.WriteLine($"No rows found in category {categoryName}");
                        continue;
                    }

                    // Skip header row
                    foreach (var row in rows.Skip(1))
                    {
                        var cells = row.SelectNodes("td");
                        if (cells == null || cells.Count < 1) continue;

                        var species = new Species();
                        species.Category = categoryName;

                        // Parse species name og URL til detaljesiden
                        var nameLink = cells[0].SelectSingleNode(".//a");
                        string? detailUrl = null;
                        if (nameLink != null)
                        {
                            species.Name = nameLink.InnerText.Trim();
                            detailUrl = "http://dnd2024.wikidot.com" + nameLink.GetAttributeValue("href", "");
                        }

                        speciesList.Add(species);
                        Console.WriteLine($"Found species: {species.Name} (Category: {categoryName})");
                        
                        // Scrape detaljer fra detaljesiden
                        if (detailUrl != null)
                        {
                            await ScrapeSpeciesDetails(client, species, detailUrl);
                            await Task.Delay(500); // Vær pæn mod serveren
                        }
                    }
                }

                Console.WriteLine($"\n=== Total species found: {speciesList.Count} ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scraping species: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        return speciesList;
    }

    private static async Task ScrapeSpeciesDetails(HttpClient client, Species species, string url)
    {
        try
        {
            var html = await client.GetStringAsync(url);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var pageContent = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='page-content']");
            if (pageContent == null)
            {
                Console.WriteLine($"  ✗ No page-content found for {species.Name}");
                return;
            }

            // Gem hele HTML-indholdet
            species.FullContent = pageContent.InnerHtml;

            // Parse Source
            var paragraphs = pageContent.SelectNodes(".//p");
            if (paragraphs != null && paragraphs.Count > 0)
            {
                // Første paragraf er normalt Source
                var firstParagraphText = paragraphs[0].InnerText.Trim();
                if (firstParagraphText.StartsWith("Source:"))
                {
                    species.Source = firstParagraphText.Replace("Source:", "").Trim();
                }

                // Saml beskrivelsen (paragraffer indtil vi finder Creature Type)
                var descriptionParagraphs = new List<string>();
                foreach (var p in paragraphs.Skip(1))
                {
                    var text = p.InnerText.Trim();
                    
                    // Stop ved første strong tag med detaljer
                    if (text.Contains("Creature Type:") || text.Contains("Size:") || text.Contains("Speed:"))
                    {
                        // Parse species detaljer
                        var detailsText = text;

                        // Parse Creature Type
                        var creatureMatch = Regex.Match(detailsText, @"Creature Type:\s*([^\n]+)");
                        if (creatureMatch.Success)
                        {
                            species.CreatureType = creatureMatch.Groups[1].Value.Trim();
                        }

                        // Parse Size
                        var sizeMatch = Regex.Match(detailsText, @"Size:\s*([^\n]+)");
                        if (sizeMatch.Success)
                        {
                            species.Size = sizeMatch.Groups[1].Value.Trim();
                        }

                        // Parse Speed
                        var speedMatch = Regex.Match(detailsText, @"Speed:\s*([^\n]+)");
                        if (speedMatch.Success)
                        {
                            species.Speed = speedMatch.Groups[1].Value.Trim();
                        }

                        break;
                    }
                    else if (!text.StartsWith("Source:"))
                    {
                        descriptionParagraphs.Add(text);
                    }
                }

                species.Description = string.Join("\n\n", descriptionParagraphs);

                // Parse traits (normalt liste items efter detaljer)
                var listItems = pageContent.SelectNodes(".//ul/li | .//ol/li");
                if (listItems != null)
                {
                    species.Traits = listItems
                        .Select(li => li.InnerText.Trim())
                        .Where(t => !string.IsNullOrWhiteSpace(t))
                        .ToList();
                }
            }

            Console.WriteLine($"  ✓ Scraped details for {species.Name}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ✗ Error scraping details for {species.Name}: {ex.Message}");
        }
    }
}
