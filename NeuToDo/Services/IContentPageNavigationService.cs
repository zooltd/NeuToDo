using NeuToDo.Models;
using NeuToDo.Views.EventDetailPage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NeuToDo.Views.SyncPage;

namespace NeuToDo.Services
{
    /// <summary>
    /// 内容导航服务接口。
    /// </summary>
    public interface IContentPageNavigationService
    {
        /// <summary>
        /// 导航到事件编辑页面。
        /// </summary>
        /// <param name="e"></param>
        Task PushAsync(EventModel e);

        /// <summary>
        /// 导航到页面。
        /// </summary>
        /// <param name="pageKey"></param>
        Task PushAsync(string pageKey);

        /// <summary>
        /// 返回Root页
        /// </summary>
        /// <returns></returns>
        Task PopToRootAsync();
    }

    /// <summary>
    /// 内容导航常量。
    /// </summary>
    public static class ContentNavigationConstants
    {
        public const string WebSyncPage = nameof(Views.SyncPage.WebSyncPage);

        public const string LocalSyncPage = nameof(Views.SyncPage.LocalSyncPage);

        /// <summary>
        /// 页面键-页面类型字典。
        /// </summary>
        public static readonly Dictionary<string, Type> PageKeyTypeDictionary =
            new Dictionary<string, Type>
            {
                {nameof(NeuEvent), typeof(NeuEventDetailPage)},
                {nameof(MoocEvent), typeof(MoocEventDetailPage)},
                {nameof(UserEvent), typeof(UserEventDetailPage)},
                {WebSyncPage, typeof(WebSyncPage)},
                {LocalSyncPage, typeof(LocalSyncPage)}
            };
    }
}