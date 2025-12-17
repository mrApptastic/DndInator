using System.Net.Http.Json;
using DndShared.Models;

namespace DndInator.Services
{
    public interface ISpeciesService
    {
        Task<List<Species>> GetAllSpeciesAsync();
    }

    public class SpeciesService : ISpeciesService
    {
        private readonly HttpClient _http;

        public SpeciesService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Species>> GetAllSpeciesAsync()
        {
            return await _http.GetFromJsonAsync<List<Species>>($"data/2024/species.json") ?? new List<Species>();
        }
    }
}
