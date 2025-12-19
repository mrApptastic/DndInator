using DndShared.Models;
using System.Net.Http.Json;

namespace DndInator.Services;

public interface IArmorService
{
    Task<List<Armor>> GetAllArmorAsync();
}

public class ArmorService : IArmorService
{
    private readonly HttpClient _httpClient;

    public ArmorService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Armor>> GetAllArmorAsync()
    {
        try
        {
            var armor = await _httpClient.GetFromJsonAsync<List<Armor>>("data/2024/armor.json");
            return armor ?? new List<Armor>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading armor: {ex.Message}");
            return new List<Armor>();
        }
    }
}
