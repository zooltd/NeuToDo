using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public interface IFileAccessHelper
    {
        string GetPrivateExternalDirectory();

        Task<bool> CheckPermission();
    }
}