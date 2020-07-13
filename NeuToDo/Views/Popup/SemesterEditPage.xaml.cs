using System;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace NeuToDo.Views.Popup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SemesterEditPage : PopupPage
    {
        public SemesterEditPage()
        {
            InitializeComponent();
            DateLabel.Text = DateTime.Today.ToString("D");
        }
    }
}