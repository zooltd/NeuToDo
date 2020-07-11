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
            // Account = await _accountStorageService.GetAccountAsync(ServerType.WebDav);
            // if (Account == null)
            // {
            //     IsConnecting = false;
            //     ConnectionResult = new ConnectionResult {PictureSource = "failure.png", Description = "未成功连接服务器"};
            //     return;
            // }
            // _httpWebDavService.Initiate(Account);
            // var res = await _httpWebDavService.TestConnection();
            await Task.Delay(3000);
            PictureSource = "success.png";
            Description = "已成功连接服务器";
            IsConnecting = false;
        }


        private Account _account;

        public Account Account
        {
            get => _account;
            set => Set(nameof(Account), ref _account, value);
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
            await _popupNavigationService.PushAsync(PopupPageNavigationConstants.SyncLoginPage);
        }

        private RelayCommand _onLogin;

        public RelayCommand Login =>
            _onLogin ??= new RelayCommand(async () => await OnLoginFunction());


        private async Task OnLoginFunction()
        {
            throw new System.NotImplementedException();
        }
    }

   

    
}