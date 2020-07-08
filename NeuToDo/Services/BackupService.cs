using Plugin.FilePicker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NeuToDo.Services
{
    public class BackupService : IBackupService
    {
        private readonly IStorageProvider _storageProvider;

        public BackupService(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
        }

        public async Task ImportAsync(List<FileType> allowedTypes = null)
        {
            string[] fileTypes = Device.RuntimePlatform switch
            {
                Device.Android => allowedTypes?.ConvertAll(x => PlatformFileTypeDict.DroidTypes[x]).ToArray(),
                Device.UWP => allowedTypes?.ConvertAll(x => PlatformFileTypeDict.UWPTypes[x]).ToArray(),
                _ => null
            };
            var pickedFile = await CrossFilePicker.Current.PickFile(fileTypes);
            if (pickedFile == null) return;

            if (pickedFile.FileName != "events.sqlite3")
                throw new Exception("导入文件名应为\"events.sqlite3\"");

            await _storageProvider.CloseConnectionAsync();

            if (File.Exists(StorageProvider.DbPath))
                File.Delete(StorageProvider.DbPath);
            var stream = pickedFile.GetStream();
            using var fileStream = File.Create(StorageProvider.DbPath);
            CopyStream(stream, fileStream);
        }

        private void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[16 * 1024];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                output.Write(buffer, 0, read);
        }
    }
}