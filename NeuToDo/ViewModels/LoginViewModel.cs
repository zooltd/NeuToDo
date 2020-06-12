using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models;
using NeuToDo.Models.SettingsModels;
using NeuToDo.Services;
using Rg.Plugins.Popup.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace NeuToDo.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private ILoginService _loginService;

        private readonly IPopupNavigationService _popupNavigationService;

        private readonly IEventModelStorageProvider _eventModelStorageProvider;

        public LoginViewModel(IPopupNavigationService popupNavigationService,
            IEventModelStorageProvider eventModelStorageProvider)
        {
            _popupNavigationService = popupNavigationService;
            _eventModelStorageProvider = eventModelStorageProvider;
        }

        #region 绑定方法

        private RelayCommand _pageAppearingCommand;

        public RelayCommand PageAppearingCommand
            => _pageAppearingCommand ??= new RelayCommand(async () =>
                await PageAppearingCommandFunction());

        private async Task PageAppearingCommandFunction()
        {
            if (SettingItem == null) return;
            await TryGetUserNameAsync(SettingItem.ServerType + "Id");
            await TryGetPasswordAsync(SettingItem.ServerType + "Pd");
        }

        private RelayCommand _onLogin;

        public RelayCommand OnLogin => _onLogin ??= new RelayCommand((async () =>
        {
            await _popupNavigationService.PushAsync(PopupPageNavigationConstants.LoadingPopupPage);

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
                await _popupNavigationService.PushAsync(PopupPageNavigationConstants.SuccessPopupPage);
                await UpdateSecureStorage();
            }
            else
            {
                await _popupNavigationService.PushAsync(PopupPageNavigationConstants.ErrorPopupPage);
            }

            await Task.Delay(1500);

            await PopupNavigation.Instance.PopAllAsync();
        }));

        #endregion

        #region 绑定属性

        private SettingItem _settingItem;

        public SettingItem SettingItem
        {
            get => _settingItem;
            set => Set(nameof(SettingItem), ref _settingItem, value);
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

        private async Task TryGetUserNameAsync(string key)
        {
            try
            {
                UserName = await SecureStorage.GetAsync(key);
            }
            catch (Exception e)
            {
                // ignored
                UserName = string.Empty;
            }
        }

        //
        private async Task TryGetPasswordAsync(string key)
        {
            try
            {
                Password = await SecureStorage.GetAsync(key);
            }
            catch (Exception e)
            {
                // ignored
                Password = string.Empty;
            }
        }

        private async Task UpdateSecureStorage()
        {
            try
            {
                await SecureStorage.SetAsync(SettingItem.ServerType + "Id", UserName);
                await SecureStorage.SetAsync(SettingItem.ServerType + "Pd", Password);
            }
            catch (Exception e)
            {
                // ignored
            }
        }
    }
}