using System;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Services;
using NeuToDo.Views.Popup;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Plugin.Calendar.Models;

namespace NeuToDo.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        public LoginViewModel()
        {
            var getUserName = TryGetUserName();
            getUserName.Wait();
            var getPassword = TryGetPassword();
            getPassword.Wait();
        }

        #region 绑定方法

        private RelayCommand _onLogin;

        public RelayCommand OnLogin => _onLogin ?? (_onLogin = new RelayCommand((async () =>
        {
            var loadingPage = new LoadingPopupPage();
            await PopupNavigation.Instance.PushAsync(loadingPage);
            var getter = new NeuSyllabusGetter(UserName, Password);
            await getter.WebCrawler();
            new ViewModelLocator().ToDoCalendarViewModel.UpdateEvents();
            var successPage = new LoginSuccessPopupPage();
            await PopupNavigation.Instance.PushAsync(successPage);
            await Task.Delay(2000);
            try
            {
                await SecureStorage.SetAsync("NeuId", UserName);
                await SecureStorage.SetAsync("NeuPd", Password);
            }
            catch (Exception e)
            {
                //ignored
            }

            // await PopupNavigation.Instance.RemovePageAsync(loadingPage);
            await PopupNavigation.Instance.PopAllAsync();
        })));

        #endregion

        #region 绑定属性

        private static string _userName;

        public string UserName
        {
            get => _userName;
            set => Set(nameof(UserName), ref _userName, value);
        }


        private string _password;

        public string Password
        {
            get => _password;
            set => Set(nameof(Password), ref _password, value);
        }

        #endregion

        private async Task TryGetUserName()
        {
            try
            {
                UserName = await SecureStorage.GetAsync("NueId");
            }
            catch (Exception e)
            {
                // ignored
                UserName = string.Empty;
            }
        }

        private async Task TryGetPassword()
        {
            try
            {
                Password = await SecureStorage.GetAsync("NuePd");
            }
            catch (Exception e)
            {
                // ignored
                Password = string.Empty;
            }
        }
    }
}