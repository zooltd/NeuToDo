using NeuToDo.Models;
using System.Windows.Input;
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

        //TODO 页面初始化绑定时也会触发
        private void CustomCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            // if (sender is CustomCheckBox checkBox)
            //     checkBox.IsChecked = !checkBox.IsChecked;
            if (BindingContext is EventModel eventModel)
                CheckCommand?.Execute(eventModel);
        }
    }
}