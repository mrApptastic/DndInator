using System.Net.Http.Json;
using DndShared.Models;

namespace DndInator.Services
{
    public interface ILineageService
    {
        Task<List<Lineage>> GetAllLineagesAsync();
    }

    public class LineageService : ILineageService
    {
        private readonly HttpClient _http;

        public LineageService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Lineage>> GetAllLineagesAsync()
        {
            return await _http.GetFromJsonAsync<List<Lineage>>($"data/lineages.json");          
        }
    }
}