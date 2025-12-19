using HtmlAgilityPack;
using DndShared.Models;
using System.Text.RegularExpressions;

namespace DndScraper.Helpers;

public class CharacterClassScraper
{
    public static async Task<List<CharacterClass>> ScrapeClasses2024()
    {
        var classList = new List<CharacterClass>();
        
        // Liste over alle classes med deres URLs og subclass URL patterns
        var classUrls = new Dictionary<string, (string mainUrl, string subclassPrefix)>
        {
            { "Artificer", ("http://dnd2024.wikidot.com/artificer:main", "artificer") },
            { "Barbarian", ("http://dnd2024.wikidot.com/barbarian:main", "barbarian") },
            { "Bard", ("http://dnd2024.wikidot.com/bard:main", "bard") },
            { "Cleric", ("http://dnd2024.wikidot.com/cleric:main", "cleric") },
            { "Druid", ("http://dnd2024.wikidot.com/druid:main", "druid") },
            { "Fighter", ("http://dnd2024.wikidot.com/fighter:main", "fighter") },
            { "Monk", ("http://dnd2024.wikidot.com/monk:main", "monk") },
            { "Paladin", ("http://dnd2024.wikidot.com/paladin:main", "paladin") },
            { "Ranger", ("http://dnd2024.wikidot.com/ranger:main", "ranger") },
            { "Rogue", ("http://dnd2024.wikidot.com/rogue:main", "rogue") },
            { "Sorcerer", ("http://dnd2024.wikidot.com/sorcerer:main", "sorcerer") },
            { "Warlock", ("http://dnd2024.wikidot.com/warlock:main", "warlock") },
            { "Wizard", ("http://dnd2024.wikidot.com/wizard:main", "wizard") }
        };

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

            Console.WriteLine($"\n=== Scraping character classes from dnd2024.wikidot.com ===");

            foreach (var kvp in classUrls)
            {
                try
                {
                    var className = kvp.Key;
                    var url = kvp.Value.mainUrl;

                    Console.WriteLine($"\n--- Scraping {className} ---");

                    var html = await client.GetStringAsync(url);
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(html);

                    var characterClass = new CharacterClass
                    {
                        Name = className
                    };

                    var pageContent = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='page-content']");
                    if (pageContent == null)
                    {
                        Console.WriteLine($"Could not find page content for {className}");
                        continue;
                    }

                    // Parse Source
                    var sourceParagraph = pageContent.SelectSingleNode(".//p[starts-with(text(), 'Source:')]");
                    if (sourceParagraph != null)
                    {
                        characterClass.Source = sourceParagraph.InnerText.Replace("Source:", "").Trim();
                    }

                    // Parse description (første paragraf efter Source)
                    var paragraphs = pageContent.SelectNodes(".//p");
                    if (paragraphs != null && paragraphs.Count > 1)
                    {
                        var descriptionParts = new List<string>();
                        bool foundSource = false;
                        foreach (var p in paragraphs)
                        {
                            var text = p.InnerText.Trim();
                            if (text.StartsWith("Source:"))
                            {
                                foundSource = true;
                                continue;
                            }
                            if (foundSource && !string.IsNullOrWhiteSpace(text) && !text.StartsWith("Core "))
                            {
                                descriptionParts.Add(text);
                                if (descriptionParts.Count >= 3) break; // Tag de første 3 beskrivelsesparagraffer
                            }
                        }
                        characterClass.Description = string.Join("\n\n", descriptionParts);
                    }

                    // Parse Core Traits tabel
                    var coreTraitsTable = pageContent.SelectSingleNode(".//table[.//th[contains(text(), 'Core') and contains(text(), 'Traits')]]");
                    if (coreTraitsTable != null)
                    {
                        var rows = coreTraitsTable.SelectNodes(".//tr");
                        if (rows != null)
                        {
                            foreach (var row in rows)
                            {
                                var cells = row.SelectNodes("td");
                                if (cells == null || cells.Count < 2) continue;

                                var label = cells[0].InnerText.Trim();
                                var value = cells[1].InnerText.Trim();

                                switch (label)
                                {
                                    case "Primary Ability":
                                        characterClass.PrimaryAbility = value;
                                        break;
                                    case "Hit Point Die":
                                        characterClass.HitPointDie = value;
                                        break;
                                    case "Saving Throw Proficiencies":
                                        characterClass.SavingThrowProficiencies = value;
                                        break;
                                    case "Skill Proficiencies":
                                        characterClass.SkillProficiencies = value;
                                        break;
                                    case "Weapon Proficiencies":
                                        characterClass.WeaponProficiencies = value;
                                        break;
                                    case "Armor Training":
                                        characterClass.ArmorTraining = value;
                                        break;
                                    case "Tool Proficiencies":
                                        characterClass.ToolProficiencies = value;
                                        break;
                                    case "Starting Equipment":
                                        characterClass.StartingEquipment = value;
                                        break;
                                }
                            }
                        }
                    }

                    // Parse Subclasses
                    var subclassTable = pageContent.SelectSingleNode(".//table[.//th[contains(text(), 'Subclass')]]");
                    if (subclassTable != null)
                    {
                        var subclassRows = subclassTable.SelectNodes(".//tr");
                        if (subclassRows != null)
                        {
                            foreach (var row in subclassRows.Skip(1)) // Skip header
                            {
                                var cells = row.SelectNodes("td");
                                if (cells != null && cells.Count > 0)
                                {
                                    characterClass.SubclassNames.Add(cells[0].InnerText.Trim());
                                }
                            }
                        }
                    }

                    // Gem hele page content som backup
                    characterClass.FullContent = pageContent.InnerText;

                    classList.Add(characterClass);
                    Console.WriteLine($"✓ Scraped {className} successfully");

                    await Task.Delay(500); // Vær pæn mod serveren
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"✗ Error scraping {kvp.Key}: {ex.Message}");
                }
            }

            Console.WriteLine($"\n=== Total classes found: {classList.Count} ===");
        }

        return classList;
    }

    public static async Task<List<Subclass>> ScrapeSubclasses2024()
    {
        var subclassList = new List<Subclass>();
        
        // Map af subclass names til deres URL slugs
        var subclassUrls = new Dictionary<string, (string parentClass, string url)>
        {
            // Artificer
            { "Alchemist", ("Artificer", "http://dnd2024.wikidot.com/artificer:alchemist") },
            { "Armorer", ("Artificer", "http://dnd2024.wikidot.com/artificer:armorer") },
            { "Artillerist", ("Artificer", "http://dnd2024.wikidot.com/artificer:artillerist") },
            { "Battle Smith", ("Artificer", "http://dnd2024.wikidot.com/artificer:battle-smith") },
            
            // Barbarian
            { "Path of the Berserker", ("Barbarian", "http://dnd2024.wikidot.com/barbarian:berserker") },
            { "Path of the Wild Heart", ("Barbarian", "http://dnd2024.wikidot.com/barbarian:wild-heart") },
            { "Path of the World Tree", ("Barbarian", "http://dnd2024.wikidot.com/barbarian:world-tree") },
            { "Path of the Zealot", ("Barbarian", "http://dnd2024.wikidot.com/barbarian:zealot") },
            
            // Bard
            { "College of Dance", ("Bard", "http://dnd2024.wikidot.com/bard:college-of-dance") },
            { "College of Glamour", ("Bard", "http://dnd2024.wikidot.com/bard:college-of-glamour") },
            { "College of Lore", ("Bard", "http://dnd2024.wikidot.com/bard:college-of-lore") },
            { "College of Valor", ("Bard", "http://dnd2024.wikidot.com/bard:college-of-valor") },
            
            // Cleric
            { "Life Domain", ("Cleric", "http://dnd2024.wikidot.com/cleric:life-domain") },
            { "Light Domain", ("Cleric", "http://dnd2024.wikidot.com/cleric:light-domain") },
            { "Trickery Domain", ("Cleric", "http://dnd2024.wikidot.com/cleric:trickery-domain") },
            { "War Domain", ("Cleric", "http://dnd2024.wikidot.com/cleric:war-domain") },
            
            // Druid
            { "Circle of the Land", ("Druid", "http://dnd2024.wikidot.com/druid:circle-of-the-land") },
            { "Circle of the Moon", ("Druid", "http://dnd2024.wikidot.com/druid:circle-of-the-moon") },
            { "Circle of the Sea", ("Druid", "http://dnd2024.wikidot.com/druid:circle-of-the-sea") },
            { "Circle of the Stars", ("Druid", "http://dnd2024.wikidot.com/druid:circle-of-the-stars") },
            
            // Fighter
            { "Battle Master", ("Fighter", "http://dnd2024.wikidot.com/fighter:battle-master") },
            { "Champion", ("Fighter", "http://dnd2024.wikidot.com/fighter:champion") },
            { "Eldritch Knight", ("Fighter", "http://dnd2024.wikidot.com/fighter:eldritch-knight") },
            { "Psi Warrior", ("Fighter", "http://dnd2024.wikidot.com/fighter:psi-warrior") },
            
            // Monk
            { "Warrior of Mercy", ("Monk", "http://dnd2024.wikidot.com/monk:warrior-of-mercy") },
            { "Warrior of Shadow", ("Monk", "http://dnd2024.wikidot.com/monk:warrior-of-shadow") },
            { "Warrior of the Elements", ("Monk", "http://dnd2024.wikidot.com/monk:warrior-of-the-elements") },
            { "Warrior of the Open Hand", ("Monk", "http://dnd2024.wikidot.com/monk:warrior-of-the-open-hand") },
            
            // Paladin
            { "Oath of Devotion", ("Paladin", "http://dnd2024.wikidot.com/paladin:oath-of-devotion") },
            { "Oath of Glory", ("Paladin", "http://dnd2024.wikidot.com/paladin:oath-of-glory") },
            { "Oath of the Ancients", ("Paladin", "http://dnd2024.wikidot.com/paladin:oath-of-the-ancients") },
            { "Oath of Vengeance", ("Paladin", "http://dnd2024.wikidot.com/paladin:oath-of-vengeance") },
            
            // Ranger
            { "Beast Master", ("Ranger", "http://dnd2024.wikidot.com/ranger:beast-master") },
            { "Fey Wanderer", ("Ranger", "http://dnd2024.wikidot.com/ranger:fey-wanderer") },
            { "Gloom Stalker", ("Ranger", "http://dnd2024.wikidot.com/ranger:gloom-stalker") },
            { "Hunter", ("Ranger", "http://dnd2024.wikidot.com/ranger:hunter") },
            
            // Rogue
            { "Arcane Trickster", ("Rogue", "http://dnd2024.wikidot.com/rogue:arcane-trickster") },
            { "Assassin", ("Rogue", "http://dnd2024.wikidot.com/rogue:assassin") },
            { "Soulknife", ("Rogue", "http://dnd2024.wikidot.com/rogue:soulknife") },
            { "Thief", ("Rogue", "http://dnd2024.wikidot.com/rogue:thief") },
            
            // Sorcerer
            { "Aberrant Sorcery", ("Sorcerer", "http://dnd2024.wikidot.com/sorcerer:aberrant-sorcery") },
            { "Clockwork Sorcery", ("Sorcerer", "http://dnd2024.wikidot.com/sorcerer:clockwork-sorcery") },
            { "Draconic Sorcery", ("Sorcerer", "http://dnd2024.wikidot.com/sorcerer:draconic-sorcery") },
            { "Wild Magic Sorcery", ("Sorcerer", "http://dnd2024.wikidot.com/sorcerer:wild-magic-sorcery") },
            
            // Warlock
            { "Archfey Patron", ("Warlock", "http://dnd2024.wikidot.com/warlock:archfey-patron") },
            { "Celestial Patron", ("Warlock", "http://dnd2024.wikidot.com/warlock:celestial-patron") },
            { "Fiend Patron", ("Warlock", "http://dnd2024.wikidot.com/warlock:fiend-patron") },
            { "Great Old One Patron", ("Warlock", "http://dnd2024.wikidot.com/warlock:great-old-one-patron") },
            
            // Wizard
            { "Abjurer", ("Wizard", "http://dnd2024.wikidot.com/wizard:abjurer") },
            { "Diviner", ("Wizard", "http://dnd2024.wikidot.com/wizard:diviner") },
            { "Evoker", ("Wizard", "http://dnd2024.wikidot.com/wizard:evoker") },
            { "Illusionist", ("Wizard", "http://dnd2024.wikidot.com/wizard:illusionist") }
        };

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

            Console.WriteLine($"\n=== Scraping subclasses from dnd2024.wikidot.com ===");

            foreach (var kvp in subclassUrls)
            {
                try
                {
                    var subclassName = kvp.Key;
                    var parentClass = kvp.Value.parentClass;
                    var url = kvp.Value.url;

                    Console.WriteLine($"--- Scraping {subclassName} ({parentClass}) ---");

                    var html = await client.GetStringAsync(url);
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(html);

                    var subclass = new Subclass
                    {
                        Name = subclassName,
                        ParentClass = parentClass
                    };

                    var pageContent = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='page-content']");
                    if (pageContent == null)
                    {
                        Console.WriteLine($"Could not find page content for {subclassName}");
                        continue;
                    }

                    // Parse Source
                    var sourceParagraph = pageContent.SelectSingleNode(".//p[starts-with(text(), 'Source:')]");
                    if (sourceParagraph != null)
                    {
                        subclass.Source = sourceParagraph.InnerText.Replace("Source:", "").Trim();
                    }

                    // Parse description (første paragraf efter Source)
                    var paragraphs = pageContent.SelectNodes(".//p");
                    if (paragraphs != null && paragraphs.Count > 1)
                    {
                        var descriptionParts = new List<string>();
                        bool foundSource = false;
                        foreach (var p in paragraphs)
                        {
                            var text = p.InnerText.Trim();
                            if (text.StartsWith("Source:"))
                            {
                                foundSource = true;
                                continue;
                            }
                            if (foundSource && !string.IsNullOrWhiteSpace(text) && !text.StartsWith("Level "))
                            {
                                descriptionParts.Add(text);
                                if (descriptionParts.Count >= 2) break; // Tag de første 2 beskrivelsesparagraffer
                            }
                        }
                        subclass.Description = string.Join("\n\n", descriptionParts);
                    }

                    // Gem hele page content som backup
                    subclass.FullContent = pageContent.InnerText;

                    subclassList.Add(subclass);
                    Console.WriteLine($"✓ Scraped {subclassName} successfully");

                    await Task.Delay(500); // Vær pæn mod serveren
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"✗ Error scraping {kvp.Key}: {ex.Message}");
                }
            }

            Console.WriteLine($"\n=== Total subclasses found: {subclassList.Count} ===");
        }

        return subclassList;
    }
}
