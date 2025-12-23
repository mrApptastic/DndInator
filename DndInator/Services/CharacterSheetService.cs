using Microsoft.JSInterop;
using DndShared.Models;
using System.Text.Json;

namespace DndInator.Services;

public interface ICharacterSheetService
{
    /// <summary>
    /// Fills a D&D character sheet PDF with character data and returns it as a data URL
    /// </summary>
    Task<string> FillCharacterSheetAsync(Character character, string edition = "2024");
    
    /// <summary>
    /// Downloads a filled character sheet PDF
    /// </summary>
    Task DownloadCharacterSheetAsync(Character character, string filename, string edition = "2024");
    
    /// <summary>
    /// Gets the list of available form fields in the PDF (for debugging)
    /// </summary>
    Task<List<FormField>> GetFormFieldsAsync(string edition = "2024");
}

public class CharacterSheetService : ICharacterSheetService
{
    private readonly IJSRuntime _jsRuntime;
    
    public CharacterSheetService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
    public async Task<string> FillCharacterSheetAsync(Character character, string edition = "2024")
    {
        try
        {
            // Get the PDF URL based on edition
            var pdfUrl = $"data/{edition}/CharacterSheet.pdf";
            
            // Prepare character data for JavaScript
            var characterData = PrepareCharacterData(character);
            
            // Call JavaScript to fill the PDF
            var dataUrl = await _jsRuntime.InvokeAsync<string>(
                "characterSheetModule.fillCharacterSheet",
                pdfUrl,
                characterData
            );
            
            return dataUrl;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error filling character sheet: {ex.Message}");
            throw;
        }
    }
    
    public async Task DownloadCharacterSheetAsync(Character character, string filename, string edition = "2024")
    {
        try
        {
            // Fill the character sheet
            var dataUrl = await FillCharacterSheetAsync(character, edition);
            
            // Trigger download via JavaScript
            await _jsRuntime.InvokeVoidAsync(
                "characterSheetModule.downloadPdf",
                dataUrl,
                filename
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error downloading character sheet: {ex.Message}");
            throw;
        }
    }
    
    public async Task<List<FormField>> GetFormFieldsAsync(string edition = "2024")
    {
        try
        {
            var pdfUrl = $"data/{edition}/CharacterSheet.pdf";
            
            var fields = await _jsRuntime.InvokeAsync<List<FormField>>(
                "characterSheetModule.getFormFields",
                pdfUrl
            );
            
            return fields ?? new List<FormField>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting form fields: {ex.Message}");
            throw;
        }
    }
    
    /// <summary>
    /// Prepares character data in a format suitable for JavaScript consumption
    /// </summary>
    private object PrepareCharacterData(Character character)
    {
        // Create a clean object with all the character data
        // This will be serialized to JSON and passed to JavaScript
        return new
        {
            information = character.Information != null ? new
            {
                name = character.Information.Name ?? "",
                race = character.Information.Race ?? "",
                @class = character.Information.Class ?? "",
                background = character.Information.Background ?? "",
                alignment = character.Information.Alignment?.ToString() ?? "",
                playerName = "", // Add to model if needed
                experiencePoints = "" // Add to model if needed
            } : null,
            
            stats = character.Stats != null ? new
            {
                strength = character.Stats.Strength,
                dexterity = character.Stats.Dexterity,
                constitution = character.Stats.Constitution,
                intelligence = character.Stats.Intelligence,
                wisdom = character.Stats.Wisdom,
                charisma = character.Stats.Charisma
            } : null,
            
            @class = character.Class != null ? new
            {
                name = character.Class.Name ?? "",
                hitPointDie = character.Class.HitPointDie ?? "",
                primaryAbility = character.Class.PrimaryAbility ?? "",
                savingThrowProficiencies = character.Class.SavingThrowProficiencies ?? "",
                skillProficiencies = character.Class.SkillProficiencies ?? ""
            } : null,
            
            background = character.Background != null ? new
            {
                name = character.Background.Name ?? "",
                description = character.Background.Description ?? "",
                skillProficiencies = character.Background.SkillProficiencies ?? new List<string>(),
                toolProficiency = character.Background.ToolProficiency ?? "",
                equipment = character.Background.Equipment ?? ""
            } : null,
            
            race = character.Race != null ? new
            {
                name = character.Race.Name ?? "",
                speed = character.Race.Speed ?? "",
                size = character.Race.Size ?? "",
                traits = character.Race.Traits ?? new List<string>()
            } : null,
            
            feats = character.Feats?.Select(f => new
            {
                name = f.Name ?? "",
                description = f.Description ?? "",
                benefits = f.Benefits ?? new List<string>()
            }).ToArray()
        };
    }
}

/// <summary>
/// Represents a form field in a PDF
/// </summary>
public class FormField
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}
