using System.Net.Http.Json;
using DndShared.Models;

namespace DndInator.Services
{
    public interface ISpellService
    {
        Task<List<Spell>> GetAllSpellsAsync();
    }

    public class SpellService : ISpellService
    {
        private readonly HttpClient _http;

        public SpellService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Spell>> GetAllSpellsAsync()
        {
            return await _http.GetFromJsonAsync<List<Spell>>($"data/spells.json");          
        }
    }
}