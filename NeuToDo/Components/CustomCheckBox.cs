using System.Windows.Input;
using NeuToDo.Models;
using Xamarin.Forms;

namespace NeuToDo.Components
{
    public class CustomCheckBox : CheckBox
    {
        public static readonly BindableProperty CheckCommandProperty =
            BindableProperty.Create(nameof(CheckCommand), typeof(ICommand), typeof(CustomCheckBox), null);

        public ICommand CheckCommand
        {
            get => (ICommand) GetValue(CheckCommandProperty);
            set => SetValue(CheckCommandProperty, value);
        }

        public CustomCheckBox()
        {
            CheckedChanged += CustomCheckBox_CheckedChanged;
        }

        private void CustomCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (BindingContext is EventModel eventModel)
            {
                CheckCommand?.Execute(eventModel);
            }
        }
    }
}