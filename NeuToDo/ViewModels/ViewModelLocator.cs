using GalaSoft.MvvmLight.Ioc;
using NeuToDo.Models.SettingsModels;

namespace NeuToDo.ViewModels
{
    public class ViewModelLocator
    {
        public ToDoCalendarViewModel ToDoCalendarViewModel =>
            SimpleIoc.Default.GetInstance<ToDoCalendarViewModel>();

        public SettingsViewModel SettingsViewModel =>
            SimpleIoc.Default.GetInstance<SettingsViewModel>();

        public ViewModelLocator()
        {
            SimpleIoc.Default.Register<ToDoCalendarViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
        }
    }
}