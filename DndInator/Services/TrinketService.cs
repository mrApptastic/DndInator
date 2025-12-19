using DndShared.Models;
using System.Net.Http.Json;

namespace DndInator.Services;

public interface ITrinketService
{
    Task<List<Trinket>> GetAllTrinketsAsync();
}

public class TrinketService : ITrinketService
{
    private readonly HttpClient _httpClient;

    public TrinketService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Trinket>> GetAllTrinketsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Trinket>>("data/2024/trinkets.json") ?? new List<Trinket>();
    }
}
