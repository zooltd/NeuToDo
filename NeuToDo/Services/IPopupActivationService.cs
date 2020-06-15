using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;

namespace NeuToDo.Services
{
    /// <summary>
    /// 内容页激活服务接口。
    /// </summary>
    public interface IPopupActivationService
    {
        /// <summary>
        /// 激活内容页。
        /// </summary>
        /// <param name="pageKey">页面键。</param>
        // ContentPage ActivateContentPage(string pageKey);

        PopupPage ActivatePopupPage(string pageKey);
    }
}