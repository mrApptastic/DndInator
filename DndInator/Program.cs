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
builder.Services.AddScoped<IStorageService, StorageService>();

await builder.Build().RunAsync();
