using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models;
using NeuToDo.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeuToDo.Views;

namespace NeuToDo.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly IPopupNavigationService _popupNavigationService;
        private readonly IAccountStorageService _accountStorageService;
        private readonly IDialogService _dialogService;
        private readonly IEventModelStorage<NeuEvent> _neuStorage;
        private readonly IEventModelStorage<MoocEvent> _moocStorage;
        private readonly IDbStorageProvider _dbStorageProvider;
        private readonly IContentPageNavigationService _contentPageNavigationService;
        private readonly ICampusStorageService _campusStorageService;

        public SettingsViewModel(IPopupNavigationService popupNavigationService,
            IAccountStorageService accountStorageService,
            IDbStorageProvider dbStorageProvider,
            IDialogService dialogService,
            IContentPageNavigationService contentPageNavigationService,
            ICampusStorageService campusStorageService)
        {
            _neuStorage = dbStorageProvider.GetEventModelStorage<NeuEvent>();
            _moocStorage = dbStorageProvider.GetEventModelStorage<MoocEvent>();
            _popupNavigationService = popupNavigationService;
            _accountStorageService = accountStorageService;
            _dbStorageProvider = dbStorageProvider;
            _dialogService = dialogService;
            _contentPageNavigationService = contentPageNavigationService;
            _campusStorageService = campusStorageService;
            accountStorageService.UpdateData += OnUpdateAccount;
        }

        private async void OnUpdateAccount(object sender, EventArgs e)
        {
            await LoadPlatforms();
        }

        private bool _isInit;

        #region 绑定方法

        private RelayCommand _pageAppearingCommand;

        public RelayCommand PageAppearingCommand => _pageAppearingCommand ??=
            new RelayCommand(async () => await PageAppearingCommandFunction());

        private async Task PageAppearingCommandFunction()
        {
            if (_isInit) return;

            Campus = _campusStorageService.GetCampus();

            await LoadPlatforms();

            _isInit = true;
        }

        private async Task LoadPlatforms()
        {
            var defaultPlatforms = ServerPlatform.ServerPlatforms;

            foreach (var platform in defaultPlatforms)
            {
                var platformAccount = await _accountStorageService.GetAccountAsync(platform.ServerType);
                if (platformAccount == null) continue;
                platform.UserName = platformAccount.UserName;
                platform.LastUpdateTime = platformAccount.LastUpdateTime;
                platform.Button1Text = "更新";
                platform.IsBound = true;
            }

            ServerPlatforms = new List<ServerPlatform>(defaultPlatforms);
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


        private async Task Command2Function(ServerPlatform p)
        {
            if (!p.IsBound)
            {
                _dialogService.DisplayAlert("提示", "请先登录", "OK");
                return;
            }

            var res = await _dialogService.DisplayAlert("警告", "将清空数据库(未同步的数据将丢失)", "确认", "取消");
            if (!res) return;

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

            _accountStorageService.OnUpdateData();
        }

        private RelayCommand _navigateToBackupPageCommand;

        public RelayCommand NavigateToBackupPageCommand =>
            _navigateToBackupPageCommand ??= new RelayCommand(async () =>
                await _contentPageNavigationService.PushAsync(ContentNavigationConstants.BackupPage));


        private RelayCommand _navigateToSyncPageCommand;

        public RelayCommand NavigateToSyncPageCommand =>
            _navigateToSyncPageCommand ??= new RelayCommand(async () =>
                await _contentPageNavigationService.PushAsync(ContentNavigationConstants.SyncPage));

        private RelayCommand _selectCampus;

        public RelayCommand SelectCampus =>
            _selectCampus ??= new RelayCommand(async () => await SelectCampusFunction());

        private async Task SelectCampusFunction()
        {
            var res = await _dialogService.DisplayActionSheet("选择校区", "取消", null, "南湖", "浑南");
            if (res == "取消" || res == null) return;
            if (res != Campus.ToString())
            {
                Campus = (Campus) Enum.Parse(typeof(Campus), res);
                _campusStorageService.SaveCampus(Campus);
            }

            _dialogService.DisplayAlert("提示", "已保存", "OK");
        }

        #endregion

        #region 绑定属性

        private List<ServerPlatform> _serverPlatforms;

        public List<ServerPlatform> ServerPlatforms
        {
            get => _serverPlatforms;
            set => Set(nameof(ServerPlatforms), ref _serverPlatforms, value);
        }

        private Campus _campus;

        public Campus Campus
        {
            get => _campus;
            set => Set(nameof(Campus), ref _campus, value);
        }

        #endregion
    }
}