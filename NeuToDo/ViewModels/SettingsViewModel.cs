using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models;
using NeuToDo.Models.SettingsModels;
using NeuToDo.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeuToDo.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly IPopupNavigationService _popupNavigationService;

        private readonly ISecureStorageProvider _secureStorageProvider;

        public SettingsViewModel(IPopupNavigationService popupNavigationService,
            ISecureStorageProvider secureStorageProvider)
        {
            _popupNavigationService = popupNavigationService;
            _secureStorageProvider = secureStorageProvider;
            Settings = SettingItemGroup.SettingGroup;
        }


        #region 绑定方法

        private RelayCommand _pageAppearingCommand;

        public RelayCommand PageAppearingCommand => _pageAppearingCommand ??=
            new RelayCommand(async () => await PageAppearingCommandFunction());

        private async Task PageAppearingCommandFunction()
        {
            foreach (var settingItem in Settings.SelectMany(settingItemGroup => settingItemGroup))
            {
                string itemId;
                if ((itemId = await _secureStorageProvider.GetAsync(settingItem.ServerType + "Id")) == null) continue;
                var lastUpdateTime = await _secureStorageProvider.GetAsync(settingItem.ServerType + "Time");
                settingItem.Detail = $"已关联用户名: {itemId}, 更新时间: {lastUpdateTime}";
                settingItem.Button1Text = "更新";
            }
        }

        private RelayCommand<SettingItem> _command1;

        public RelayCommand<SettingItem> Command1 =>
            _command1 ??= new RelayCommand<SettingItem>(Command1Function);

        public void Command1Function(SettingItem item)
        {
            _popupNavigationService.PushAsync(PopupPageNavigationConstants.LoginPopupPage, item);
        }

        private RelayCommand _command2;

        public RelayCommand Command2 =>
            _command2 ??= new RelayCommand(() => { });

        #endregion

        #region 绑定属性

        private List<SettingItemGroup> _settings;

        public List<SettingItemGroup> Settings
        {
            get => _settings;
            set => Set(nameof(Settings), ref _settings, value);
        }

        #endregion
    }
}