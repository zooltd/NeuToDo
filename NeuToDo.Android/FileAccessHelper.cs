using NeuToDo.Droid;
using NeuToDo.Services;
using Java.IO;
using Xamarin.Forms;
using Environment = Android.OS.Environment;

[assembly: Dependency(typeof(FileAccessHelper))]

namespace NeuToDo.Droid
{
    public class FileAccessHelper : IFileAccessHelper
    {
        private File _storageFolder;

        public string GetBackUpDirectory()
        {
            _storageFolder ??=
                Android.App.Application.Context.GetExternalFilesDir(Environment.DirectoryDcim);
            return _storageFolder.AbsolutePath;
        }
    }
}