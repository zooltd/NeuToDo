using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        /// <param name="sourceKey">来源页面键。</param>
        Task PushAsync();
        
        /// <summary>
        /// 导航到页面。
        /// </summary>
        /// <param name="sourceKey">来源页面键。</param>
        /// <param name="parameter">参数。</param>
        Task PushAsync(object parameter);
    }
}