using DndShared.Models;
using System.Net.Http.Json;

namespace DndInator.Services;

public interface IWeaponService
{
    Task<List<Weapon>> GetAllWeaponsAsync();
}

public class WeaponService : IWeaponService
{
    private readonly HttpClient _httpClient;

    public WeaponService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Weapon>> GetAllWeaponsAsync()
    {
        try
        {
            var weapons = await _httpClient.GetFromJsonAsync<List<Weapon>>("data/2024/weapons.json");
            return weapons ?? new List<Weapon>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading weapons: {ex.Message}");
            return new List<Weapon>();
        }
    }
}
