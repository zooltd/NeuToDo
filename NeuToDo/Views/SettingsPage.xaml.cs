using System;
using System.Threading.Tasks;
using NeuToDo.Views.Popup;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NeuToDo.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// TODO 删了我
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button1_OnClicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new LoginPopupPage(), true);
        }
    }
}