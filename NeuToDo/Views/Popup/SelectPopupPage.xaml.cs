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

        protected override void OnAppearingAnimationBegin()
        {
            base.OnAppearingAnimationBegin();

            FrameContainer.HeightRequest = 400;
            FrameContainer.WidthRequest = 400;

            if (IsAnimationEnabled) return;

            SaveButton.Scale = 1;
            SaveButton.Opacity = 1;

            CollectionView.TranslationX = 0;
            CollectionView.Opacity = 1;
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