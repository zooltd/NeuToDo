using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NeuToDo.Views;

namespace NeuToDo.Services
{
    /// <summary>
    /// 内容导航服务接口。
    /// </summary>
    public interface IContentNavigationService
    {
        /// <summary>
        /// 导航到页面。
        /// </summary>
        /// <param name="pageKey">页面键。</param>
        Task PushAsync(string sourceKey, string pageKey);

        /// <summary>
        /// 导航到页面。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pageKey">页面键。</param>
        /// <param name="parameter">参数。</param>
        Task PushAsync(string sourceKey, string pageKey, object parameter);
    }

    public static class TabbedPageConstants
    {
        public const string ToDoCalendarPage = nameof(Views.ToDoCalendarPage);

        /// <summary>
        /// 页面键-页面类型字典。
        /// </summary>
        public static readonly Dictionary<string, int> PageIndexDictionary = new Dictionary<string, int>
        {
            {ToDoCalendarPage, 0}
        };
    }

    /// <summary>
    /// 内容导航常量。
    /// </summary>
    public static class ContentNavigationConstants
    {
        public const string EventDetailPage = nameof(Views.EventDetailPage);

        /// <summary>
        /// 页面键-页面类型字典。
        /// </summary>
        public static readonly Dictionary<string, Type> PageKeyTypeDictionary = new Dictionary<string, Type>
        {
            {EventDetailPage, typeof(EventDetailPage)}
        };
    }
}