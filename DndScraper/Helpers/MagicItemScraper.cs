using HtmlAgilityPack;
using DndShared.Models;
using System.Text.RegularExpressions;

namespace DndScraper.Helpers;

public class MagicItemScraper
{
    public static async Task<List<MagicItem>> ScrapeMagicItems2024()
    {
        string magicItemUrl = "http://dnd2024.wikidot.com/magic-item:all";
        var magicItems = new List<MagicItem>();

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

            try
            {
                // Hent hovesiden med listen over magic items
                var html = await client.GetStringAsync(magicItemUrl);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                // Find alle tabs (kategorier)
                var tabNavigation = htmlDoc.DocumentNode.SelectSingleNode("//ul[@class='yui-nav']");
                var tabs = tabNavigation?.SelectNodes(".//li");
                
                if (tabs == null)
                {
                    Console.WriteLine("No tabs found");
                    return magicItems;
                }

                // Find yui-content container
                var yuiContent = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='yui-content']");
                if (yuiContent == null)
                {
                    Console.WriteLine("No yui-content found");
                    return magicItems;
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

                        var magicItem = new MagicItem();
                        magicItem.Category = categoryName;

                        // Parse magic item name og URL til detaljesiden
                        var nameLink = cells[0].SelectSingleNode(".//a");
                        string? detailUrl = null;
                        if (nameLink != null)
                        {
                            magicItem.Name = nameLink.InnerText.Trim();
                            detailUrl = "http://dnd2024.wikidot.com" + nameLink.GetAttributeValue("href", "");
                        }

                        magicItems.Add(magicItem);
                        Console.WriteLine($"Found magic item: {magicItem.Name} (Category: {categoryName})");
                        
                        // Scrape detaljer fra detaljesiden
                        if (detailUrl != null)
                        {
                            await ScrapeMagicItemDetails(client, magicItem, detailUrl);
                            await Task.Delay(500); // Vær pæn mod serveren
                        }
                    }
                }

                Console.WriteLine($"\n=== Total magic items found: {magicItems.Count} ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scraping magic items: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        return magicItems;
    }

    private static async Task ScrapeMagicItemDetails(HttpClient client, MagicItem magicItem, string url)
    {
        try
        {
            var html = await client.GetStringAsync(url);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var pageContent = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='page-content']");
            if (pageContent == null)
            {
                Console.WriteLine($"  ✗ No page-content found for {magicItem.Name}");
                return;
            }

            // Gem hele HTML-indholdet
            magicItem.FullContent = pageContent.InnerHtml;

            // Parse Source
            var paragraphs = pageContent.SelectNodes(".//p");
            if (paragraphs != null && paragraphs.Count > 0)
            {
                // Første paragraf er normalt Source
                var firstParagraphText = paragraphs[0].InnerText.Trim();
                if (firstParagraphText.StartsWith("Source:"))
                {
                    magicItem.Source = firstParagraphText.Replace("Source:", "").Trim();
                }

                // Saml beskrivelsen (paragraffer indtil vi finder detaljer)
                var descriptionParagraphs = new List<string>();
                foreach (var p in paragraphs.Skip(1))
                {
                    var text = p.InnerText.Trim();
                    
                    // Stop ved første strong tag med detaljer
                    if (text.Contains("Type:") || text.Contains("Rarity:") || text.Contains("Attunement:"))
                    {
                        // Parse magic item detaljer
                        var detailsText = text;

                        // Parse Type
                        var typeMatch = Regex.Match(detailsText, @"Type:\s*([^\n]+)");
                        if (typeMatch.Success)
                        {
                            magicItem.Type = typeMatch.Groups[1].Value.Trim();
                        }

                        // Parse Rarity
                        var rarityMatch = Regex.Match(detailsText, @"Rarity:\s*([^\n]+)");
                        if (rarityMatch.Success)
                        {
                            magicItem.Rarity = rarityMatch.Groups[1].Value.Trim();
                        }

                        // Parse Attunement
                        var attunementMatch = Regex.Match(detailsText, @"Attunement:\s*([^\n]+)");
                        if (attunementMatch.Success)
                        {
                            magicItem.Attunement = attunementMatch.Groups[1].Value.Trim();
                        }

                        break;
                    }
                    else if (!text.StartsWith("Source:"))
                    {
                        descriptionParagraphs.Add(text);
                    }
                }

                magicItem.Description = string.Join("\n\n", descriptionParagraphs);

                // Parse properties (normalt liste items)
                var listItems = pageContent.SelectNodes(".//ul/li | .//ol/li");
                if (listItems != null)
                {
                    magicItem.Properties = listItems
                        .Select(li => li.InnerText.Trim())
                        .Where(t => !string.IsNullOrWhiteSpace(t))
                        .ToList();
                }
            }

            Console.WriteLine($"  ✓ Scraped details for {magicItem.Name}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ✗ Error scraping details for {magicItem.Name}: {ex.Message}");
        }
    }
}
