using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public class HttpWebDavService
    {
        public string BaseUrl => "";
        public string UserName => "";
        public string Password => "";

        private readonly HttpClient _httpClient;

        public HttpWebDavService()
        {
            HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.UseProxy = true;
            httpClientHandler.Proxy = new WebProxy();
            httpClientHandler.Credentials = new NetworkCredential(this.UserName, this.Password, "Domain");
            httpClientHandler.PreAuthenticate = true;
            this._httpClient = new HttpClient(httpClientHandler);
        }

        /// <summary>
        /// 获取资源
        /// </summary>
        /// <param name="api"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<byte[]> GetBytes(string fileName)
        {
            string url = $"{this.BaseUrl}{fileName}";
            return await this._httpClient.GetByteArrayAsync(url);
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="folderName"></param>
        public async Task<bool> MakeFolder(string folderName)
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage
            {
                Method = new HttpMethod("MKCOL"), RequestUri = new Uri($"{this.BaseUrl}{folderName}")
            };
            HttpResponseMessage httpResponseMessage = await this._httpClient.SendAsync(httpRequestMessage);
            return httpResponseMessage.IsSuccessStatusCode;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="deleteFile"></param>
        public async Task<bool> Delete(string deleteFile)
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Delete, RequestUri = new Uri($"{this.BaseUrl}{deleteFile}")
            };
            HttpResponseMessage httpResponseMessage = await this._httpClient.SendAsync(httpRequestMessage);
            return httpResponseMessage.IsSuccessStatusCode;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="bys"></param>
        /// <returns></returns>
        public async Task<bool> Upload(string fileName, byte[] bys)
        {
            string url = $"{this.BaseUrl}{fileName}";
            ByteArrayContent byteArrayContent = new ByteArrayContent(bys);
            HttpResponseMessage httpResponseMessage = await _httpClient.PutAsync(url, byteArrayContent);
            return httpResponseMessage.IsSuccessStatusCode;
        }
    }
}