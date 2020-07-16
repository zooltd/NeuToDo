using Rg.Plugins.Popup.Pages;
using System.Threading.Tasks;
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

        protected override bool OnBackgroundClicked()
        {
            CloseAllPopup();

            return false;
        }

        private async void CloseAllPopup()
        {
            await PopupNavigation.Instance.PopAllAsync();
        }
    }
}