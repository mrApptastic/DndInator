using HtmlAgilityPack;
using DndShared.Models;
using System.Text.RegularExpressions;

namespace DndScraper.Helpers;

public class BackgroundScraper
{
    private const int DelayMs = 800;

    public static async Task<List<Background>> ScrapeBackgrounds2014()
    {
        var candidateUrls = new[]
        {
            "https://dnd5e.wikidot.com/background",
            "https://dnd5e.wikidot.com/backgrounds"
        };

        var backgrounds = new List<Background>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

            foreach (var backgroundUrl in candidateUrls)
            {
                try
                {
                    Console.WriteLine($"\n=== Scraping backgrounds from {backgroundUrl} ===");

                    var html = await client.GetStringAsync(backgroundUrl);
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(html);

                    var pageContent = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='page-content']");
                    if (pageContent == null)
                    {
                        Console.WriteLine("No page-content found");
                        continue;
                    }

                    var links = pageContent.SelectNodes(".//a[@href]");
                    if (links == null)
                    {
                        Console.WriteLine("No background links found");
                        continue;
                    }

                    foreach (var link in links)
                    {
                        var href = link.GetAttributeValue("href", string.Empty);
                        if (!href.StartsWith("/background:")) continue;

                        var name = link.InnerText.Trim();
                        var detailUrl = "https://dnd5e.wikidot.com" + href;

                        if (seen.Contains(detailUrl)) continue;
                        seen.Add(detailUrl);

                        var background = new Background
                        {
                            Name = name,
                            Category = "Background"
                        };

                        backgrounds.Add(background);
                        Console.WriteLine($"Found background: {background.Name}");

                        await ScrapeBackgroundDetails(client, background, detailUrl);
                        await Task.Delay(DelayMs);
                    }

                    if (backgrounds.Count > 0)
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error scraping backgrounds from {backgroundUrl}: {ex.Message}");
                }
            }
        }

        Console.WriteLine($"\n=== Total backgrounds found: {backgrounds.Count} ===");
        return backgrounds;
    }

    public static async Task<List<Background>> ScrapeBackgrounds2024()
    {
        string backgroundUrl = "http://dnd2024.wikidot.com/background:all";
        var backgrounds = new List<Background>();

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

            try
            {
                // Hent hovesiden med listen over backgrounds
                var html = await client.GetStringAsync(backgroundUrl);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                // Find alle tabs (kategorier)
                var tabNavigation = htmlDoc.DocumentNode.SelectSingleNode("//ul[@class='yui-nav']");
                var tabs = tabNavigation?.SelectNodes(".//li");
                
                if (tabs == null)
                {
                    Console.WriteLine("No tabs found");
                    return backgrounds;
                }

                // Find yui-content container
                var yuiContent = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='yui-content']");
                if (yuiContent == null)
                {
                    Console.WriteLine("No yui-content found");
                    return backgrounds;
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

                        var background = new Background();
                        background.Category = categoryName;

                        // Parse background name og URL til detaljesiden
                        var nameLink = cells[0].SelectSingleNode(".//a");
                        string? detailUrl = null;
                        if (nameLink != null)
                        {
                            background.Name = nameLink.InnerText.Trim();
                            detailUrl = "http://dnd2024.wikidot.com" + nameLink.GetAttributeValue("href", "");
                        }

                        backgrounds.Add(background);
                        Console.WriteLine($"Found background: {background.Name} (Category: {categoryName})");
                        
                        // Scrape detaljer fra detaljesiden
                        if (detailUrl != null)
                        {
                            await ScrapeBackgroundDetails(client, background, detailUrl);
                            await Task.Delay(500); // Vær pæn mod serveren
                        }
                    }
                }

                Console.WriteLine($"\n=== Total backgrounds found: {backgrounds.Count} ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scraping backgrounds: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        return backgrounds;
    }

    private static async Task ScrapeBackgroundDetails(HttpClient client, Background background, string url)
    {
        try
        {
            var html = await client.GetStringAsync(url);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var pageContent = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='page-content']");
            if (pageContent == null)
            {
                Console.WriteLine($"  ✗ No page-content found for {background.Name}");
                return;
            }

            // Gem hele HTML-indholdet
            background.FullContent = pageContent.InnerHtml;

            // Parse Source
            var paragraphs = pageContent.SelectNodes(".//p");
            if (paragraphs != null && paragraphs.Count > 0)
            {
                // Første paragraf er normalt Source
                var firstParagraphText = paragraphs[0].InnerText.Trim();
                if (firstParagraphText.StartsWith("Source:"))
                {
                    background.Source = firstParagraphText.Replace("Source:", "").Trim();
                }

                // Anden paragraf er normalt beskrivelsen
                if (paragraphs.Count > 1)
                {
                    background.Description = paragraphs[1].InnerText.Trim();
                }

                // Tredje paragraf indeholder detaljer (Ability Scores, Feat, etc.)
                if (paragraphs.Count > 2)
                {
                    var detailsParagraph = paragraphs[2];
                    var detailsText = detailsParagraph.InnerText;

                    // Parse Ability Scores
                    var abilityMatch = Regex.Match(detailsText, @"Ability Scores:\s*([^\n]+)");
                    if (abilityMatch.Success)
                    {
                        var abilities = abilityMatch.Groups[1].Value.Split(',')
                            .Select(a => a.Trim())
                            .Where(a => !string.IsNullOrWhiteSpace(a))
                            .ToList();
                        background.AbilityScores = abilities;
                    }

                    // Parse Feat
                    var featMatch = Regex.Match(detailsText, @"Feat:\s*([^\n]+)");
                    if (featMatch.Success)
                    {
                        background.Feat = featMatch.Groups[1].Value.Trim();
                    }

                    // Parse Skill Proficiencies
                    var skillMatch = Regex.Match(detailsText, @"Skill Proficiencies:\s*([^\n]+)");
                    if (skillMatch.Success)
                    {
                        var skills = skillMatch.Groups[1].Value.Split(new[] { " and ", ", " }, StringSplitOptions.None)
                            .Select(s => s.Trim())
                            .Where(s => !string.IsNullOrWhiteSpace(s))
                            .ToList();
                        background.SkillProficiencies = skills;
                    }

                    // Parse Tool Proficiency
                    var toolMatch = Regex.Match(detailsText, @"Tool Proficiency:\s*([^\n]+)");
                    if (toolMatch.Success)
                    {
                        background.ToolProficiency = toolMatch.Groups[1].Value.Trim();
                    }

                    // Parse Equipment
                    var equipmentMatch = Regex.Match(detailsText, @"Equipment:\s*(.+)", RegexOptions.Singleline);
                    if (equipmentMatch.Success)
                    {
                        background.Equipment = equipmentMatch.Groups[1].Value.Trim();
                    }
                }
            }

            Console.WriteLine($"  ✓ Scraped details for {background.Name}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ✗ Error scraping details for {background.Name}: {ex.Message}");
        }
    }
}
