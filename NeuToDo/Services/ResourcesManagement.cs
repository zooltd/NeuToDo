using System.Net;
using System.Net.Http;

namespace NeuToDo.Services
{
    /// <summary>
    /// 之后会改为基于IHttpClientFactory的实现
    /// https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/http-requests?view=aspnetcore-3.1#basic-usage 
    /// </summary>
    public class ResourcesManagement
    {
        protected static CookieContainer CookieContainer;
        protected static HttpClientHandler ClientHandler;
        protected static HttpClient Client;

        protected static void InitSources(bool allowAutoRedirect)
        {
            CookieContainer = new CookieContainer();
            ClientHandler = new HttpClientHandler
                {AllowAutoRedirect = allowAutoRedirect, UseCookies = true, CookieContainer = CookieContainer};
            Client = new HttpClient(ClientHandler);
        }

        protected static void ReallocateSources(bool allowAutoRedirect)
        {
            ClientHandler.Dispose();
            Client.Dispose();
            ClientHandler = new HttpClientHandler()
                {AllowAutoRedirect = allowAutoRedirect, UseCookies = true, CookieContainer = CookieContainer};
            Client = new HttpClient(ClientHandler);
        }
    }
}