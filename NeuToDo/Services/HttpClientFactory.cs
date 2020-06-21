using System.Net;
using System.Net.Http;

namespace NeuToDo.Services {
    /// <summary>
    /// 基于微软官网上IHttpClientFactory的实现有坑。
    /// 实现一个模拟的HttpClientFactory。
    /// </summary>
    public class HttpClientFactory : IHttpClientFactory {
        private static CookieContainer _cookieContainer;

        /// <summary>
        /// 用于获取Neu表单信息的客户端。
        /// </summary>
        public HttpClient NeuInitClient() {
            _cookieContainer = new CookieContainer();
            var client = new HttpClient(new HttpClientHandler {
                AllowAutoRedirect = false,
                UseCookies = true,
                CookieContainer = _cookieContainer
            });
            return client;
        }

        /// <summary>
        /// 用于登录Neu并爬取信息的客户端。
        /// </summary>
        public HttpClient NeuReallocateClient() {
            var client = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = _cookieContainer
            });
            return client;
        }

        /// <summary>
        /// 用于登录慕课并爬取信息的客户端。
        /// </summary>
        public HttpClient MoocClient() {
            var client = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = new CookieContainer()
            });
            client.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.61 Safari/537.36");
            return client;
        }
    }
}