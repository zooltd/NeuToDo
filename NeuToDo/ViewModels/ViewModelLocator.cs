using GalaSoft.MvvmLight.Ioc;

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
        }
    }
}