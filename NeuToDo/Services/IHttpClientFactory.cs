﻿using System.Net.Http;

namespace NeuToDo.Services
{
    /// <summary>
    /// 模拟IHttpClientFactory实现。
    /// </summary>
    public interface IHttpClientFactory
    {
        /// <summary>
        /// 用于获取Neu并爬取信息的客户端。
        /// </summary>
        public HttpClient NeuClient();

        /// <summary>
        /// 用于登录慕课并爬取信息的客户端。
        /// </summary>
        public HttpClient MoocClient();
    }
}