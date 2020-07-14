using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NeuToDo.Models;
using NeuToDo.ViewModels;
using WebDav;

namespace NeuToDo.Services
{
    public interface IHttpWebDavService
    {
        void Initiate(Account account);

        bool IsInitialized { get; set; }

        Task<bool> TestConnection();

        Task UploadFileAsync(string destPath, string sourcePath);
        
        Task UploadFileAsync(string destPath, Stream stream);

        Task CreateFolder(string folderName);

        Task<List<RecoveryFile>> GetFilesAsync(string sourcePath, string searchPattern = null);

        Task<Stream> GetFileAsync(string uri);

        Task<Stream> GetFileStreamAsync(string destPath);

        Task UploadFileAsZip<T>(string fileName, string destPath, IList<T> objectLists) where T : EventModel;

        [Obsolete]
        Task<IList<T>> DownLoadFileAsEventModelList<T>(string destPath) where T : EventModel;
    }
}