using HtmlAgilityPack;
using DndShared.Models;
using System.Text.RegularExpressions;

namespace DndScraper.Helpers;

public class FeatScraper
{
    public static async Task<List<Feat>> ScrapeFeats2024()
    {
        string featUrl = "http://dnd2024.wikidot.com/feat:all";
        var feats = new List<Feat>();

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

            try
            {
                // Hent hovesiden med listen over feats
                var html = await client.GetStringAsync(featUrl);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                // Find alle tabs (kategorier)
                var tabNavigation = htmlDoc.DocumentNode.SelectSingleNode("//ul[@class='yui-nav']");
                var tabs = tabNavigation?.SelectNodes(".//li");
                
                if (tabs == null)
                {
                    Console.WriteLine("No tabs found");
                    return feats;
                }

                // Find yui-content container
                var yuiContent = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='yui-content']");
                if (yuiContent == null)
                {
                    Console.WriteLine("No yui-content found");
                    return feats;
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

                        var feat = new Feat();
                        feat.Category = categoryName;

                        // Parse feat name og URL til detaljesiden
                        var nameLink = cells[0].SelectSingleNode(".//a");
                        string? detailUrl = null;
                        if (nameLink != null)
                        {
                            feat.Name = nameLink.InnerText.Trim();
                            detailUrl = "http://dnd2024.wikidot.com" + nameLink.GetAttributeValue("href", "");
                        }

                        feats.Add(feat);
                        Console.WriteLine($"Found feat: {feat.Name} (Category: {categoryName})");
                        
                        // Scrape detaljer fra detaljesiden
                        if (detailUrl != null)
                        {
                            await ScrapeFeatDetails(client, feat, detailUrl);
                            await Task.Delay(500); // Vær pæn mod serveren
                        }
                    }
                }

                Console.WriteLine($"\n=== Total feats found: {feats.Count} ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scraping feats: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        return feats;
    }

    private static async Task ScrapeFeatDetails(HttpClient client, Feat feat, string url)
    {
        try
        {
            var html = await client.GetStringAsync(url);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var pageContent = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='page-content']");
            if (pageContent == null)
            {
                Console.WriteLine($"  ✗ No page-content found for {feat.Name}");
                return;
            }

            // Gem hele HTML-indholdet
            feat.FullContent = pageContent.InnerHtml;

            // Parse Source
            var paragraphs = pageContent.SelectNodes(".//p");
            if (paragraphs != null && paragraphs.Count > 0)
            {
                // Første paragraf er normalt Source
                var firstParagraphText = paragraphs[0].InnerText.Trim();
                if (firstParagraphText.StartsWith("Source:"))
                {
                    feat.Source = firstParagraphText.Replace("Source:", "").Trim();
                }

                // Saml beskrivelsen (paragraffer indtil vi finder detaljer)
                var descriptionParagraphs = new List<string>();
                foreach (var p in paragraphs.Skip(1))
                {
                    var text = p.InnerText.Trim();
                    
                    // Stop ved første strong tag med detaljer
                    if (text.Contains("Level:") || text.Contains("Prerequisite:") || text.Contains("Repeatable:"))
                    {
                        // Parse feat detaljer
                        var detailsText = text;

                        // Parse Level
                        var levelMatch = Regex.Match(detailsText, @"Level:\s*([^\n]+)");
                        if (levelMatch.Success)
                        {
                            feat.Level = levelMatch.Groups[1].Value.Trim();
                        }

                        // Parse Prerequisite
                        var prereqMatch = Regex.Match(detailsText, @"Prerequisite:\s*([^\n]+)");
                        if (prereqMatch.Success)
                        {
                            feat.Prerequisite = prereqMatch.Groups[1].Value.Trim();
                        }

                        // Parse Repeatable
                        var repeatMatch = Regex.Match(detailsText, @"Repeatable:\s*([^\n]+)");
                        if (repeatMatch.Success)
                        {
                            feat.Repeatable = repeatMatch.Groups[1].Value.Trim();
                        }

                        break;
                    }
                    else if (!text.StartsWith("Source:"))
                    {
                        descriptionParagraphs.Add(text);
                    }
                }

                feat.Description = string.Join("\n\n", descriptionParagraphs);

                // Parse benefits (normalt liste items)
                var listItems = pageContent.SelectNodes(".//ul/li | .//ol/li");
                if (listItems != null)
                {
                    feat.Benefits = listItems
                        .Select(li => li.InnerText.Trim())
                        .Where(t => !string.IsNullOrWhiteSpace(t))
                        .ToList();
                }
            }

            Console.WriteLine($"  ✓ Scraped details for {feat.Name}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ✗ Error scraping details for {feat.Name}: {ex.Message}");
        }
    }
}
