using System;
using NeuToDo.Droid;
using NeuToDo.Services;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Threading.Tasks;
using Java.IO;
using Xamarin.Forms;
using Environment = Android.OS.Environment;
using PermissionStatus = Plugin.Permissions.Abstractions.PermissionStatus;

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

        public async Task<bool> CheckPermission()
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync<StoragePermission>();

            if (status == PermissionStatus.Granted) return status == PermissionStatus.Granted;

            await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage);

            status = await CrossPermissions.Current.RequestPermissionAsync<StoragePermission>();

            return status == PermissionStatus.Granted;
        }
    }
}