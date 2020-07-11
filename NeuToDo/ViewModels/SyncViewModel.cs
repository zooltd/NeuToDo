using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models;
using NeuToDo.Services;

namespace NeuToDo.ViewModels
{
    public class SyncViewModel : ViewModelBase
    {
        private readonly IAccountStorageService _accountStorageService;
        private readonly IPopupNavigationService _popupNavigationService;
        private readonly IHttpWebDavService _httpWebDavService;

        public SyncViewModel(IAccountStorageService accountStorageService,
            IPopupNavigationService popupNavigationService,
            IHttpWebDavService httpWebDavService)
        {
            _accountStorageService = accountStorageService;
            _popupNavigationService = popupNavigationService;
            _httpWebDavService = httpWebDavService;
        }

        private RelayCommand _pageAppearingCommand;

        public RelayCommand PageAppearingCommand => _pageAppearingCommand ??=
            new RelayCommand(async () => await PageAppearingCommandFunction());

        public async Task PageAppearingCommandFunction()
        {
            IsConnecting = true;
            ShowedAccount = await _accountStorageService.GetAccountAsync(ServerType.WebDav);
            if (ShowedAccount == null)
            {
                ShowedAccount = AccountStorageService.DefaultAccount;
                IsConnecting = false;
                PictureSource = "failure.png";
                Description = "未登录WebDAV";
                return;
            }

            _httpWebDavService.Initiate(ShowedAccount);
            var res = await _httpWebDavService.TestConnection();

            IsConnecting = false;
            if (res)
            {
                PictureSource = "success.png";
                Description = "已成功连接服务器";
            }
            else
            {
                PictureSource = "failure.png";
                Description = "连接服务器失败,请检查网络或登录账户";
            }
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

        private string _pictureSource;

        public string PictureSource
        {
            get => _pictureSource;
            set => Set(nameof(PictureSource), ref _pictureSource, value);
        }

        private string _description;

        public string Description
        {
            get => _description;
            set => Set(nameof(Description), ref _description, value);
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
            if (res)
            {
                ShowedAccount = new Account
                {
                    BaseUri = LoginAccount.BaseUri, UserName = LoginAccount.UserName, Password = LoginAccount.Password,
                    Remarks = LoginAccount.Remarks
                };
                PictureSource = "success.png";
                Description = "已成功连接服务器";
                await _accountStorageService.SaveAccountAsync(ServerType.WebDav, LoginAccount);
            }
            else
            {
                PictureSource = "failure.png";
                Description = "连接服务器失败,请检查网络或登录账户";
            }
            await _popupNavigationService.PopAllAsync();
        }
    }
}