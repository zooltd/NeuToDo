using System.Collections.Generic;
using System.Reflection;
using NeuToDo.Views;
using Xamarin.Forms;

namespace NeuToDo
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Device.SetFlags(new[] { "Expander_Experimental" });
            MainPage = new MainPage();
        }
    }
}