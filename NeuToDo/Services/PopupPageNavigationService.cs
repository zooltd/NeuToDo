using GalaSoft.MvvmLight.Ioc;
using NeuToDo.Models.SettingsModels;
using NeuToDo.ViewModels;
using NeuToDo.Views.Popup;
using Rg.Plugins.Popup.Services;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NeuToDo.Services
{
    public class PopupPageNavigationService
    {
        public async Task Login(ServerType type)
        {
            SimpleIoc.Default.Unregister<LoginViewModel>();
            SimpleIoc.Default.Register(() => new LoginViewModel(type));
            //TODO UWP下有问题
            await PopupNavigation.Instance.PushAsync(new LoginPopupPage());
        }

        public static async Task Loading()
        {
            await PopupNavigation.Instance.PushAsync(new LoadingPopupPage());
        }

        public static async Task SuccessMessage()
        {
            await PopupNavigation.Instance.PushAsync(new LoginSuccessPopupPage());
        }

        public static async Task ErrorMessage()
        {
            await PopupNavigation.Instance.PushAsync(new LoginErrorPopupPage());
        }
    }
}