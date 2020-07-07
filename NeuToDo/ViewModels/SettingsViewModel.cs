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

        public SettingsViewModel(IPopupNavigationService popupNavigationService,
            IAccountStorageService accountStorageService,
            IStorageProvider storageProvider,
            IDialogService dialogService)
        {
            _popupNavigationService = popupNavigationService;
            _accountStorageService = accountStorageService;
            _storageProvider = storageProvider;
            _dialogService = dialogService;
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
                string[] fileTypes = Device.RuntimePlatform switch
                {
                    Device.Android => null,
                    Device.UWP => new[] {".sqlite3"},
                    _ => null
                };
                var pickedFile = await CrossFilePicker.Current.PickFile(fileTypes);
                if (pickedFile == null) return;
                if (pickedFile.FileName != "events.sqlite3")
                {
                    _dialogService.DisplayAlert("警告", "导入文件名应为\"events.sqlite3\"", "OK");
                    return;
                }

                await _storageProvider.CloseConnectionAsync();

                if (File.Exists(StorageProvider.DbPath))
                    File.Delete(StorageProvider.DbPath);
                var stream = pickedFile.GetStream();
                using (var fileStream = File.Create(StorageProvider.DbPath))
                    CopyStream(stream, fileStream);
                _storageProvider.OnUpdateData();
            }
            catch (Exception e)
            {
                _dialogService.DisplayAlert("警告", e.ToString(), "Ok");
            }
        }

        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[16 * 1024];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                output.Write(buffer, 0, read);
        }

        private async Task Command2Function(Platform p)
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

        private List<Platform> _platforms;

        public List<Platform> Platforms
        {
            get => _platforms;
            set => Set(nameof(Platforms), ref _platforms, value);
        }

        #endregion
    }
}