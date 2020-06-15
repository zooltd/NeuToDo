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
        Task PushAsync(string sourceKey);

        /// <summary>
        /// 导航到页面。
        /// </summary>
        /// <param name="sourceKey">来源页面键。</param>
        /// <param name="parameter">参数。</param>
        Task PushAsync(string sourceKey, object parameter);
    }

    public static class TabbedPageConstants
    {
        public const string ToDoCalendarPage = nameof(Views.ToDoCalendarPage);

        public const string ToDoListPage = nameof(Views.ToDoListPage);


        /// <summary>
        /// 页面键-页面类型字典。
        /// </summary>
        public static readonly Dictionary<string, int> PageIndexDictionary = new Dictionary<string, int>
        {
            {ToDoCalendarPage, 0},
            {ToDoListPage, 0}
        };
    }
}