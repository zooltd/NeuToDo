using System.Windows.Input;
using Xamarin.Forms;

namespace NeuToDo.Components
{
    public class CustomSwitch : Switch
    {
        public static readonly BindableProperty ToggleCommandProperty =
            BindableProperty.Create(nameof(ToggleCommand), typeof(ICommand), typeof(CustomSwitch), null);

        public ICommand ToggleCommand
        {
            get => (ICommand) GetValue(ToggleCommandProperty);
            set => SetValue(ToggleCommandProperty, value);
        }

        public CustomSwitch()
        {
            Toggled += CustomSwitch_Toggled;
        }

        private void CustomSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            ToggleCommand?.Execute(sender);
        }
    }
}