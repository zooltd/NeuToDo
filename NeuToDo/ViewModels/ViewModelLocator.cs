using GalaSoft.MvvmLight.Ioc;
using NeuToDo.Services;

namespace NeuToDo.ViewModels
{
    public class ViewModelLocator
    {
        public MainPageViewModel MainPageViewModel => SimpleIoc.Default.GetInstance<MainPageViewModel>();

        public ToDoViewModel ToDoViewModel => SimpleIoc.Default.GetInstance<ToDoViewModel>();

        public SettingsViewModel SettingsViewModel => SimpleIoc.Default.GetInstance<SettingsViewModel>();

        public LoginViewModel LoginViewModel => SimpleIoc.Default.GetInstance<LoginViewModel>();

        public EventDetailViewModel EventDetailViewModel => SimpleIoc.Default.GetInstance<EventDetailViewModel>();

        public ViewModelLocator()
        {
            SimpleIoc.Default.Register<MainPageViewModel>();
            SimpleIoc.Default.Register<ToDoViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<EventDetailViewModel>();
            SimpleIoc.Default.Register<IAccountStorageService, AccountStorageService>();
            SimpleIoc.Default.Register<IBackupService, BackupService>();
            SimpleIoc.Default.Register<ICampusStorageService, CampusStorageService>();
            SimpleIoc.Default.Register<IContentPageActivationService, ContentPageActivationService>();
            SimpleIoc.Default.Register<IContentPageNavigationService, ContentPageNavigationService>();
            SimpleIoc.Default.Register<IDialogService, DialogService>();
            SimpleIoc.Default.Register<IPopupNavigationService, PopupNavigationService>();
            SimpleIoc.Default.Register<IPopupActivationService, PopupActivationService>();
            SimpleIoc.Default.Register<IStorageProvider, StorageProvider>();
            SimpleIoc.Default.Register<ILoginAndFetchDataService, LoginAndFetchDataService>();
            SimpleIoc.Default.Register<ISecureStorageProvider, SecureStorageProvider>();
            SimpleIoc.Default.Register<IPreferenceStorageProvider, PreferenceStorageProvider>();
            SimpleIoc.Default.Register<IHttpClientFactory, HttpClientFactory>();
        }
    }
}