using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NeuToDo.Models;
using NeuToDo.Views;

namespace NeuToDo.Services
{
    /// <summary>
    /// 内容导航服务接口。
    /// </summary>
    public interface IEventDetailNavigationService
    {
        /// <summary>
        /// 导航到页面。
        /// </summary>
        /// <param name="e"></param>
        Task PushAsync(EventModel e);
    }
}