using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public interface ISecureStorageProvider
    {
        Task<string> GetAsync(string key);
        Task<string> TryGetAsync(string key, string defaultValue);
        Task SetAsync(string key, string value);
        bool Remove(string key);
        void RemoveAll();
    }
}