using GalaSoft.MvvmLight.Ioc;
using NeuToDo.Services;

namespace NeuToDo.ViewModels
{
    public class ViewModelLocator
    {
        public ToDoCalendarViewModel ToDoCalendarViewModel =>
            SimpleIoc.Default.GetInstance<ToDoCalendarViewModel>();

        public SettingsViewModel SettingsViewModel =>
            SimpleIoc.Default.GetInstance<SettingsViewModel>();

        public LoginViewModel LoginViewModel =>
            SimpleIoc.Default.GetInstance<LoginViewModel>();

        public EventDetailViewModel EventDetailViewModel =>
            SimpleIoc.Default.GetInstance<EventDetailViewModel>();

        public ViewModelLocator()
        {
            SimpleIoc.Default.Register<ToDoCalendarViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<EventDetailViewModel>();
            SimpleIoc.Default.Register<IPopupNavigationService, PopupNavigationService>();
            SimpleIoc.Default.Register<IPopupActivationService, PopupActivationService>();
            SimpleIoc.Default.Register<IEventDetailNavigationService, EventDetailNavigationService>();
            SimpleIoc.Default.Register<IEventModelStorageProvider, EventModelStorageProvider>();
            SimpleIoc.Default.Register<ILoginServiceProvider,LoginServiceProvider>();
        }
    }
}