using System.Net.Http.Json;
using DndShared.Models;

namespace DndInator.Services
{
    public interface IMagicItemService
    {
        Task<List<MagicItem>> GetAllMagicItemsAsync();
    }

    public class MagicItemService : IMagicItemService
    {
        private readonly HttpClient _http;

        public MagicItemService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<MagicItem>> GetAllMagicItemsAsync()
        {
            return await _http.GetFromJsonAsync<List<MagicItem>>($"data/2024/magic-items.json") ?? new List<MagicItem>();
        }
    }
}
