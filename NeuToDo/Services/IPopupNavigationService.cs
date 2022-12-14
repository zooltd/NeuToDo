using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NeuToDo.Views.Popup;

namespace NeuToDo.Services
{
    public interface IPopupNavigationService
    {
        Task PushAsync(string pageKey);

        Task PushAsync(string pageKey, object parameter);

        Task PopAllAsync();
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
        /// 成功弹出页。
        /// </summary>
        public const string SuccessPopupPage = nameof(Views.Popup.SuccessPopupPage);

        /// <summary>
        /// 错误弹出页。
        /// </summary>
        public const string ErrorPopupPage = nameof(Views.Popup.ErrorPopupPage);

        /// <summary>
        /// 选择弹出页。
        /// </summary>
        public const string SelectPopupPage = nameof(Views.Popup.SelectPopupPage);


        /// <summary>
        /// Neu课程编辑——周数选择页
        /// </summary>
        public const string WeekNoSelectPopupPage = nameof(Views.Popup.WeekNoSelectPopupPage);

        /// <summary>
        /// WebDAV登录页
        /// </summary>
        public const string SyncLoginPage = nameof(Views.Popup.SyncLoginPage);


        /// <summary>
        /// 页面键-页面类型字典。
        /// </summary>
        public static readonly Dictionary<string, Type> PageKeyTypeDictionary = new Dictionary<string, Type>
        {
            {LoginPopupPage, typeof(LoginPopupPage)},
            {LoadingPopupPage, typeof(LoadingPopupPage)},
            {SuccessPopupPage, typeof(SuccessPopupPage)},
            {ErrorPopupPage, typeof(ErrorPopupPage)},
            {SelectPopupPage, typeof(SelectPopupPage)},
            {WeekNoSelectPopupPage, typeof(WeekNoSelectPopupPage)},
            {SyncLoginPage, typeof(SyncLoginPage)},
        };
    }
}