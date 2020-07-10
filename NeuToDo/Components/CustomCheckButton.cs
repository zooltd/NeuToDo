using Xamarin.Forms;

namespace NeuToDo.Components
{
    public class CustomCheckButton : Button
    {
        public static readonly BindableProperty IsClickedProperty =
            BindableProperty.CreateAttached("IsClicked", typeof(bool), typeof(CustomCheckButton), false);

        public bool IsClicked
        {
            get => (bool)GetValue(IsClickedProperty);
            set
            {
                SetValue(IsClickedProperty, value);
                BackgroundColor = IsClicked ? SelectColor : Color.Transparent;
                TextColor = IsClicked ? Color.White : Color.Black;
            }
        }

        public static readonly BindableProperty SelectColorProperty =
            BindableProperty.CreateAttached("SelectColor", typeof(Color), typeof(CustomCheckButton), Color.Transparent);

        public Color SelectColor
        {
            get => (Color)GetValue(SelectColorProperty);
            set => SetValue(SelectColorProperty, value);
        }

        public CustomCheckButton() : base()
        {
            BackgroundColor = Color.Transparent;
            Clicked += (sender, args) =>
            {
                if (sender is CustomCheckButton button)
                {
                    button.IsClicked = !button.IsClicked;
                }
            };
        }
    }
}