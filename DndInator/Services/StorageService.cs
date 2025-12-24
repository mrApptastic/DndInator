using Microsoft.JSInterop;
using System.Text.Json;
using DndShared.Models;

namespace DndInator.Services
{
    public enum StorageType
    {
        LocalStorage,
        SessionStorage
    }

    public interface IStorageService
    {
        Task SetLocalItem(string key, string value);
        Task<string?> GetLocalItem(string key);
        Task RemoveLocalItem(string key);
        Task ClearLocalStorage();
        Task SetSessionItem(string key, string value);
        Task<string?> GetSessionItem(string key);
        Task RemoveSessionItem(string key);
        Task ClearSessionStorage();
        
        // Character Management
        Task<List<Character>> GetAllCharactersAsync();
        Task SaveCharacterAsync(Character character);
        Task DeleteCharacterAsync(Guid characterId);
        Task<Character?> GetCharacterAsync(Guid characterId);
        Task<StorageType> GetStorageTypeAsync();
        Task SetStorageTypeAsync(StorageType storageType);
        Task ClearCharactersAsync();
        Task<string> ExportCharactersAsync();
        Task<(int imported, int duplicates)> ImportCharactersAsync(string json);
    }

    public class StorageService : IStorageService
    {
        private readonly IJSRuntime _js;
        private const string CharactersKey = "dnd_characters";
        private const string StorageTypeKey = "dnd_storage_type";
        
        public StorageService(IJSRuntime js)
        {
            _js = js;
        }

        public async Task SetLocalItem(string key, string value)
        {
            await _js.InvokeVoidAsync("localStorage.setItem", key, value);
        }

        public async Task<string?> GetLocalItem(string key)
        {
            return await _js.InvokeAsync<string?>("localStorage.getItem", key);
        }

        public async Task RemoveLocalItem(string key)
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", key);
        }

        public async Task ClearLocalStorage()
        {
            await _js.InvokeVoidAsync("localStorage.clear");
        }

        public async Task SetSessionItem(string key, string value)
        {
            await _js.InvokeVoidAsync("sessionStorage.setItem", key, value);   
        }

        public async Task<string?> GetSessionItem(string key)
        {
            return await _js.InvokeAsync<string?>("sessionStorage.getItem", key);
        }

        public async Task RemoveSessionItem(string key)
        {
            await _js.InvokeVoidAsync("sessionStorage.removeItem", key);
        }

        public async Task ClearSessionStorage()
        {
            await _js.InvokeVoidAsync("sessionStorage.clear");
        }

        // Character Management Methods
        public async Task<StorageType> GetStorageTypeAsync()
        {
            var typeString = await GetLocalItem(StorageTypeKey);
            if (Enum.TryParse<StorageType>(typeString, out var storageType))
            {
                return storageType;
            }
            return StorageType.LocalStorage; // Default
        }

        public async Task SetStorageTypeAsync(StorageType storageType)
        {
            await SetLocalItem(StorageTypeKey, storageType.ToString());
        }

        private async Task<string?> GetItemAsync(string key)
        {
            var storageType = await GetStorageTypeAsync();
            return storageType == StorageType.LocalStorage
                ? await GetLocalItem(key)
                : await GetSessionItem(key);
        }

        private async Task SetItemAsync(string key, string value)
        {
            var storageType = await GetStorageTypeAsync();
            if (storageType == StorageType.LocalStorage)
            {
                await SetLocalItem(key, value);
            }
            else
            {
                await SetSessionItem(key, value);
            }
        }

        public async Task<List<Character>> GetAllCharactersAsync()
        {
            try
            {
                var json = await GetItemAsync(CharactersKey);
                if (string.IsNullOrEmpty(json))
                {
                    return new List<Character>();
                }

                var characters = JsonSerializer.Deserialize<List<Character>>(json);
                return characters ?? new List<Character>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading characters: {ex.Message}");
                return new List<Character>();
            }
        }

        public async Task SaveCharacterAsync(Character character)
        {
            var characters = await GetAllCharactersAsync();
            
            // Update if exists, otherwise add
            var existingIndex = characters.FindIndex(c => c.Id == character.Id);
            if (existingIndex >= 0)
            {
                characters[existingIndex] = character;
            }
            else
            {
                characters.Add(character);
            }

            var json = JsonSerializer.Serialize(characters);
            await SetItemAsync(CharactersKey, json);
        }

        public async Task DeleteCharacterAsync(Guid characterId)
        {
            var characters = await GetAllCharactersAsync();
            characters.RemoveAll(c => c.Id == characterId);
            
            var json = JsonSerializer.Serialize(characters);
            await SetItemAsync(CharactersKey, json);
        }

        public async Task<Character?> GetCharacterAsync(Guid characterId)
        {
            var characters = await GetAllCharactersAsync();
            return characters.FirstOrDefault(c => c.Id == characterId);
        }

        public async Task ClearCharactersAsync()
        {
            var storageType = await GetStorageTypeAsync();
            if (storageType == StorageType.LocalStorage)
            {
                await RemoveLocalItem(CharactersKey);
            }
            else
            {
                await RemoveSessionItem(CharactersKey);
            }
        }

        public async Task<string> ExportCharactersAsync()
        {
            var characters = await GetAllCharactersAsync();
            return JsonSerializer.Serialize(characters, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
        }

        public async Task<(int imported, int duplicates)> ImportCharactersAsync(string json)
        {
            try
            {
                var importedCharacters = JsonSerializer.Deserialize<List<Character>>(json);
                if (importedCharacters == null || !importedCharacters.Any())
                {
                    return (0, 0);
                }

                var existingCharacters = await GetAllCharactersAsync();
                var existingIds = existingCharacters.Select(c => c.Id).ToHashSet();
                
                int imported = 0;
                int duplicates = 0;

                foreach (var character in importedCharacters)
                {
                    if (existingIds.Contains(character.Id))
                    {
                        duplicates++;
                        // Optionally: Generate new ID for duplicate
                        character.Id = Guid.NewGuid();
                    }
                    
                    existingCharacters.Add(character);
                    imported++;
                }

                var serialized = JsonSerializer.Serialize(existingCharacters);
                await SetItemAsync(CharactersKey, serialized);

                return (imported, duplicates);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error importing characters: {ex.Message}");
                throw;
            }
        }
    }
}