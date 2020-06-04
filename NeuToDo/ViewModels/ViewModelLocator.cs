using GalaSoft.MvvmLight.Ioc;

namespace NeuToDo.ViewModels
{
    public class ViewModelLocator
    {
        public ToDoCalendarViewModel ToDoCalendarViewModel =>
            SimpleIoc.Default.GetInstance<ToDoCalendarViewModel>();

        public ViewModelLocator()
        {
            SimpleIoc.Default.Register<ToDoCalendarViewModel>();
        }
    }
}