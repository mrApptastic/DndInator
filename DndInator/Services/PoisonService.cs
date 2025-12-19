using DndShared.Models;
using System.Net.Http.Json;

namespace DndInator.Services;

public interface IPoisonService
{
    Task<List<Poison>> GetAllPoisonsAsync();
}

public class PoisonService : IPoisonService
{
    private readonly HttpClient _httpClient;

    public PoisonService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Poison>> GetAllPoisonsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Poison>>("data/2024/poisons.json") ?? new List<Poison>();
    }
}
