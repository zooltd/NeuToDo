using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NeuToDo.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ToDoCalendarPage : ContentPage
    {
        public ToDoCalendarPage()
        {
            InitializeComponent();
        }

        private async void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            var image = sender as Image;
            var expander = image.Parent.Parent as Expander;
            if (expander.IsExpanded)
                await image.RotateTo(0, 300, Easing.Linear);
            else
                await image.RotateTo(45, 300, Easing.Linear);
        }
    }
}