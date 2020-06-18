using System.Threading.Tasks;
using Xamarin.Essentials;

namespace NeuToDo.Services
{
    public class SecureStorageProvider : ISecureStorageProvider
    {
        public async Task<string> GetAsync(string key) => await SecureStorage.GetAsync(key);

        public async Task SetAsync(string key, string value) => await SecureStorage.SetAsync(key, value);

        public bool Remove(string key) => SecureStorage.Remove(key);

        public void RemoveAll() => SecureStorage.RemoveAll();
    }
}