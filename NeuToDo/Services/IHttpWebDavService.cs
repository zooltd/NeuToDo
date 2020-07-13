using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NeuToDo.ViewModels;
using WebDav;

namespace NeuToDo.Services
{
    public interface IHttpWebDavService
    {
        void Initiate(Account account);

        bool IsInitialized { get; set; }

        Task<bool> TestConnection();

        Task UploadFile(string destPath, string sourcePath);

        Task CreateFolder(string folderName);

        Task<List<RecoveryFile>> GetFilesAsync(string sourcePath, string searchPattern = null);

        Task<Stream> GetFileAsync(string uri);
    }
}