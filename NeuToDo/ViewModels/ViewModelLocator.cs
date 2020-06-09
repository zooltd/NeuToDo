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

        public ViewModelLocator()
        {
            SimpleIoc.Default.Register<ToDoCalendarViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<INavigationService, NavigationService>();
            SimpleIoc.Default.Register<IPageActivationService, PageActivationService>();
            SimpleIoc.Default.Register<IEventModelStorageProvider, EventModelStorageProvider>();
        }
    }
}