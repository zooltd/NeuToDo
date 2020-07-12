using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public interface IFileAccessHelper
    {
        string GetBackUpDirectory();
        // Task CopyToAsync();

        Task<bool> CheckPermission();
    }
}