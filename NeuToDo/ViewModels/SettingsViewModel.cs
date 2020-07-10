using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models;
using NeuToDo.Models.SettingsModels;
using NeuToDo.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeuToDo.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly IPopupNavigationService _popupNavigationService;
        private readonly IAccountStorageService _accountStorageService;
        private readonly IDialogService _dialogService;
        private readonly IBackupService _backupService;
        private readonly IEventModelStorage<NeuEvent> _neuStorage;
        private readonly IEventModelStorage<MoocEvent> _moocStorage;
        private readonly IDbStorageProvider _dbStorageProvider;
        private readonly IContentPageNavigationService _contentPageNavigationService;

        public SettingsViewModel(IPopupNavigationService popupNavigationService,
            IAccountStorageService accountStorageService,
            IDbStorageProvider dbStorageProvider,
            IDialogService dialogService,
            IBackupService backupService,
            IContentPageNavigationService contentPageNavigationService)
        {
            _neuStorage = dbStorageProvider.GetEventModelStorage<NeuEvent>();
            _moocStorage = dbStorageProvider.GetEventModelStorage<MoocEvent>();
            _popupNavigationService = popupNavigationService;
            _accountStorageService = accountStorageService;
            _dbStorageProvider = dbStorageProvider;
            _dialogService = dialogService;
            _backupService = backupService;
            _contentPageNavigationService = contentPageNavigationService;
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
                _dbStorageProvider.OnUpdateData();
            }
            catch (Exception e)
            {
                _dialogService.DisplayAlert("警告", e.ToString(), "Ok");
            }
        }

        private RelayCommand _exportDb;

        public RelayCommand ExportDb =>
            _exportDb ??= new RelayCommand(async () => await ExportDbFunction());

        private async Task ExportDbFunction()
        {
            try
            {
                var dest = await _backupService.ExportAsync();
                _dialogService.DisplayAlert("提示", $"保存在{dest}", "OK");
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

                    await _neuStorage.ClearTableAsync();
                    _dbStorageProvider.OnUpdateData();
                    break;
                case ServerType.Mooc:
                    await _moocStorage.ClearTableAsync();
                    _dbStorageProvider.OnUpdateData();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            p.UserName = string.Empty;
            p.LastUpdateTime = string.Empty;
            p.Button1Text = "关联";
            p.IsBound = false;
        }

        private RelayCommand _webSyncCommand;

        public RelayCommand WebSyncCommand =>
            _webSyncCommand ??= new RelayCommand(async () =>
                await _contentPageNavigationService.PushAsync(ContentNavigationConstants.WebSyncPage));

        private RelayCommand _localSyncCommand;

        public RelayCommand LocalSyncCommand =>
            _localSyncCommand ??= new RelayCommand(async () =>
                await _contentPageNavigationService.PushAsync(ContentNavigationConstants.LocalSyncPage));

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