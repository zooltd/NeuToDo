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

        public SyncViewModel(IAccountStorageService accountStorageService,
            IPopupNavigationService popupNavigationService)
        {
            _accountStorageService = accountStorageService;
            _popupNavigationService = popupNavigationService;
        }

        private RelayCommand _pageAppearingCommand;

        public RelayCommand PageAppearingCommand => _pageAppearingCommand ??=
            new RelayCommand(async () => await PageAppearingCommandFunction());

        public async Task PageAppearingCommandFunction()
        {
            Account = await _accountStorageService.GetAccountAsync(ServerType.WebDav);
        }


        private Account _account;

        public Account Account
        {
            get => _account;
            set => Set(nameof(Account), ref _account, value);
        }

        private RelayCommand _navigateToSyncLoginPage;

        public RelayCommand NavigateToSyncLoginPage =>
            _navigateToSyncLoginPage ??= new RelayCommand(async () => await NavigateToSyncLoginPageFunction());

        private async Task NavigateToSyncLoginPageFunction()
        {
            await _popupNavigationService.PushAsync(PopupPageNavigationConstants.SyncLoginPage);
        }
    }
}