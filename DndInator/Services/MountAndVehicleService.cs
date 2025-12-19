using DndShared.Models;
using System.Net.Http.Json;

namespace DndInator.Services;

public interface IMountAndVehicleService
{
    Task<List<MountAndVehicle>> GetAllMountsAndVehiclesAsync();
}

public class MountAndVehicleService : IMountAndVehicleService
{
    private readonly HttpClient _httpClient;

    public MountAndVehicleService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<MountAndVehicle>> GetAllMountsAndVehiclesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<MountAndVehicle>>("data/2024/mounts-and-vehicles.json") ?? new List<MountAndVehicle>();
    }
}
