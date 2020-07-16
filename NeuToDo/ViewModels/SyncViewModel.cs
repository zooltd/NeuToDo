using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models;
using NeuToDo.Services;
using Plugin.FilePicker;
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
        private readonly IContentPageNavigationService _contentPageNavigationService;
        private readonly IFileAccessHelper _fileAccessHelper;
        private readonly ISyncService _syncService;
        private const string AppName = "NeuToDo";

        // private bool _isConnected;

        public SyncViewModel(IAccountStorageService accountStorageService,
            IPopupNavigationService popupNavigationService,
            IHttpWebDavService httpWebDavService,
            IDialogService dialogService,
            IDbStorageProvider dbStorageProvider,
            IContentPageNavigationService contentPageNavigationService,
            ISyncService syncService)
        {
            _accountStorageService = accountStorageService;
            _popupNavigationService = popupNavigationService;
            _httpWebDavService = httpWebDavService;
            _dialogService = dialogService;
            _dbStorageProvider = dbStorageProvider;
            _contentPageNavigationService = contentPageNavigationService;
            _syncService = syncService;
            _fileAccessHelper = DependencyService.Get<IFileAccessHelper>();
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
            var res = await _httpWebDavService.TestConnection(); //TODO 超时
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

        private List<RecoveryFile> _recoveryFiles;

        public List<RecoveryFile> RecoveryFiles
        {
            get => _recoveryFiles;
            set => Set(nameof(RecoveryFiles), ref _recoveryFiles, value);
        }

        /*-------------------------------------*/
        private const double MaxOpacity = 1;

        private double _expandedPercentage;

        public double ExpandedPercentage
        {
            get => _expandedPercentage;
            set
            {
                Set(nameof(ExpandedPercentage), ref _expandedPercentage, value);
                OverlayOpacity = MaxOpacity < value ? MaxOpacity : value;
            }
        }

        private bool _isExpanded;

        public bool IsExpanded
        {
            get => _isExpanded;
            set => Set(nameof(IsExpanded), ref _isExpanded, value);
        }

        private bool _isVisible;

        public bool IsVisible
        {
            get => _isVisible;
            set => Set(nameof(IsVisible), ref _isVisible, value);
        }

        private double _overlayOpacity;

        public double OverlayOpacity
        {
            get => _overlayOpacity;
            set => Set(nameof(OverlayOpacity), ref _overlayOpacity, value);
        }

        private RelayCommand _backgroundClicked;

        public RelayCommand BackgroundClicked =>
            _backgroundClicked ??= new RelayCommand(() => { IsExpanded = false; });

        private RelayCommand _closeDrawer;

        public RelayCommand CloseDrawer =>
            _closeDrawer ??= new RelayCommand(() =>
            {
                IsExpanded = false;
                IsVisible = false;
            });

        /*-------------------------------------*/
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
                    await _httpWebDavService.UploadFileAsync(destPath, DbStorageProvider.DbPath);
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
            _exportToLocal ??= new RelayCommand(ExportToLocalFunction);

        private void ExportToLocalFunction()
        {
            //TODO check permission
            try
            {
                var destDir = _fileAccessHelper.GetBackUpDirectory();
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


        private RelayCommand _showBackUpFiles;

        public RelayCommand ShowBackUpFiles =>
            _showBackUpFiles ??= new RelayCommand(async () => await ShowBackUpFilesFunction());

        private async Task ShowBackUpFilesFunction()
        {
            IsVisible = true;
            IsExpanded = true;
            RecoveryFiles = await GetBackupFiles();
        }


        private RelayCommand _helpCommand;

        public RelayCommand HelpCommand =>
            _helpCommand ??= new RelayCommand(async () =>
            {
                await _contentPageNavigationService.PushAsync(ContentNavigationConstants.HelpPage);
            });


        private async Task<List<RecoveryFile>> GetBackupFiles()
        {
            var localDir = _fileAccessHelper.GetBackUpDirectory();
            var files = Directory.GetFiles(localDir, "*_*.sqlite3");

            var fileList = files.Select(file => new FileInfo(file)).Select(fi => new RecoveryFile
                {FileName = fi.Name, FilePath = fi.FullName, FileSource = FileSource.Local}).ToList();

            if (!ConnectionResponse.IsConnected) return fileList;
            var webFiles = await _httpWebDavService.GetFilesAsync($"{AppName}", @"(.*)_events.sqlite3");
            fileList.AddRange(webFiles);
            return fileList;
        }


        private RelayCommand<RecoveryFile> _importFile;

        public RelayCommand<RecoveryFile> ImportFile =>
            _importFile ??= new RelayCommand<RecoveryFile>(async (file) => await ImportFileFunction(file));

        private async Task ImportFileFunction(RecoveryFile recoveryFile)
        {
            var res = await _dialogService.DisplayAlert("警告", "将覆盖现有数据库文件，请确保已备份重要信息", "确认覆盖", "取消覆盖");
            if (!res) return;
            await _dbStorageProvider.CloseConnectionAsync();
            var inputStream = recoveryFile.FileSource switch
            {
                FileSource.Server => await _httpWebDavService.GetFileAsync(recoveryFile.FilePath),
                FileSource.Local => File.OpenRead(recoveryFile.FilePath),
                _ => throw new ArgumentOutOfRangeException()
            };

            var outputStream = File.Create(DbStorageProvider.DbPath);
            CopyStream(inputStream, outputStream);
            inputStream.Close();
            outputStream.Close();
            _dbStorageProvider.OnUpdateData();
            _dialogService.DisplayAlert("提示", "导入成功", "OK");
        }

        private static void CopyStream(Stream input, Stream output)
        {
            var buffer = new byte[16 * 1024];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                output.Write(buffer, 0, read);
        }


        private RelayCommand _syncCommand;

        public RelayCommand SyncCommand =>
            _syncCommand ??= new RelayCommand(async () => await SyncCommandFunction());

        private async Task SyncCommandFunction()
        {
            if (_httpWebDavService.IsInitialized && ConnectionResponse.IsConnected)
            {
                await _popupNavigationService.PushAsync(PopupPageNavigationConstants.LoadingPopupPage);
                try
                {
                    await _syncService.SyncEventModelsAsync($"{AppName}/{AppName}.zip");
                    _dbStorageProvider.OnUpdateData();
                    await _popupNavigationService.PopAllAsync();
                    _dialogService.DisplayAlert("成功", "已同步", "OK");
                }
                catch (Exception e)
                {
                    await _popupNavigationService.PopAllAsync();
                    _dialogService.DisplayAlert("错误", e.ToString(), "OK");
                }
            }
            else
            {
                _dialogService.DisplayAlert("警告", "请先登录", "OK");
            }
        }


        //
        public async Task ImportAsync()
        {
            var pickedFile = await CrossFilePicker.Current.PickFile();

            if (pickedFile == null)
                throw new Exception($"导入文件名应为\"{DbStorageProvider.DbName}\"");

            if (pickedFile.FileName != DbStorageProvider.DbName)
                throw new Exception($"导入文件名应为\"{DbStorageProvider.DbName}\"");

            await _dbStorageProvider.CloseConnectionAsync();

            var stream = pickedFile.GetStream();
            using var fileStream = File.Create(DbStorageProvider.DbPath);
            CopyStream(stream, fileStream);
        }
    }
}