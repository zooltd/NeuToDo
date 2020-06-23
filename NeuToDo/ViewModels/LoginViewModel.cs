using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models.SettingsModels;
using NeuToDo.Services;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using NeuToDo.Models;

namespace NeuToDo.ViewModels {
    public class LoginViewModel : ViewModelBase {
        private readonly ILoginAndFetchDataService _loginAndFetchDataService;

        private readonly IPopupNavigationService _popupNavigationService;

        private readonly ISecureStorageProvider _secureStorageProvider;

        private readonly IEventModelStorageProvider _storageProvider;

        public LoginViewModel(IPopupNavigationService popupNavigationService,
            ILoginAndFetchDataService loginAndFetchDataService,
            ISecureStorageProvider secureStorageProvider,
            IEventModelStorageProvider eventModelStorageProvider) {
            _popupNavigationService = popupNavigationService;
            _loginAndFetchDataService = loginAndFetchDataService;
            _secureStorageProvider = secureStorageProvider;
            _storageProvider = eventModelStorageProvider;
        }

        #region 绑定方法

        private RelayCommand _pageAppearingCommand;

        public RelayCommand PageAppearingCommand =>
            _pageAppearingCommand ??= new RelayCommand(async () =>
                await PageAppearingCommandFunction());

        public async Task PageAppearingCommandFunction() {
            if (SettingItem == null)
                return;
            await TryGetUserNameAsync(SettingItem.ServerType + "Id");
            await TryGetPasswordAsync(SettingItem.ServerType + "Pd");
        }

        private RelayCommand _onLogin;

        public RelayCommand OnLogin =>
            _onLogin ??= new RelayCommand((async () => {
                await OnLoginFunction();
            }));

        public async Task OnLoginFunction() {
            await _popupNavigationService.PushAsync(PopupPageNavigationConstants
                .LoadingPopupPage);

            var res =
                await _loginAndFetchDataService.LoginAndFetchDataAsync(
                    SettingItem.ServerType, UserName, Password);

            if (res) {
                await UpdateSecureStorage();
                SettingItem.Detail = $"已关联用户名{UserName}";
                SettingItem.Button1Text = "更新";
                await _popupNavigationService.PushAsync(
                    PopupPageNavigationConstants.SuccessPopupPage);
                await Task.Delay(1500);

                Courses = MoocInfoGetter.CourseList;
                if (Courses.Any()) {
                    await _popupNavigationService.PushAsync(
                        PopupPageNavigationConstants.SelectPopupPage);
                }
            } else {
                await _popupNavigationService.PushAsync(
                    PopupPageNavigationConstants.ErrorPopupPage);
            }

            // await PopupNavigation.Instance.PopAllAsync();
        }

        #endregion

        #region 绑定属性

        public IEnumerable<Course> Courses { get; private set; }

        public ObservableCollection<Course> SelectedCourses { get; set; }


        private SettingItem _settingItem;

        public SettingItem SettingItem {
            get => _settingItem;
            set => Set(nameof(SettingItem), ref _settingItem, value);
        }

        private static string _userName;

        public string UserName {
            get => _userName;
            set => Set(nameof(UserName), ref _userName, value);
        }

        private string _password;

        public string Password {
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

        public async Task SaveSelectedCoursesCommandFunction() {
            var resultList =
                (from course in SelectedCourses
                    from moocEvent in MoocInfoGetter.EventList
                    where moocEvent.Code == course.Code select moocEvent)
                .ToList();

            var moocStorage =
                await _storageProvider.GetEventModelStorage<MoocEvent>();
            await moocStorage.ClearTableAsync();
            await moocStorage.InsertAllAsync(MoocInfoGetter.EventList);
        }

        #endregion

        private async Task TryGetUserNameAsync(string key) {
            try {
                UserName = await _secureStorageProvider.GetAsync(key);
            } catch (Exception e) {
                // ignored
                UserName = string.Empty;
            }
        }

        //
        private async Task TryGetPasswordAsync(string key) {
            try {
                Password = await _secureStorageProvider.GetAsync(key);
            } catch (Exception e) {
                // ignored
                Password = string.Empty;
            }
        }

        private async Task UpdateSecureStorage() {
            try {
                await _secureStorageProvider.SetAsync(
                    SettingItem.ServerType + "Id", UserName);
                await _secureStorageProvider.SetAsync(
                    SettingItem.ServerType + "Pd", Password);
            } catch (Exception e) {
                // ignored
            }
        }
    }
}