﻿using Plugin.FilePicker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;

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

            if (pickedFile == null)
                throw new Exception($"导入文件名应为\"{StorageProvider.DbName}\"");

            if (pickedFile.FileName != StorageProvider.DbName)
                throw new Exception($"导入文件名应为\"{StorageProvider.DbName}\"");

            await _storageProvider.CloseConnectionAsync();

            var stream = pickedFile.GetStream();
            using var fileStream = File.Create(StorageProvider.DbPath);
            CopyStream(stream, fileStream);
        }

        public async Task<string> ExportAsync()
        {
            string destPath = null;
            switch (Device.RuntimePlatform)
            {
                case Device.UWP:

                    await Clipboard.SetTextAsync(StorageProvider.DbPath);
                    throw new Exception("UWP下暂不支持, 可自行复制, 路径已保存到剪切板");
                    break;
                case Device.Android:
                    await _storageProvider.CloseConnectionAsync();
                    var accessHelper = DependencyService.Get<IFileAccessHelper>();
                    var privateExternalDirectory = accessHelper.GetPrivateExternalDirectory();
                    if (!await accessHelper.CheckPermission()) throw new Exception("缺少外部存储访问权限");

                    destPath = Path.Combine(privateExternalDirectory, 
                        StorageProvider.DbName);

                    File.Copy(StorageProvider.DbPath, destPath, true);

                    break;
            }

            return destPath;
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