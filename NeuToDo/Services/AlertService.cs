using NeuToDo.Views;
using Xamarin.Forms;

namespace NeuToDo.Services
{
    public class AlertService : IAlertService
    {
        private MainPage _mainPage;

        public MainPage MainPage => _mainPage ??= Application.Current.MainPage as MainPage;

        public void DisplayAlert(string title, string content, string button)
        {
            Device.BeginInvokeOnMainThread(async () =>
                await MainPage.DisplayAlert(title, content, button));
        }
    }
}