using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using DndInator;
using DndInator.Services;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IDiceService, DiceService>();
builder.Services.AddScoped<ILineageService, LineageService>();
builder.Services.AddScoped<ISpellService, SpellService>();
builder.Services.AddScoped<IBackgroundService, BackgroundService>();
builder.Services.AddScoped<ISpeciesService, SpeciesService>();
builder.Services.AddScoped<IFeatService, FeatService>();
builder.Services.AddScoped<IMagicItemService, MagicItemService>();
builder.Services.AddScoped<IClassService, ClassService>();
builder.Services.AddScoped<IWeaponService, WeaponService>();
builder.Services.AddScoped<IArmorService, ArmorService>();
builder.Services.AddScoped<IAdventuringGearService, AdventuringGearService>();
builder.Services.AddScoped<IMountAndVehicleService, MountAndVehicleService>();
builder.Services.AddScoped<IPoisonService, PoisonService>();
builder.Services.AddScoped<IToolService, ToolService>();
builder.Services.AddScoped<ITrinketService, TrinketService>();
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<ICharacterSheetService, CharacterSheetService>();

await builder.Build().RunAsync();
