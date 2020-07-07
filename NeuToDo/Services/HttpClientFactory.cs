using System.Net;
using System.Net.Http;

namespace NeuToDo.Services
{
    /// <summary>
    /// 基于微软官网上IHttpClientFactory的实现有坑。
    /// 实现一个模拟的HttpClientFactory。
    /// </summary>
    public class HttpClientFactory : IHttpClientFactory
    {
        public HttpClient NeuClient()
        {
            var client = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = new CookieContainer()
            });
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36 Edg/83.0.478.58");
            return client;
        }

        /// <summary>
        /// 用于登录慕课并爬取信息的客户端。
        /// </summary>
        public HttpClient MoocClient()
        {
            var client = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = new CookieContainer()
            });
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36 Edg/83.0.478.58");
            return client;
        }
    }
}