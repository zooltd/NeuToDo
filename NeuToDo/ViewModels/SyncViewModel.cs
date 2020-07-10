using GalaSoft.MvvmLight;
using NeuToDo.Services;

namespace NeuToDo.ViewModels
{
    public class SyncViewModel : ViewModelBase
    {
        public SyncViewModel(IAccountStorageService accountStorageService)
        {
        }

        private Account _account;

        public Account Account
        {
            get => _account;
            set => Set(nameof(Account), ref _account, value);
        }
    }

    public class Account
    {
        public string ServerUri { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Remarks { get; set; }
    }
}