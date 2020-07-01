using Xamarin.Forms;

namespace NeuToDo.Components
{
    public class CustomButton : Button
    {
        public static readonly BindableProperty IsClickedProperty =
            BindableProperty.CreateAttached("IsClicked", typeof(bool), typeof(CustomButton), false);

        public bool IsClicked
        {
            get => (bool) GetValue(IsClickedProperty);
            set
            {
                SetValue(IsClickedProperty, value);
                BackgroundColor = IsClicked ? SelectColor : Color.Transparent;
            }
        }

        public static readonly BindableProperty SelectColorProperty =
            BindableProperty.CreateAttached("SelectColor", typeof(Color), typeof(CustomButton), Color.Transparent);

        public Color SelectColor
        {
            get => (Color) GetValue(SelectColorProperty);
            set => SetValue(SelectColorProperty, value);
        }

        public CustomButton() : base()
        {
            BackgroundColor = Color.Transparent;
            Clicked += (sender, args) =>
            {
                if (sender is CustomButton button)
                {
                    button.IsClicked = !button.IsClicked;
                }
            };
        }
    }
}