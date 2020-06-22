﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace NeuToDo.Services {
    /// <summary>
    /// 模拟IHttpClientFactory实现。
    /// </summary>
    public interface IHttpClientFactory {
        /// <summary>
        /// 用于获取Neu表单信息的客户端。
        /// </summary>
        public HttpClient NeuInitClient();

        /// <summary>
        /// 用于登录Neu并爬取信息的客户端。
        /// </summary>
        public HttpClient NeuReallocateClient();

        /// <summary>
        /// 用于登录慕课并爬取信息的客户端。
        /// </summary>
        public HttpClient MoocClient();
    }
}