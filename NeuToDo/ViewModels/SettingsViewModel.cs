using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models;
using NeuToDo.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace NeuToDo.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        #region 构造函数

        private readonly IPopupNavigationService _popupNavigationService;
        private readonly IAccountStorageService _accountStorageService;
        private readonly IDialogService _dialogService;
        private readonly IEventModelStorage<NeuEvent> _neuStorage;
        private readonly IEventModelStorage<MoocEvent> _moocStorage;
        private readonly IEventModelStorage<UserEvent> _userStorage;
        private readonly IDbStorageProvider _dbStorageProvider;
        private readonly IContentPageNavigationService _contentPageNavigationService;
        private readonly ICampusStorageService _campusStorageService;
        private readonly ICalendarStorageProvider _calendarStorageProvider;

        public SettingsViewModel(IPopupNavigationService popupNavigationService,
            IAccountStorageService accountStorageService,
            IDbStorageProvider dbStorageProvider,
            IDialogService dialogService,
            IContentPageNavigationService contentPageNavigationService,
            ICampusStorageService campusStorageService,
            ICalendarStorageProvider calendarStorageProvider)
        {
            _neuStorage = dbStorageProvider.GetEventModelStorage<NeuEvent>();
            _moocStorage = dbStorageProvider.GetEventModelStorage<MoocEvent>();
            _userStorage = dbStorageProvider.GetEventModelStorage<UserEvent>();
            _popupNavigationService = popupNavigationService;
            _accountStorageService = accountStorageService;
            _dbStorageProvider = dbStorageProvider;
            _dialogService = dialogService;
            _contentPageNavigationService = contentPageNavigationService;
            _campusStorageService = campusStorageService;
            _calendarStorageProvider = calendarStorageProvider;
            accountStorageService.UpdateData += OnUpdateAccount;
            campusStorageService.UpdateCampus += OnUpdateCampus;
        }

        #endregion

        private async void OnUpdateAccount(object sender, EventArgs e)
            => await LoadPlatforms();

        private void OnUpdateCampus(object sender, EventArgs e)
            => LoadCampus();

        private bool _isInit;

        #region 绑定方法

        /// <summary>
        /// 页面显示命令。
        /// </summary>
        private RelayCommand _pageAppearingCommand;

        /// <summary>
        /// 页面显示命令。
        /// </summary>
        public RelayCommand PageAppearingCommand => _pageAppearingCommand ??=
            new RelayCommand(async () => await PageAppearingCommandFunction());

        private async Task PageAppearingCommandFunction()
        {
            if (_isInit) return;

            LoadCampus();
            await LoadPlatforms();

            _isInit = true;
        }

        /// <summary>
        /// 加载校区
        /// </summary>
        private void LoadCampus()
        {
            Campus = _campusStorageService.GetCampus();
        }

        /// <summary>
        /// 加载平台。
        /// </summary>
        /// <returns></returns>
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
                    await _neuStorage.DeleteAllAsync();
                    _dbStorageProvider.OnUpdateData();
                    break;
                case ServerType.Mooc:
                    await _moocStorage.DeleteAllAsync();
                    _dbStorageProvider.OnUpdateData();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _accountStorageService.OnUpdateData();
        }

        /// <summary>
        /// 导航到备份页命令。
        /// </summary>
        private RelayCommand _navigateToBackupPageCommand;

        public RelayCommand NavigateToBackupPageCommand =>
            _navigateToBackupPageCommand ??= new RelayCommand(async () =>
                await _contentPageNavigationService.PushAsync(ContentNavigationConstants.BackupPage));


        /// <summary>
        /// 导航到同步页命令。
        /// </summary>
        private RelayCommand _navigateToSyncPageCommand;

        public RelayCommand NavigateToSyncPageCommand =>
            _navigateToSyncPageCommand ??= new RelayCommand(async () =>
                await _contentPageNavigationService.PushAsync(ContentNavigationConstants.SyncPage));

        /// <summary>
        /// 选择校区命令。
        /// </summary>
        private RelayCommand _selectCampus;

        /// <summary>
        /// 选择校区命令。
        /// </summary>
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

        /// <summary>
        /// 导出到本地日历命令。
        /// </summary>
        private RelayCommand _exportToLocalCalendar;

        /// <summary>
        /// 导出到本地日历命令。
        /// </summary>
        public RelayCommand ExportToLocalCalendar =>
            _exportToLocalCalendar ??= new RelayCommand(async () => await ExportToLocalCalendarFunction());

        private async Task ExportToLocalCalendarFunction()
        {
            var res = await _calendarStorageProvider.CheckPermissionsAsync();
            if (!res)
            {
                _dialogService.DisplayAlert("错误", "日历读写授权失败，请至系统设置内授予读写日历权限", "OK");
                return;
            }

            await _popupNavigationService.PushAsync(PopupPageNavigationConstants.LoadingPopupPage);

            await _calendarStorageProvider.DeleteCalendarsAsync(AppInfo.Name, "NeuToDo");

            await _calendarStorageProvider.AddCalendarAsync(AppInfo.Name, "NeuToDo");

            var myCalendar = await _calendarStorageProvider.GetCalendarsAsync("NeuToDo");

            if (myCalendar == null)
            {
                _dialogService.DisplayAlert("错误", "创建日历失败", "OK");
                await _popupNavigationService.PopAllAsync();
                return;
            }

            var totalEventList = new List<EventModel>();
            totalEventList.AddRange(await _neuStorage.GetAllAsync(x => !x.IsDeleted));
            totalEventList.AddRange(await _moocStorage.GetAllAsync(x => !x.IsDeleted));
            totalEventList.AddRange(await _userStorage.GetAllAsync(x => !x.IsDeleted));

            var calendarEvents = _calendarStorageProvider.ToDoEventsToCalenderEvents(totalEventList);

            foreach (var calendarEvent in calendarEvents)
            {
                await _calendarStorageProvider.AddOrUpdateEventAsync(myCalendar, calendarEvent);
            }

            await _popupNavigationService.PopAllAsync();
            _dialogService.DisplayAlert("提示", "导出日历成功", "OK");
        }

        /// <summary>
        /// 删除本地日历命令。
        /// </summary>
        private RelayCommand _deleteLocalCalendar;

        /// <summary>
        /// 删除本地日历命令。
        /// </summary>
        public RelayCommand DeleteLocalCalendar =>
            _deleteLocalCalendar ??= new RelayCommand(async () => await DeleteLocalCalendarFunction());

        private async Task DeleteLocalCalendarFunction()
        {
            var res = await _calendarStorageProvider.CheckPermissionsAsync();
            if (!res)
            {
                _dialogService.DisplayAlert("错误", "日历读写授权失败，请至系统设置内授予读写日历权限", "OK");
                return;
            }

            await _popupNavigationService.PushAsync(PopupPageNavigationConstants.LoadingPopupPage);
            await _calendarStorageProvider.DeleteCalendarsAsync(AppInfo.Name, "NeuToDo");
            await _popupNavigationService.PopAllAsync();
            _dialogService.DisplayAlert("提示", "删除日历成功", "OK");
        }

        #endregion

        #region 绑定属性

        /// <summary>
        /// 平台。
        /// </summary>
        private List<ServerPlatform> _serverPlatforms;

        /// <summary>
        /// 平台。
        /// </summary>
        public List<ServerPlatform> ServerPlatforms
        {
            get => _serverPlatforms;
            set => Set(nameof(ServerPlatforms), ref _serverPlatforms, value);
        }

        /// <summary>
        /// 校区。
        /// </summary>
        private Campus _campus;

        /// <summary>
        /// 校区。
        /// </summary>
        public Campus Campus
        {
            get => _campus;
            set => Set(nameof(Campus), ref _campus, value);
        }

        #endregion
    }
}