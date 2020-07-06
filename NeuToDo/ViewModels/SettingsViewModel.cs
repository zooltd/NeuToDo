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

        private readonly IPreferenceStorageProvider _preferenceStorageProvider;

        private readonly IStorageProvider _storageProvider;

        private readonly IAlertService _alertService;

        public SettingsViewModel(IPopupNavigationService popupNavigationService,
            ISecureStorageProvider secureStorageProvider,
            IPreferenceStorageProvider preferenceStorageProvider,
            IStorageProvider storageProvider,
            IAlertService alertService)
        {
            _popupNavigationService = popupNavigationService;
            _secureStorageProvider = secureStorageProvider;
            _preferenceStorageProvider = preferenceStorageProvider;
            _storageProvider = storageProvider;
            _alertService = alertService;
            Platforms = Platform.Platforms;
        }

        private bool _isInit;

        #region 绑定方法

        private RelayCommand _pageAppearingCommand;

        public RelayCommand PageAppearingCommand => _pageAppearingCommand ??=
            new RelayCommand(PageAppearingCommandFunction);

        private void PageAppearingCommandFunction()
        {
            if (_isInit) return;
            foreach (var platform in Platforms.Where(platform =>
                _preferenceStorageProvider.ContainsKey(platform.ServerType + "Id")))
            {
                platform.UserName = _preferenceStorageProvider.Get(platform.ServerType + "Id", "");
                platform.LastUpdateTime = _preferenceStorageProvider.Get(platform.ServerType + "Time", "未知的时间");
                platform.Button1Text = "更新";
                platform.IsBound = true;
            }

            _isInit = true;
        }

        private RelayCommand<Platform> _command1;

        public RelayCommand<Platform> Command1 =>
            _command1 ??= new RelayCommand<Platform>(Command1Function);

        public void Command1Function(Platform item)
        {
            _popupNavigationService.PushAsync(PopupPageNavigationConstants.LoginPopupPage, item);
        }

        private RelayCommand<Platform> _command2;

        public RelayCommand<Platform> Command2 =>
            _command2 ??= new RelayCommand<Platform>(async (p) => await Command2Function(p));

        private async Task Command2Function(Platform p)
        {
            if (!p.IsBound)
            {
                _alertService.DisplayAlert("提示", "请先登录", "OK");
                return;
            }

            var itemType = p.ServerType;
            _preferenceStorageProvider.Remove(itemType + "Id");
            _secureStorageProvider.Remove(itemType + "Pd");
            _preferenceStorageProvider.Remove(itemType + "Time");
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

            p.UserName = string.Empty;
            p.LastUpdateTime = string.Empty;
            p.Button1Text = "关联";
            p.IsBound = false;
        }

        #endregion

        #region 绑定属性

        //
        // private List<SettingItemGroup> _settings;
        //
        // public List<SettingItemGroup> Settings
        // {
        //     get => _settings;
        //     set => Set(nameof(Settings), ref _settings, value);
        // }
        private List<Platform> _platforms;

        public List<Platform> Platforms
        {
            get => _platforms;
            set => Set(nameof(Platforms), ref _platforms, value);
        }

        #endregion
    }
}