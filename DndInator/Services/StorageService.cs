using Microsoft.JSInterop;

namespace DndInator.Services
{
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
    }

    public class StorageService : IStorageService
    {
        private readonly IJSRuntime _js;
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
    }
}