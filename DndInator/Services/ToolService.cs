using DndShared.Models;
using System.Net.Http.Json;

namespace DndInator.Services;

public interface IToolService
{
    Task<List<Tool>> GetAllToolsAsync();
}

public class ToolService : IToolService
{
    private readonly HttpClient _httpClient;

    public ToolService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Tool>> GetAllToolsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Tool>>("data/2024/tools.json") ?? new List<Tool>();
    }
}
