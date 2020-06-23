﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models.SettingsModels;
using NeuToDo.Services;
using Rg.Plugins.Popup.Services;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace NeuToDo.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly ILoginAndFetchDataService _loginAndFetchDataService;

        private readonly IPopupNavigationService _popupNavigationService;

        private readonly ISecureStorageProvider _secureStorageProvider;


        public LoginViewModel(IPopupNavigationService popupNavigationService,
            ILoginAndFetchDataService loginAndFetchDataService,
            ISecureStorageProvider secureStorageProvider)
        {
            _popupNavigationService = popupNavigationService;
            _loginAndFetchDataService = loginAndFetchDataService;
            _secureStorageProvider = secureStorageProvider;
        }

        #region 绑定方法

        private RelayCommand _pageAppearingCommand;

        public RelayCommand PageAppearingCommand
            => _pageAppearingCommand ??= new RelayCommand(async () => await PageAppearingCommandFunction());

        public async Task PageAppearingCommandFunction()
        {
            if (SettingItem == null) return;
            await TryGetUserNameAsync(SettingItem.ServerType + "Id");
            await TryGetPasswordAsync(SettingItem.ServerType + "Pd");
            await TryGetLastUpdateTimeAsync(SettingItem.ServerType + "Time");
        }


        private RelayCommand _onLogin;

        public RelayCommand OnLogin => _onLogin ??= new RelayCommand((async () => { await OnLoginFunction(); }));

        public async Task OnLoginFunction()
        {
            await _popupNavigationService.PushAsync(PopupPageNavigationConstants.LoadingPopupPage);

            var res = await _loginAndFetchDataService.LoginAndFetchDataAsync(SettingItem.ServerType, UserName,
                Password);


            if (res)
            {
                await UpdateSecureStorage();
                SettingItem.UserName = UserName;
                SettingItem.LastUpdateTime = LastUpdateTime;
                SettingItem.Button1Text = "更新";
                SettingItem.IsBound = true;
                await _popupNavigationService.PushAsync(PopupPageNavigationConstants.SuccessPopupPage);
            }
            else
            {
                await _popupNavigationService.PushAsync(PopupPageNavigationConstants.ErrorPopupPage);
            }

            await Task.Delay(1500);

            await PopupNavigation.Instance.PopAllAsync();
        }

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

        private string _lastUpdateTime;

        public string LastUpdateTime
        {
            get => _lastUpdateTime;
            set => Set(nameof(LastUpdateTime), ref _lastUpdateTime, value);
        }
        #endregion

        private async Task TryGetUserNameAsync(string key)
        {
            try
            {
                UserName = await _secureStorageProvider.GetAsync(key);
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
                Password = await _secureStorageProvider.GetAsync(key);
            }
            catch (Exception e)
            {
                // ignored
                Password = string.Empty;
            }
        }

        private async Task TryGetLastUpdateTimeAsync(string key)
        {
            try
            {
                LastUpdateTime = await _secureStorageProvider.GetAsync(key);
            }
            catch (Exception e)
            {
                // ignored
                LastUpdateTime = string.Empty;
            }
        }


        private async Task UpdateSecureStorage()
        {
            try
            {
                await _secureStorageProvider.SetAsync(SettingItem.ServerType + "Id", UserName);
                await _secureStorageProvider.SetAsync(SettingItem.ServerType + "Pd", Password);
                LastUpdateTime = DateTime.Now.ToString(CultureInfo.CurrentCulture);
                await _secureStorageProvider.SetAsync(SettingItem.ServerType + "Time",LastUpdateTime);
            }
            catch (Exception e)
            {
                // ignored
            }
        }
    }
}