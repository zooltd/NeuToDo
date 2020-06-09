using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NeuToDo.Views.Popup;

namespace NeuToDo.Services
{
    /// <summary>
    /// 内容导航服务接口。
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// 导航到页面。
        /// </summary>
        /// <param name="pageKey">页面键。</param>
        Task NavigateToContentPageAsync(string pageKey);

        /// <summary>
        /// 导航到弹出页。
        /// </summary>
        /// <param name="pageKey">页面键。</param>
        Task NavigateToPopupPageAsync(string pageKey);

        /// <summary>
        /// 导航到页面。
        /// </summary>
        /// <param name="pageKey">页面键。</param>
        /// <param name="parameter">参数。</param>
        Task NavigateToContentPageAsync(string pageKey, object parameter);

        /// <summary>
        /// 导航到弹出页。
        /// </summary>
        /// <param name="pageKey">页面键。</param>
        /// <param name="parameter">参数。</param>
        Task NavigateToPopupPageAsync(string pageKey, object parameter);
    }

    /// <summary>
    /// 内容导航常量。
    /// </summary>
    public static class PopupPageNavigationConstants
    {
        /// <summary>
        /// 登陆弹出页。
        /// </summary>
        public const string LoginPopupPage = nameof(Views.Popup.LoginPopupPage);

        /// <summary>
        /// 加载弹出页。
        /// </summary>
        public const string LoadingPopupPage = nameof(Views.Popup.LoadingPopupPage);

        /// <summary>
        /// 登陆弹出页。
        /// </summary>
        public const string SuccessPopupPage = nameof(Views.Popup.SuccessPopupPage);

        /// <summary>
        /// 登陆弹出页。
        /// </summary>
        public const string ErrorPopupPage = nameof(Views.Popup.ErrorPopupPage);

        /// <summary>
        /// 页面键-页面类型字典。
        /// </summary>
        public static readonly Dictionary<string, Type> PageKeyTypeDictionary = new Dictionary<string, Type>
        {
            {LoginPopupPage, typeof(LoginPopupPage)},
            {LoadingPopupPage, typeof(LoadingPopupPage)},
            {SuccessPopupPage, typeof(SuccessPopupPage)},
            {ErrorPopupPage, typeof(ErrorPopupPage)}
        };
    }
}