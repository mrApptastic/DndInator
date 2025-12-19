using System.Net.Http.Json;
using DndShared.Models;

namespace DndInator.Services;

public interface IClassService
{
    Task<List<CharacterClass>> GetAllClassesAsync();
}

public class ClassService : IClassService
{
    private readonly HttpClient _httpClient;

    public ClassService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<CharacterClass>> GetAllClassesAsync()
    {
        try
        {
            var classes = await _httpClient.GetFromJsonAsync<List<CharacterClass>>("data/2024/classes.json");
            return classes ?? new List<CharacterClass>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading classes: {ex.Message}");
            return new List<CharacterClass>();
        }
    }
}
