using DndShared.Models;
using System.Net.Http.Json;

namespace DndInator.Services;

public interface IAdventuringGearService
{
    Task<List<AdventuringGear>> GetAllAdventuringGearAsync();
}

public class AdventuringGearService : IAdventuringGearService
{
    private readonly HttpClient _httpClient;

    public AdventuringGearService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<AdventuringGear>> GetAllAdventuringGearAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<AdventuringGear>>("data/2024/adventuring-gear.json") ?? new List<AdventuringGear>();
    }
}
