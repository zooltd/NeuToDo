using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NeuToDo.Views.Popup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectPopupPage : PopupPage
    {
        public SelectPopupPage()
        {
            InitializeComponent();
        }

        private async void OnCloseButtonTapped(object sender, EventArgs e) {
            await PopupNavigation.Instance.PopAllAsync();
        }
    }
}