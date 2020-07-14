using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using NeuToDo.ViewModels;
using Newtonsoft.Json;
using WebDav;
using Xamarin.Essentials;

namespace NeuToDo.Services
{
    public class HttpWebDavService : IHttpWebDavService
    {
        private static IWebDavClient _client;

        public bool IsInitialized { get; set; }

        // private string _baseUri;

        private Uri _baseUri;
        // public static readonly string TargetDirectory = AppInfo.Name;

        public void Initiate(Account account)
        {
            var clientParams = new WebDavClientParams
            {
                BaseAddress = new Uri(account.BaseUri),
                Credentials = new NetworkCredential(account.UserName, account.Password)
            };

            _client = new WebDavClient(clientParams);
            _baseUri = new Uri(account.BaseUri);
            IsInitialized = true;
        }

        public async Task<bool> TestConnection()
        {
            if (!IsInitialized) throw new Exception("WebDAV未初始化");
            var result = await _client.Propfind(_baseUri);
            return result.IsSuccessful;
        }

        public async Task UploadFile(string destPath, string sourcePath)
        {
            var stream = File.OpenRead(sourcePath);
            var res = await _client.PutFile(new Uri(_baseUri, destPath), new StreamContent(stream));
            if (!res.IsSuccessful) throw new HttpRequestException(res.Description);
        }

        public async Task CreateFolder(string folderName)
        {
            var res = await _client.Mkcol(folderName);
            if (!res.IsSuccessful) throw new HttpRequestException(res.Description);
        }

        public async Task<List<RecoveryFile>> GetFilesAsync(string sourcePath, string searchPattern = null)
        {
            if (!IsInitialized) throw new Exception("WebDAV未初始化");
            var webFiles = new List<WebDavResource>();
            var res = await _client.Propfind(new Uri(_baseUri, sourcePath));
            if (!res.IsSuccessful) throw new HttpRequestException(res.Description);
            var files = res.Resources.ToList();
            if (searchPattern == null)
                webFiles = files;
            else
            {
                webFiles.AddRange(from file in files
                    let match = Regex.Match(file.DisplayName, searchPattern)
                    where match.Success
                    select file);
            }

            return webFiles.ConvertAll(x => new RecoveryFile
            {
                FileName = x.DisplayName, FilePath = new Uri(_baseUri, x.Uri).AbsoluteUri,
                FileSource = FileSource.Server
            });
        }

        public async Task<Stream> GetFileAsync(string uri)
        {
            if (!IsInitialized) throw new Exception("WebDAV未初始化");
            var res = await _client.GetRawFile(uri);
            if (!res.IsSuccessful) throw new HttpRequestException(res.Description);
            return res.Stream;
        }


        public async Task UploadFileAsZip(string destPath, string fileName, IList<object> objectLists)
        {
            if (!IsInitialized) throw new Exception("WebDAV未初始化");
            var json = JsonConvert.SerializeObject(objectLists);
            var fileStream = new MemoryStream();
            var zipStream = new ZipOutputStream(fileStream);

            zipStream.SetLevel(3);

            var newEntry = new ZipEntry(fileName) {DateTime = DateTime.Now};
            zipStream.PutNextEntry(newEntry);

            var jsonStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            await Task.Run(() => StreamUtils.Copy(jsonStream, zipStream, new byte[1024]));
            jsonStream.Close();
            zipStream.CloseEntry();

            zipStream.IsStreamOwner = false;
            zipStream.Close();

            fileStream.Position = 0;

            await _client.Delete(destPath);
            var res = await _client.PutFile(destPath, fileStream);
            if (!res.IsSuccessful) throw new HttpRequestException(res.Description);
        }
    }
}