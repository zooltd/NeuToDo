using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WebDav;
using Xamarin.Essentials;

namespace NeuToDo.Services
{
    public class HttpWebDavService : IHttpWebDavService
    {
        private static IWebDavClient _client;

        public bool IsInitialized { get; set; }

        private string _baseUri;

        public static readonly string TargetDirectory = AppInfo.Name;

        public void Initiate(Account account)
        {
            var clientParams = new WebDavClientParams
            {
                BaseAddress = new Uri(account.BaseUri),
                Credentials = new NetworkCredential(account.UserName, account.Password)
            };

            _client = new WebDavClient(clientParams);
            _baseUri = account.BaseUri;
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
            var res = await _client.PutFile(_baseUri + destPath, new StreamContent(stream));
            if (!res.IsSuccessful) throw new HttpRequestException(res.Description);
        }

        public async Task CreateFolder(string folderName)
        {
            var res = await _client.Mkcol(folderName);
            if (!res.IsSuccessful) throw new HttpRequestException(res.Description);
        }


        public async Task GetAll()
        {
            if (!IsInitialized) throw new Exception("WebDAV未初始化");
            var result = await _client.Propfind($"{_baseUri}/{TargetDirectory}");
        }
    }
}