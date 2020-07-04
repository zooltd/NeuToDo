using GalaSoft.MvvmLight.Ioc;
using NeuToDo.Services;

namespace NeuToDo.ViewModels
{
    public class ViewModelLocator
    {
        public ToDoViewModel ToDoViewModel => SimpleIoc.Default.GetInstance<ToDoViewModel>();

        public SettingsViewModel SettingsViewModel => SimpleIoc.Default.GetInstance<SettingsViewModel>();

        public LoginViewModel LoginViewModel => SimpleIoc.Default.GetInstance<LoginViewModel>();

        public EventDetailViewModel EventDetailViewModel => SimpleIoc.Default.GetInstance<EventDetailViewModel>();

        public ViewModelLocator()
        {
            SimpleIoc.Default.Register<ToDoViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<EventDetailViewModel>();
            SimpleIoc.Default.Register<IAcademicCalendar, AcademicCalendar>();
            SimpleIoc.Default.Register<IPopupNavigationService, PopupNavigationService>();
            SimpleIoc.Default.Register<IPopupActivationService, PopupActivationService>();
            SimpleIoc.Default.Register<IEventDetailNavigationService, EventDetailNavigationService>();
            SimpleIoc.Default.Register<IEventDetailPageActivationService, EventDetailPageActivationService>();
            SimpleIoc.Default.Register<IStorageProvider, StorageProvider>();
            SimpleIoc.Default.Register<ILoginAndFetchDataService, LoginAndFetchDataService>();
            SimpleIoc.Default.Register<ISecureStorageProvider, SecureStorageProvider>();
            SimpleIoc.Default.Register<IPreferenceStorageProvider, PreferenceStorageProvider>();
            SimpleIoc.Default.Register<IHttpClientFactory, HttpClientFactory>();
            SimpleIoc.Default.Register<IAlertService, AlertService>();
        }
    }
}