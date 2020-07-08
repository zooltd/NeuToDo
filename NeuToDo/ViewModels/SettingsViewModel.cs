using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models;
using NeuToDo.Models.SettingsModels;
using NeuToDo.Services;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;

namespace NeuToDo.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly IPopupNavigationService _popupNavigationService;

        private readonly IAccountStorageService _accountStorageService;

        private readonly IStorageProvider _storageProvider;

        private readonly IDialogService _dialogService;

        private readonly IBackupService _backupService;

        public SettingsViewModel(IPopupNavigationService popupNavigationService,
            IAccountStorageService accountStorageService,
            IStorageProvider storageProvider,
            IDialogService dialogService,
            IBackupService backupService)
        {
            _popupNavigationService = popupNavigationService;
            _accountStorageService = accountStorageService;
            _storageProvider = storageProvider;
            _dialogService = dialogService;
            _backupService = backupService;
            ServerPlatforms = ServerPlatform.ServerPlatforms;
        }

        private bool _isInit;

        #region 绑定方法

        private RelayCommand _pageAppearingCommand;

        public RelayCommand PageAppearingCommand => _pageAppearingCommand ??=
            new RelayCommand(PageAppearingCommandFunction);

        private void PageAppearingCommandFunction()
        {
            if (_isInit) return;
            foreach (var platform in ServerPlatforms.Where(platform =>
                _accountStorageService.AccountExist(platform.ServerType)))
            {
                platform.UserName = _accountStorageService.GetUserName(platform.ServerType);
                platform.LastUpdateTime = _accountStorageService.GetUpdateTime(platform.ServerType);
                platform.Button1Text = "更新";
                platform.IsBound = true;
            }

            _isInit = true;
        }

        private RelayCommand<ServerPlatform> _command1;

        public RelayCommand<ServerPlatform> Command1 =>
            _command1 ??= new RelayCommand<ServerPlatform>(Command1Function);

        public void Command1Function(ServerPlatform item)
        {
            _popupNavigationService.PushAsync(PopupPageNavigationConstants.LoginPopupPage, item);
        }

        private RelayCommand<ServerPlatform> _command2;

        public RelayCommand<ServerPlatform> Command2 =>
            _command2 ??= new RelayCommand<ServerPlatform>(async (p) => await Command2Function(p));

        private RelayCommand _importDb;

        public RelayCommand ImportDb =>
            _importDb ??= new RelayCommand(async () => await ImportDbFunction());

        private async Task ImportDbFunction()
        {
            try
            {
                await _backupService.ImportAsync(new List<FileType> {FileType.Sqlite});
                _storageProvider.OnUpdateData();
            }
            catch (Exception e)
            {
                _dialogService.DisplayAlert("警告", e.ToString(), "Ok");
            }
        }


        private async Task Command2Function(ServerPlatform p)
        {
            if (!p.IsBound)
            {
                _dialogService.DisplayAlert("提示", "请先登录", "OK");
                return;
            }

            var itemType = p.ServerType;
            _accountStorageService.RemoveAccountHistory(itemType);
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

        private List<ServerPlatform> _serverPlatforms;

        public List<ServerPlatform> ServerPlatforms
        {
            get => _serverPlatforms;
            set => Set(nameof(ServerPlatforms), ref _serverPlatforms, value);
        }

        #endregion
    }
}