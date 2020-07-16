using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models;
using NeuToDo.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace NeuToDo.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IFetchSemesterDataService _fetchSemesterDataService;

        private readonly IPopupNavigationService _popupNavigationService;

        private readonly IAccountStorageService _accountStorageService;

        private readonly IEventModelStorage<NeuEvent> _neuStorage;

        private readonly IEventModelStorage<MoocEvent> _moocStorage;

        private readonly IDbStorageProvider _dbStorageProvider;


        public LoginViewModel(IPopupNavigationService popupNavigationService,
            IFetchSemesterDataService fetchSemesterDataService,
            IAccountStorageService accountStorageService,
            IDbStorageProvider dbStorageProvider)
        {
            _neuStorage = dbStorageProvider.GetEventModelStorage<NeuEvent>();
            _moocStorage = dbStorageProvider.GetEventModelStorage<MoocEvent>();
            _popupNavigationService = popupNavigationService;
            _fetchSemesterDataService = fetchSemesterDataService;
            _accountStorageService = accountStorageService;
            _dbStorageProvider = dbStorageProvider;
        }

        #region 绑定方法

        private RelayCommand _pageAppearingCommand;

        public RelayCommand PageAppearingCommand =>
            _pageAppearingCommand ??= new RelayCommand(async () =>
                await PageAppearingCommandFunction());

        public async Task PageAppearingCommandFunction()
        {
            if (ServerPlatform == null) return;
            Account = await _accountStorageService.GetAccountAsync(ServerPlatform.ServerType);
            Account ??= new Account();
        }


        private RelayCommand _onLogin;

        public RelayCommand OnLogin =>
            _onLogin ??= new RelayCommand((async () => { await OnLoginFunction(); }));

        public async Task OnLoginFunction()
        {
            await _popupNavigationService.PushAsync(PopupPageNavigationConstants.LoadingPopupPage);

            try
            {
                switch (ServerPlatform.ServerType)
                {
                    case ServerType.Neu:
                    {
                        var webCrawler = new NeuEventsGetter();
                        await webCrawler.Login(Account.UserName, Account.Password);
                        var eventList = await webCrawler.GetEventList();


                        //更新本地数据库
                        foreach (var course in eventList)
                            if (!await _neuStorage.ExistAsync(x => x.Uuid == course.Uuid))
                                await _neuStorage.InsertAsync(course);
                        _dbStorageProvider.OnUpdateData();

                        //更新账号存储
                        Account.LastUpdateTime = DateTime.Now.ToString(CultureInfo.CurrentCulture);
                        await _accountStorageService.SaveAccountAsync(ServerType.Neu, Account);
                        _accountStorageService.OnUpdateData();

                        await _popupNavigationService.PushAsync(PopupPageNavigationConstants.SuccessPopupPage);
                        await Task.Delay(1500);
                        await _popupNavigationService.PopAllAsync();
                        break;
                    }
                    case ServerType.Mooc:
                    {
                        var webCrawler = new MoocEventsGetter();
                        await webCrawler.Login(Account.UserName, Account.Password);
                        (Courses, EventList) = await webCrawler.GetEventList();
                        SelectedCourses = new ObservableCollection<object>();
                        await _popupNavigationService.PushAsync(PopupPageNavigationConstants.SelectPopupPage);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                await _popupNavigationService.PushAsync(PopupPageNavigationConstants.ErrorPopupPage);
                await Task.Delay(1500);
                await _popupNavigationService.PopAllAsync();
            }
        }

        #endregion

        #region 绑定属性

        public List<Course> Courses { get; private set; }


        private static ObservableCollection<object> _selectedCourses;

        public ObservableCollection<object> SelectedCourses
        {
            get => _selectedCourses;
            set => Set(nameof(SelectedCourses), ref _selectedCourses, value);
        }

        private ServerPlatform _serverPlatform;

        public ServerPlatform ServerPlatform
        {
            get => _serverPlatform;
            set => Set(nameof(ServerPlatform), ref _serverPlatform, value);
        }

        private Account _account;

        public Account Account
        {
            get => _account;
            set => Set(nameof(Account), ref _account, value);
        }

        public List<MoocEvent> EventList { get; set; }

        #endregion

        #region 绑定命令

        /// <summary>
        /// 保存课程命令。
        /// </summary>
        private RelayCommand _saveSelectedCoursesCommand;

        /// <summary>
        /// 保存课程命令。
        /// </summary>
        public RelayCommand SaveSelectedCoursesCommand =>
            _saveSelectedCoursesCommand ??= new RelayCommand(async () =>
                await SaveSelectedCoursesCommandFunction());

        public async Task SaveSelectedCoursesCommandFunction()
        {
            var resultList = (from Course course in SelectedCourses
                from moocEvent in EventList
                where moocEvent.Code == course.Code
                select moocEvent).ToList();

            //更新本地数据库
            foreach (var moocEvent in resultList)
                if (!await _moocStorage.ExistAsync(x => x.Uuid == moocEvent.Uuid))
                    await _moocStorage.InsertAsync(moocEvent);


            //更新账号存储
            Account.LastUpdateTime = DateTime.Now.ToString(CultureInfo.CurrentCulture);
            await _accountStorageService.SaveAccountAsync(ServerType.Mooc, Account);
            _accountStorageService.OnUpdateData();

            await _popupNavigationService.PushAsync(PopupPageNavigationConstants.SuccessPopupPage);
            await Task.Delay(1500);
            await _popupNavigationService.PopAllAsync();
            _dbStorageProvider.OnUpdateData();
        }

        #endregion
    }
}