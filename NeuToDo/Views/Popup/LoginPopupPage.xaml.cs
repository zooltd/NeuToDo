using System;
using System.Threading.Tasks;
using NeuToDo.Services;
using NeuToDo.ViewModels;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;
using Xamarin.Plugin.Calendar.Models;

namespace NeuToDo.Views.Popup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPopupPage : PopupPage
    {
        public LoginPopupPage()
        {
            InitializeComponent();
            var task = TryGet();
            task.Wait();
        }

        private async Task TryGet()
        {
            try
            {
                var userName = await SecureStorage.GetAsync("NeuId");
                var password = await SecureStorage.GetAsync("NeuPd");
                UsernameEntry.Text = userName;
                PasswordEntry.Text = password;
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        private async void OnLogin(object sender, EventArgs e)
        {
            var loadingPage = new LoadingPopupPage();
            await PopupNavigation.Instance.PushAsync(loadingPage);
            var userName = UsernameEntry.Text;
            var password = PasswordEntry.Text;
            await SecureStorage.SetAsync("NeuId", userName);
            await SecureStorage.SetAsync("NeuPd", password);

            await new ViewModelLocator().ToDoCalendarViewModel.UpdateEvents();

            await PopupNavigation.Instance.RemovePageAsync(loadingPage);
            await PopupNavigation.Instance.RemovePageAsync(this);
            // await PopupNavigation.Instance.PushAsync(new LoginSuccessPopupPage(), true);
        }
    }
}