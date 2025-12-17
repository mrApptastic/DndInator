using System.Net.Http.Json;
using DndShared.Models;

namespace DndInator.Services
{
    public interface IFeatService
    {
        Task<List<Feat>> GetAllFeatsAsync();
    }

    public class FeatService : IFeatService
    {
        private readonly HttpClient _http;

        public FeatService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Feat>> GetAllFeatsAsync()
        {
            return await _http.GetFromJsonAsync<List<Feat>>($"data/2024/feats.json") ?? new List<Feat>();
        }
    }
}
