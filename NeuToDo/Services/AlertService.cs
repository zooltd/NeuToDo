using System.Threading.Tasks;
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

        public Task<bool> DisplayAlert(string title, string content, string accept, string cancel)
        {
            var res = new TaskCompletionSource<bool>();

            Device.BeginInvokeOnMainThread(async () =>
            {
                res.TrySetResult(await MainPage.DisplayAlert(title, content, accept, cancel));
            });
            return res.Task;
        }
    }
}