using HtmlAgilityPack;
using DndShared.Models;

namespace DndScraper.Helpers;

public class SpellScraper
{
    public static async Task<List<Spell>> ScrapeSpells2014()
    {
        string spellUrl = "https://dnd5e.wikidot.com/spells";
        var spells = new List<Spell>();

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

            try
            {
                // Hent hovesiden med listen over spells
                var html = await client.GetStringAsync(spellUrl);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                // Find alle wiki-tab divs for hver spell level (0-9)
                for (int level = 0; level <= 9; level++)
                {
                    var tabId = $"wiki-tab-0-{level}";
                    var tabDiv = htmlDoc.DocumentNode.SelectSingleNode($"//div[@id='{tabId}']");
                    
                    if (tabDiv == null) continue;

                    var rows = tabDiv.SelectNodes(".//table[@class='wiki-content-table']//tr");
                    
                    if (rows == null) continue;

                    // Skip header row
                    foreach (var row in rows.Skip(1))
                    {
                        var cells = row.SelectNodes("td");
                        if (cells == null || cells.Count < 6) continue;

                        var spell = new Spell();
                        spell.Level = level;

                        // Parse spell name og URL til detaljesiden
                        var nameLink = cells[0].SelectSingleNode(".//a");
                        string? detailUrl = null;
                        if (nameLink != null)
                        {
                            spell.Name = nameLink.InnerText.Trim();
                            detailUrl = "https://dnd5e.wikidot.com" + nameLink.GetAttributeValue("href", "");
                        }

                        // Parse grundlæggende info fra tabellen
                        spell.School = cells[1].InnerText.Trim();
                        spell.CastingTime = cells[2].InnerText.Trim();
                        spell.Range = cells[3].InnerText.Trim();
                        spell.Duration = cells[4].InnerText.Trim();
                        spell.Components = cells[5].InnerText.Trim();

                        spells.Add(spell);
                        Console.WriteLine($"Found spell: {spell.Name} (Level {level})");
                        
                        // Gem URL midlertidigt for at scrape detaljer
                        if (detailUrl != null)
                        {
                            await ScrapeSpellDetails(client, spell, detailUrl);
                            await Task.Delay(500); // Vær pæn mod serveren
                        }
                    }
                }

                Console.WriteLine($"\nTotal spells found: {spells.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scraping spells: {ex.Message}");
            }
        }

        return spells;
    }

        public static async Task<List<Spell>> ScrapeSpells2024()
    {
        string spellUrl = "http://dnd2024.wikidot.com/spell:all";
        var spells = new List<Spell>();

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

            try
            {
                // Hent hovesiden med listen over spells
                var html = await client.GetStringAsync(spellUrl);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                // Find alle wiki-tab divs for hver spell level (0-9)
                for (int level = 0; level <= 9; level++)
                {
                    var tabId = $"wiki-tab-0-{level}";
                    var tabDiv = htmlDoc.DocumentNode.SelectSingleNode($"//div[@id='{tabId}']");
                    
                    if (tabDiv == null) continue;

                    var rows = tabDiv.SelectNodes(".//table[@class='wiki-content-table']//tr");
                    
                    if (rows == null) continue;

                    // Skip header row
                    foreach (var row in rows.Skip(1))
                    {
                        var cells = row.SelectNodes("td");
                        if (cells == null || cells.Count < 7) continue;

                        var spell = new Spell();
                        spell.Level = level;

                        // Parse spell name og URL til detaljesiden
                        var nameLink = cells[0].SelectSingleNode(".//a");
                        string? detailUrl = null;
                        if (nameLink != null)
                        {
                            spell.Name = nameLink.InnerText.Trim();
                            detailUrl = "http://dnd2024.wikidot.com" + nameLink.GetAttributeValue("href", "");
                        }

                        // Parse grundlæggende info fra tabellen
                        // 2024 format: Name | School | Spell lists | Casting Time | Range | Components | Duration
                        spell.School = cells[1].InnerText.Trim();
                        
                        // Parse spell lists fra cell 2
                        var spellListText = cells[2].InnerText.Trim();
                        if (!string.IsNullOrEmpty(spellListText))
                        {
                            spell.SpellLists = spellListText.Split(',').Select(s => s.Trim()).ToList();
                        }
                        
                        spell.CastingTime = cells[3].InnerText.Trim();
                        spell.Range = cells[4].InnerText.Trim();
                        spell.Components = cells[5].InnerText.Trim();
                        spell.Duration = cells[6].InnerText.Trim();

                        spells.Add(spell);
                        Console.WriteLine($"Found spell: {spell.Name} (Level {level})");
                        
                        // Gem URL midlertidigt for at scrape detaljer
                        if (detailUrl != null)
                        {
                            await ScrapeSpellDetails(client, spell, detailUrl);
                            await Task.Delay(500); // Vær pæn mod serveren
                        }
                    }
                }

                Console.WriteLine($"\nTotal spells found: {spells.Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scraping spells: {ex.Message}");
            }
        }

        return spells;
    }

    public static async Task ScrapeSpellDetails(HttpClient client, Spell spell, string url)
    {
        try
        {
            var html = await client.GetStringAsync(url);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var pageContent = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='page-content']");
            if (pageContent == null) return;

            // Parse Source
            var paragraphs = pageContent.SelectNodes(".//p");
            if (paragraphs != null && paragraphs.Count > 0)
            {
                spell.Source = paragraphs[0].InnerText.Replace("Source:", "").Trim();

                // Parse description (alt tekst mellem komponenterne og "At Higher Levels")
                var descriptionParagraphs = new List<string>();
                bool foundComponents = false;

                foreach (var p in paragraphs)
                {
                    var text = p.InnerText.Trim();

                    if (text.StartsWith("At Higher Levels"))
                    {
                        spell.AtHigherLevels = text.Replace("At Higher Levels.", "").Trim();
                        break;
                    }
                    else if (text.StartsWith("Spell Lists"))
                    {
                        // Parse spell lists
                        var links = p.SelectNodes(".//a");
                        if (links != null)
                        {
                            spell.SpellLists = links.Select(l => l.InnerText.Trim()).ToList();
                        }
                    }
                    else if (foundComponents && !text.StartsWith("Source:") &&
                             !text.Contains("Casting Time:") &&
                             !text.Contains("cantrip") &&
                             !text.Contains("level spell"))
                    {
                        descriptionParagraphs.Add(text);
                    }

                    if (text.Contains("Duration:"))
                    {
                        foundComponents = true;
                    }
                }

                spell.Description = string.Join("\n\n", descriptionParagraphs);
            }

            Console.WriteLine($"  ✓ Scraped details for {spell.Name}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ✗ Error scraping details for {spell.Name}: {ex.Message}");
        }
    }
}
