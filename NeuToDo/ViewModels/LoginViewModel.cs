using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using NeuToDo.Models.SettingsModels;
using NeuToDo.Services;
using Rg.Plugins.Popup.Services;
using System;
using System.Threading.Tasks;
using NeuToDo.Models;
using Xamarin.Essentials;

namespace NeuToDo.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private ILoginService _loginService;

        private readonly INavigationService _navigationService;

        private readonly IEventModelStorageProvider _eventModelStorageProvider;

        public LoginViewModel(INavigationService navigationService,
            IEventModelStorageProvider eventModelStorageProvider)
        {
            _navigationService = navigationService;
            _eventModelStorageProvider = eventModelStorageProvider;
            // FetchSecureStorage();
            // SetProperties(Type);
        }
        //
        // private void FetchSecureStorage()
        // {
        //     var getUserName = TryGetUserName();
        //     getUserName.Wait();
        //     var getPassword = TryGetPassword();
        //     getPassword.Wait();
        // }

        // private async Task UpdateSecureStorage()
        // {
        //     try
        //     {
        //         await SecureStorage.SetAsync(_type + "Id", UserName);
        //         await SecureStorage.SetAsync(_type + "Pd", Password);
        //     }
        //     catch (Exception e)
        //     {
        //         //ignored
        //     }
        // }

        #region 绑定方法

        private RelayCommand _onLogin;

        public RelayCommand OnLogin => _onLogin ?? (_onLogin = new RelayCommand((async () =>
        {
            await _navigationService.NavigateToPopupPageAsync(PopupPageNavigationConstants.LoadingPopupPage);

            switch (SettingItem.ServerType)
            {
                case ServerType.Neu:
                    _loginService = new NeuLoginService(_eventModelStorageProvider);
                    break;
                case ServerType.Mooc:
                    break;
                case ServerType.Blackboard:
                    break;
                case ServerType.WebDav:
                    break;
                case ServerType.Github:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var res = await _loginService.LoginAndFetchDataAsync(UserName, Password);

            if (res)
            {
                await _navigationService.NavigateToPopupPageAsync(PopupPageNavigationConstants.SuccessPopupPage);
                // await UpdateSecureStorage();
            }
            else
            {
                await _navigationService.NavigateToPopupPageAsync(PopupPageNavigationConstants.SuccessPopupPage);
            }

            await Task.Delay(1500);

            await PopupNavigation.Instance.PopAllAsync();
        })));

        #endregion

        #region 绑定属性

        private SettingItem _settingItem;

        public SettingItem SettingItem
        {
            get => _settingItem;
            set
            {
                Set(nameof(Type), ref _settingItem, value);
                Console.WriteLine("debug");
            }
        }

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

        // private async Task TryGetUserName()
        // {
        //     try
        //     {
        //         UserName = await SecureStorage.GetAsync(_type + "Id");
        //     }
        //     catch (Exception e)
        //     {
        //         // ignored
        //         UserName = string.Empty;
        //     }
        // }
        //
        // private async Task TryGetPassword()
        // {
        //     try
        //     {
        //         Password = await SecureStorage.GetAsync(_type + "Pd");
        //     }
        //     catch (Exception e)
        //     {
        //         // ignored
        //         Password = string.Empty;
        //     }
        // }
    }
}