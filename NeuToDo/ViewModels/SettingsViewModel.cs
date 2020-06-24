using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models;
using NeuToDo.Models.SettingsModels;
using NeuToDo.Services;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NeuToDo.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly IPopupNavigationService _popupNavigationService;

        private readonly ISecureStorageProvider _secureStorageProvider;

        private readonly IEventModelStorageProvider _storageProvider;

        public SettingsViewModel(IPopupNavigationService popupNavigationService,
            ISecureStorageProvider secureStorageProvider,
            IEventModelStorageProvider eventModelStorageProvider)
        {
            _popupNavigationService = popupNavigationService;
            _secureStorageProvider = secureStorageProvider;
            _storageProvider = eventModelStorageProvider;
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
                settingItem.UserName = itemId;
                settingItem.LastUpdateTime = lastUpdateTime;
                settingItem.Button1Text = "更新";
                settingItem.IsBound = true;
            }
        }

        private RelayCommand<SettingItem> _command1;

        public RelayCommand<SettingItem> Command1 =>
            _command1 ??= new RelayCommand<SettingItem>(Command1Function);

        public void Command1Function(SettingItem item)
        {
            _popupNavigationService.PushAsync(PopupPageNavigationConstants.LoginPopupPage, item);
        }

        private RelayCommand<SettingItem> _command2;

        public RelayCommand<SettingItem> Command2 =>
            _command2 ??= new RelayCommand<SettingItem>(async (item) => await Command2Function(item));

        private async Task Command2Function(SettingItem item)
        {
            if (!item.IsBound) return;
            //TODO Popup Page 请先登录
            var itemType = item.ServerType;
            _secureStorageProvider.Remove(itemType + "Id");
            _secureStorageProvider.Remove(itemType + "Pd");
            _secureStorageProvider.Remove(itemType + "Time");
            switch (itemType)
            {
                case ServerType.Neu:
                    var neuStorage = await _storageProvider.GetEventModelStorage<NeuEvent>();
                    await neuStorage.ClearTableAsync();
                    _storageProvider.OnUpdateData();
                    break;
                case ServerType.Mooc:
                    var moocStorage = await _storageProvider.GetEventModelStorage<MoocEvent>();
                    await moocStorage.ClearTableAsync();
                    _storageProvider.OnUpdateData();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            item.UserName = string.Empty;
            item.LastUpdateTime = string.Empty;
            item.Button1Text = "关联";
            item.IsBound = false;
        }

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