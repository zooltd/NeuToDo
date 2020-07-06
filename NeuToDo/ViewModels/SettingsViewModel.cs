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

namespace NeuToDo.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly IPopupNavigationService _popupNavigationService;

        private readonly IAccountStorageService _accountStorageService;

        private readonly IStorageProvider _storageProvider;

        private readonly IAlertService _alertService;

        public SettingsViewModel(IPopupNavigationService popupNavigationService,
            IAccountStorageService accountStorageService,
            IStorageProvider storageProvider,
            IAlertService alertService)
        {
            _popupNavigationService = popupNavigationService;
            _accountStorageService = accountStorageService;
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
                _accountStorageService.AccountExist(platform.ServerType)))
            {
                platform.UserName = _accountStorageService.GetUserName(platform.ServerType);
                platform.LastUpdateTime = _accountStorageService.GetUpdateTime(platform.ServerType);
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

        private RelayCommand _importDb;

        public RelayCommand ImportDb =>
            _importDb ??= new RelayCommand(async () => await ImportDbFunction());

        private async Task ImportDbFunction()
        {
            try
            {
                var pickedFile = await CrossFilePicker.Current.PickFile();
                if (pickedFile == null) return;
                if (pickedFile.FileName != "events.sqlite3")
                {
                    _alertService.DisplayAlert("警告", "导入文件名应为\"events.sqlite3\"", "OK");
                    return;
                }

                if (File.Exists(StorageProvider.DbPath))
                {
                    File.Delete(StorageProvider.DbPath);
                    File.Copy(pickedFile.FilePath, StorageProvider.DbPath);
                }
                else
                {
                    File.Copy(pickedFile.FilePath, StorageProvider.DbPath);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task Command2Function(Platform p)
        {
            if (!p.IsBound)
            {
                _alertService.DisplayAlert("提示", "请先登录", "OK");
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

        private List<Platform> _platforms;

        public List<Platform> Platforms
        {
            get => _platforms;
            set => Set(nameof(Platforms), ref _platforms, value);
        }

        #endregion
    }
}