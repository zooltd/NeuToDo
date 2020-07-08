using Rg.Plugins.Popup.Pages;
using System.Threading.Tasks;
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

            if (!IsAnimationEnabled)
            {
                // CloseImage.Rotation = 0;
                // CloseImage.Scale = 1;
                // CloseImage.Opacity = 1;

                SaveButton.Scale = 1;
                SaveButton.Opacity = 1;

                CollectionView.TranslationX = 0;
                CollectionView.Opacity = 1;

                return;
            }

            // CloseImage.Rotation = 30;
            // CloseImage.Scale = 0.3;
            // CloseImage.Opacity = 0;

            SaveButton.Scale = 0.3;
            SaveButton.Opacity = 0;

            CollectionView.TranslationX = -10;
            CollectionView.Opacity = 0;
        }

        protected override async Task OnAppearingAnimationEndAsync()
        {
            if (!IsAnimationEnabled)
                return;

            var translateLength = 400u;

            await Task.WhenAll(
                CollectionView.TranslateTo(0, 0, easing: Easing.SpringOut, length: translateLength),
                CollectionView.FadeTo(1));

            await Task.WhenAll(
                // CloseImage.FadeTo(1),
                // CloseImage.ScaleTo(1, easing: Easing.SpringOut),
                // CloseImage.RotateTo(0),
                SaveButton.ScaleTo(1),
                SaveButton.FadeTo(1));
        }

        protected override async Task OnDisappearingAnimationBeginAsync()
        {
            if (!IsAnimationEnabled)
                return;

            var taskSource = new TaskCompletionSource<bool>();

            var currentHeight = FrameContainer.Height;

            await Task.WhenAll(
                CollectionView.FadeTo(0),
                SaveButton.FadeTo(0));

            // FrameContainer.Animate("HideAnimation", d => { FrameContainer.HeightRequest = d; },
            //     start: currentHeight,
            //     end: 170,
            //     finished: async (d, b) =>
            //     {
            //         await Task.Delay(300);
            //         taskSource.TrySetResult(true);
            //     });

            // await taskSource.Task;
        }
    }
}