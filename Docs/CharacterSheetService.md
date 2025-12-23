# Character Sheet Service

Service til at udfylde D&D 5e character sheets med character data og returnere dem som PDF.

## Funktionalitet

CharacterSheetService giver mulighed for at:
- Udfylde et form-fillable PDF character sheet med character data
- Returnere det udfyldte sheet som en data URL
- Downloade det udfyldte sheet direkte
- Få liste over alle form fields i PDF'en (til debugging)

## Teknisk Implementation

### C# Service
- **CharacterSheetService.cs**: Service class med JSInterop integration
- Understøtter 2024, 2014 og 3.5e character sheets
- Automatisk beregning af ability modifiers
- Mapping af character data til PDF form fields

### JavaScript Module
- **characterSheet.js**: JavaScript module der bruger pdf-lib til PDF manipulation
- Henter pdf-lib dynamisk fra CDN (https://cdn.jsdelivr.net/npm/pdf-lib@1.17.1)
- Fylder form fields baseret på character data
- Konverterer til data URL for preview eller download

## Brug

### Dependency Injection
```csharp
@inject ICharacterSheetService CharacterSheetService
```

### Udfyld Character Sheet
```csharp
// Opret en character
var character = new Character
{
    Information = new CharacterInformation
    {
        Name = "Thorin Stonehammer",
        Race = "Mountain Dwarf",
        Class = "Fighter",
        Background = "Soldier"
    },
    Stats = new CharacterStats
    {
        Strength = 16,
        Dexterity = 12,
        Constitution = 15,
        Intelligence = 10,
        Wisdom = 13,
        Charisma = 8
    },
    // ... mere data
};

// Generer character sheet
string pdfDataUrl = await CharacterSheetService.FillCharacterSheetAsync(character, "2024");

// Brug data URL i en iframe til preview
<iframe src="@pdfDataUrl" style="width: 100%; height: 800px;"></iframe>
```

### Download Character Sheet
```csharp
// Download direkte
await CharacterSheetService.DownloadCharacterSheetAsync(
    character, 
    "ThorinStonehammer.pdf",
    "2024"
);
```

### Debug: Få Form Fields
```csharp
// Få liste over alle form fields i PDF'en
var fields = await CharacterSheetService.GetFormFieldsAsync("2024");
foreach (var field in fields)
{
    Console.WriteLine($"{field.Name} ({field.Type})");
}
```

## Demo Side

En komplet demo side er tilgængelig på `/charactersheet` som viser:
- Sample character data
- Generate button
- PDF preview
- Download funktionalitet
- Debug værktøj til at se form fields

## PDF Form Field Mapping

Servicen mapper automatisk følgende character data til PDF fields:

### Basic Information
- `CharacterName` → Character.Information.Name
- `ClassLevel` → Character.Information.Class
- `Background` → Character.Information.Background
- `Race ` → Character.Information.Race
- `Alignment` → Character.Information.Alignment

### Ability Scores
- `STR`, `STRmod` → Strength og modifier
- `DEX`, `DEXmod ` → Dexterity og modifier
- `CON`, `CONmod` → Constitution og modifier
- `INT`, `INTmod` → Intelligence og modifier
- `WIS`, `WISmod` → Wisdom og modifier
- `CHA`, `CHamod` → Charisma og modifier

### Class Details
- `HPMax` → Hit die fra class
- `ProfBonus` → Proficiency bonus
- `Speed` → Speed fra race

### Features
- `Features and Traits` → Feats og race traits
- `ProficienciesLang` → Skills fra background

## Tilpasning

### Tilføj Flere Form Fields
Rediger `fillFormFields` funktionen i [characterSheet.js](../DndInator/wwwroot/js/characterSheet.js) for at mappe flere fields:

```javascript
setTextField('NewFieldName', character.newData);
```

### Understøt Andre PDF Editions
PDF'er skal ligge i `wwwroot/data/{edition}/CharacterSheet.pdf`:
- `2024` → D&D 5e 2024 rules
- `2014` → D&D 5e 2014 rules
- `35` → D&D 3.5e

## Fejlfinding

### PDF Loader Ikke
- Check browser console for JavaScript errors
- Verificer at pdf-lib loader korrekt fra CDN
- Check at PDF path er korrekt (`data/2024/CharacterSheet.pdf`)

### Fields Udfyldes Ikke
- Brug debug funktionen til at få liste over alle field names
- Sammenlign field names i PDF'en med dem i `fillFormFields`
- PDF fields er case-sensitive!

### Modifier Beregnes Forkert
Modifiers beregnes som: `Math.floor((abilityScore - 10) / 2)`

## Afhængigheder

### NuGet Packages
- Microsoft.JSInterop (inkluderet i Blazor WebAssembly)

### JavaScript Libraries
- pdf-lib 1.17.1 (loaded dynamically fra CDN)

## Fremtidige Forbedringer

Potentielle udvidelser:
- Flere character felter (skills, equipment, spells)
- Multipage support
- Character sheet templates (forskellige layouts)
- Export til andre formater (JSON, HTML)
- Import fra D&D Beyond eller andre sources
