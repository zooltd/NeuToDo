using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public interface IHttpWebDavService
    {
        void Initiate(Account account);

        bool IsInitialized { get; set; }

        Task<bool> TestConnection();

        Task UploadFile(string destPath, string sourcePath);

        Task CreateFolder(string folderName);
    }
}