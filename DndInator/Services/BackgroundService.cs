using System.Net.Http.Json;
using DndShared.Models;

namespace DndInator.Services
{
    public interface IBackgroundService
    {
        Task<List<Background>> GetAllBackgroundsAsync();
    }

    public class BackgroundService : IBackgroundService
    {
        private readonly HttpClient _http;

        public BackgroundService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Background>> GetAllBackgroundsAsync()
        {
            return await _http.GetFromJsonAsync<List<Background>>($"data/2024/backgrounds.json") ?? new List<Background>();
        }
    }
}
