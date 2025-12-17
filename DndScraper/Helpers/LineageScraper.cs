using HtmlAgilityPack;
using DndShared.Models;

namespace DndScraper.Helpers;

public class LineageScraper
{
    public static async Task<List<Lineage>> ScrapeLineages()
    {
        string lineageUrl = "https://dnd5e.wikidot.com/lineage";
        var lineages = new List<Lineage>();

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

            try
            {
                // Hent hovesiden med listen over lineages
                var html = await client.GetStringAsync(lineageUrl);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                // Find alle wiki-content-table tabeller
                var tables = htmlDoc.DocumentNode.SelectNodes("//table[@class='wiki-content-table']");

                if (tables == null)
                {
                    Console.WriteLine("Could not find lineage tables.");
                    return lineages;
                }

                // Saml alle links fra alle tabeller
                var lineageLinks = new List<(string name, string url)>();
                
                foreach (var table in tables)
                {
                    var links = table.SelectNodes(".//a[@href]");
                    if (links == null) continue;

                    foreach (var link in links)
                    {
                        var href = link.GetAttributeValue("href", "");
                        if (href.StartsWith("/lineage:"))
                        {
                            var name = link.InnerText.Trim();
                            var url = "https://dnd5e.wikidot.com" + href;
                            lineageLinks.Add((name, url));
                        }
                    }
                }

                Console.WriteLine($"Found {lineageLinks.Count} lineages");

                // Scrape detaljer for hver lineage
                foreach (var (name, url) in lineageLinks)
                {
                    var lineage = new Lineage { Name = name };
                    await ScrapeLineageDetails(client, lineage, url);
                    lineages.Add(lineage);
                    
                    Console.WriteLine($"✓ Scraped {name}");
                    await Task.Delay(500); // Vær pæn mod serveren
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scraping lineages: {ex.Message}");
            }
        }

        return lineages;
    }

    private static async Task ScrapeLineageDetails(HttpClient client, Lineage lineage, string url)
    {
        try
        {
            var html = await client.GetStringAsync(url);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var pageContent = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='page-content']");
            if (pageContent == null) return;

            // Parse hovedindholdet og subraces
            var h1Nodes = pageContent.SelectNodes(".//h1");
            var h2Nodes = pageContent.SelectNodes(".//h2");
            
            // Byg fuld content som HTML string
            var contentBuilder = new System.Text.StringBuilder();
            
            // Hvis der er h1 nodes, antag at første er main lineage
            if (h1Nodes != null && h1Nodes.Count > 0)
            {
                // Parse main lineage content (fra start til første h1 eller h2 subrace)
                var mainContent = ExtractContentUntilNextHeader(pageContent, h1Nodes[0]);
                contentBuilder.AppendLine(mainContent);
                
                // Parse subraces (h2 headers)
                if (h2Nodes != null)
                {
                    foreach (var h2 in h2Nodes)
                    {
                        var subraceName = h2.InnerText.Trim();
                        var subraceContent = ExtractContentUntilNextHeader(pageContent, h2);
                        
                        lineage.Subraces.Add(new Subrace
                        {
                            Name = subraceName,
                            Content = subraceContent
                        });
                    }
                }
                
                // Hvis der er flere h1 nodes (f.eks. forskellige sourcebooks), tilføj dem også som subraces
                for (int i = 1; i < h1Nodes.Count; i++)
                {
                    var h1 = h1Nodes[i];
                    var sourceName = h1.InnerText.Trim();
                    
                    // Find alle h2 under denne h1
                    var nextH1 = i + 1 < h1Nodes.Count ? h1Nodes[i + 1] : null;
                    var subraceH2s = GetH2NodesBetween(h1, nextH1);
                    
                    foreach (var h2 in subraceH2s)
                    {
                        var subraceName = $"{sourceName} - {h2.InnerText.Trim()}";
                        var subraceContent = ExtractContentUntilNextHeader(pageContent, h2);
                        
                        lineage.Subraces.Add(new Subrace
                        {
                            Name = subraceName,
                            Content = subraceContent
                        });
                    }
                }
            }
            else
            {
                // Ingen headers, tag alt content
                contentBuilder.AppendLine(pageContent.InnerHtml);
            }
            
            lineage.Content = contentBuilder.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ✗ Error scraping details for {lineage.Name}: {ex.Message}");
        }
    }

    private static string ExtractContentUntilNextHeader(HtmlNode pageContent, HtmlNode currentHeader)
    {
        var content = new System.Text.StringBuilder();
        var node = currentHeader.NextSibling;
        
        while (node != null)
        {
            // Stop hvis vi rammer næste header
            if (node.Name == "h1" || node.Name == "h2" || node.Name == "hr")
                break;
                
            if (node.NodeType == HtmlNodeType.Element)
            {
                content.AppendLine(node.OuterHtml);
            }
            
            node = node.NextSibling;
        }
        
        return content.ToString();
    }

    private static List<HtmlNode> GetH2NodesBetween(HtmlNode startH1, HtmlNode? endH1)
    {
        var h2Nodes = new List<HtmlNode>();
        var node = startH1.NextSibling;
        
        while (node != null)
        {
            if (endH1 != null && node == endH1)
                break;
                
            if (node.Name == "h2")
                h2Nodes.Add(node);
                
            node = node.NextSibling;
        }
        
        return h2Nodes;
    }
}
