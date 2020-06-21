using System;
using NeuToDo.Views;
using Xamarin.Forms;

namespace NeuToDo
{
    public partial class App : Application
    {
        // public static IServiceProvider ServiceProvider { get; set; }

        public App()
        {
            InitializeComponent();
            // Startup.Init();
            Device.SetFlags(new[] { "Expander_Experimental" });
            MainPage = new MainPage();
        }
    }
}