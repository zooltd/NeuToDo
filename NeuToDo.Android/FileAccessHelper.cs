using Android.OS;
using NeuToDo.Droid;
using NeuToDo.Services;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Threading.Tasks;
using Xamarin.Forms;
using PermissionStatus = Plugin.Permissions.Abstractions.PermissionStatus;

[assembly: Dependency(typeof(FileAccessHelper))]

namespace NeuToDo.Droid
{
    public class FileAccessHelper : IFileAccessHelper
    {
        public string GetPrivateExternalDirectory()
        {
            return Android.App.Application.Context.GetExternalFilesDir(Environment.DirectoryDocuments).AbsolutePath;
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