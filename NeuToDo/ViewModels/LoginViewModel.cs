using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models;
using NeuToDo.Models.SettingsModels;
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
        private readonly ILoginAndFetchDataService _loginAndFetchDataService;

        private readonly IPopupNavigationService _popupNavigationService;

        private readonly IAccountStorageService _accountStorageService;

        private readonly IEventModelStorage<MoocEvent> _moocStorage;

        private readonly IDbStorageProvider _dbStorageProvider;

        private string _currTime;

        public LoginViewModel(IPopupNavigationService popupNavigationService,
            ILoginAndFetchDataService loginAndFetchDataService,
            IAccountStorageService accountStorageService,
            IDbStorageProvider dbStorageProvider)
        {
            _moocStorage = dbStorageProvider.GetEventModelStorage<MoocEvent>();
            _popupNavigationService = popupNavigationService;
            _loginAndFetchDataService = loginAndFetchDataService;
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
            UserName = _accountStorageService.GetUserName(ServerPlatform.ServerType);
            Password = await _accountStorageService.GetPasswordAsync(ServerPlatform.ServerType);
        }


        private RelayCommand _onLogin;

        public RelayCommand OnLogin =>
            _onLogin ??= new RelayCommand((async () => { await OnLoginFunction(); }));

        public async Task OnLoginFunction()
        {
            await _popupNavigationService.PushAsync(PopupPageNavigationConstants
                .LoadingPopupPage);

            var res =
                await _loginAndFetchDataService.LoginAndFetchDataAsync(
                    ServerPlatform.ServerType, UserName, Password);

            if (res)
            {
                _currTime = DateTime.Now.ToString(CultureInfo.CurrentCulture);
                await _accountStorageService.SaveAccountAsync(ServerPlatform.ServerType, UserName, Password, _currTime);
                ServerPlatform.UserName = UserName;
                ServerPlatform.LastUpdateTime = _currTime;
                ServerPlatform.Button1Text = "更新";
                ServerPlatform.IsBound = true;
                if (ServerPlatform.ServerType == ServerType.Mooc)
                {
                    Courses = MoocInfoGetter.CourseList;
                    if (Courses.Any())
                    {
                        SelectedCourses = new ObservableCollection<object>();
                        await _popupNavigationService.PushAsync(
                            PopupPageNavigationConstants.SelectPopupPage);
                    }
                }
                else
                {
                    await _popupNavigationService.PushAsync(
                        PopupPageNavigationConstants.SuccessPopupPage);
                    await Task.Delay(1500);
                    await _popupNavigationService.PopAllAsync();
                }
            }
            else
            {
                await _popupNavigationService.PushAsync(
                    PopupPageNavigationConstants.ErrorPopupPage);
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
                from moocEvent in MoocInfoGetter.EventList
                where moocEvent.Code == course.Code
                select moocEvent).ToList();
            await _moocStorage.MergeAsync(resultList);
            await _popupNavigationService.PushAsync(PopupPageNavigationConstants
                .SuccessPopupPage);
            await Task.Delay(1500);
            await _popupNavigationService.PopAllAsync();
            _dbStorageProvider.OnUpdateData();
        }

        #endregion
    }
}