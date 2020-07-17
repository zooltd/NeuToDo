using System.Threading.Tasks;
using NeuToDo.Services;
using NeuToDo.UWP;
using Xamarin.Forms;
using Windows.Storage;

[assembly: Dependency(typeof(FileAccessHelper))]

namespace NeuToDo.UWP
{
    public class FileAccessHelper : IFileAccessHelper
    {
        private StorageFolder _storageFolder;

        public string GetBackUpDirectory()
        {
            _storageFolder ??= ApplicationData.Current.LocalFolder;
            return _storageFolder.Path;
        }
    }
}