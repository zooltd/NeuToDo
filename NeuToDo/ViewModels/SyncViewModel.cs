using System;
using System.IO;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models;
using NeuToDo.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace NeuToDo.ViewModels
{
    public class SyncViewModel : ViewModelBase
    {
        private readonly IAccountStorageService _accountStorageService;
        private readonly IPopupNavigationService _popupNavigationService;
        private readonly IHttpWebDavService _httpWebDavService;
        private readonly IDialogService _dialogService;
        private readonly IDbStorageProvider _dbStorageProvider;
        public static string AppName = "NeuToDo";

        // private bool _isConnected;

        public SyncViewModel(IAccountStorageService accountStorageService,
            IPopupNavigationService popupNavigationService,
            IHttpWebDavService httpWebDavService,
            IDialogService dialogService,
            IDbStorageProvider dbStorageProvider)
        {
            _accountStorageService = accountStorageService;
            _popupNavigationService = popupNavigationService;
            _httpWebDavService = httpWebDavService;
            _dialogService = dialogService;
            _dbStorageProvider = dbStorageProvider;
        }

        private RelayCommand _pageAppearingCommand;

        public RelayCommand PageAppearingCommand => _pageAppearingCommand ??=
            new RelayCommand(async () => await PageAppearingCommandFunction());

        private async Task PageAppearingCommandFunction()
        {
            IsConnecting = true;
            ShowedAccount = await _accountStorageService.GetAccountAsync(ServerType.WebDav);
            if (ShowedAccount == null)
            {
                ShowedAccount = AccountStorageService.DefaultAccount;
                IsConnecting = false;
                ConnectionResponse.IsConnected = false;
                ConnectionResponse.Reason = "未登录WebDAV";
                return;
            }

            _httpWebDavService.Initiate(ShowedAccount);
            var res = await _httpWebDavService.TestConnection();
            ConnectionResponse.IsConnected = res;
            ConnectionResponse.Reason = res ? "已成功连接服务器" : "连接服务器失败,请检查网络或登录账户";
            IsConnecting = false;
        }


        private Account _showedAccount;

        public Account ShowedAccount
        {
            get => _showedAccount;
            set => Set(nameof(ShowedAccount), ref _showedAccount, value);
        }

        private Account _loginAccount;

        public Account LoginAccount
        {
            get => _loginAccount;
            set => Set(nameof(LoginAccount), ref _loginAccount, value);
        }

        private bool _isConnecting;

        public bool IsConnecting
        {
            get => _isConnecting;
            set => Set(nameof(IsConnecting), ref _isConnecting, value);
        }

        private ConnectionResponse _connectionResponse;

        public ConnectionResponse ConnectionResponse
        {
            get => _connectionResponse ??= new ConnectionResponse();
            set => Set(nameof(ConnectionResponse), ref _connectionResponse, value);
        }

        private RelayCommand _navigateToSyncLoginPage;

        public RelayCommand NavigateToSyncLoginPage =>
            _navigateToSyncLoginPage ??= new RelayCommand(async () => await NavigateToSyncLoginPageFunction());

        private async Task NavigateToSyncLoginPageFunction()
        {
            LoginAccount = string.IsNullOrEmpty(ShowedAccount.Password) ? new Account() : ShowedAccount;
            await _popupNavigationService.PushAsync(PopupPageNavigationConstants.SyncLoginPage);
        }

        private RelayCommand _fillAccount;

        public RelayCommand FillAccount =>
            _fillAccount ??= new RelayCommand(async () => await FillAccountFunction());

        private async Task FillAccountFunction()
        {
            IsConnecting = true;
            //TODO Validation
            _httpWebDavService.Initiate(LoginAccount);
            var res = await _httpWebDavService.TestConnection();
            IsConnecting = false;
            ConnectionResponse.IsConnected = res;
            ConnectionResponse.Reason = res ? "已成功连接服务器" : "连接服务器失败,请检查网络或登录账户";
            if (res)
            {
                ShowedAccount = LoginAccount;
                await _accountStorageService.SaveAccountAsync(ServerType.WebDav, LoginAccount);
            }

            await _popupNavigationService.PopAllAsync();
        }

        private RelayCommand _retryLogin;

        public RelayCommand RetryLogin =>
            _retryLogin ??= new RelayCommand(async () => await RetryLoginFunction());

        private async Task RetryLoginFunction()
        {
            IsConnecting = true;
            if (_httpWebDavService.IsInitialized)
            {
                var res = await _httpWebDavService.TestConnection();
                if (res == ConnectionResponse.IsConnected) //与上次结果相同，不做改变
                {
                    IsConnecting = false;
                    return;
                }

                ConnectionResponse.IsConnected = true; //记录本次结果
                ConnectionResponse.Reason = res ? "已成功连接服务器" : "连接服务器失败,请检查网络或登录账户";
                ShowedAccount = LoginAccount;
                if (res) //不成功=>成功
                    await _accountStorageService.SaveAccountAsync(ServerType.WebDav, LoginAccount);
            }
            else
            {
                _dialogService.DisplayAlert("提示", "请先点击上方登录", "OK");
            }

            IsConnecting = false;
        }

        private RelayCommand _exportToWebDav;

        public RelayCommand ExportToWebDav =>
            _exportToWebDav ??= new RelayCommand(async () => await ExportToWebDavFunction());

        private async Task ExportToWebDavFunction()
        {
            if (_httpWebDavService.IsInitialized && ConnectionResponse.IsConnected)
            {
                try
                {
                    await _dbStorageProvider.CloseConnectionAsync();

                    var destPath =
                        $"{AppName}/{DeviceInfo.Name}_{DateTime.Now:yyyy_MM_dd_HH_mm}_{DbStorageProvider.DbName}";
                    await _httpWebDavService.CreateFolder($"{AppName}");
                    await _httpWebDavService.UploadFile(destPath, DbStorageProvider.DbPath);
                    _dialogService.DisplayAlert("提示", $"已保存文件至{destPath}", "OK");
                }
                catch (Exception e)
                {
                    _dialogService.DisplayAlert("警告", e.ToString(), "OK");
                }
            }
            else
            {
                _dialogService.DisplayAlert("警告", "连接失败", "OK");
            }
        }

        private RelayCommand _exportToLocal;

        public RelayCommand ExportToLocal =>
            _exportToLocal ??= new RelayCommand(async () => await ExportToLocalFunction());

        private async Task ExportToLocalFunction()
        {
            //TODO check permission
            try
            {
                var accessHelper = DependencyService.Get<IFileAccessHelper>();
                var destDir = accessHelper.GetBackUpDirectory();
                var fileName = $"{DeviceInfo.Name}_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}_{DbStorageProvider.DbName}";
                var destPath = Path.Combine(destDir, fileName);
                File.Copy(DbStorageProvider.DbPath, destPath);
                _dialogService.DisplayAlert("提示", $"已保存至{destPath}", "Ok");
            }
            catch (Exception e)
            {
                _dialogService.DisplayAlert("警告", e.ToString(), "Ok");
            }
        }
    }

    public class ConnectionResponse : ObservableObject
    {
        private bool _isConnected;

        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                _isConnected = value;
                PictureSource = IsConnected ? "success.png" : "failure.png";
            }
        }

        private string _reason;

        public string Reason
        {
            get => _reason;
            set => Set(nameof(Reason), ref _reason, value);
        }

        private string _pictureSource;

        public string PictureSource
        {
            get => _pictureSource;
            set => Set(nameof(PictureSource), ref _pictureSource, value);
        }
    }
}