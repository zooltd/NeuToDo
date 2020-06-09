using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using NeuToDo.Services;
using NeuToDo.ViewModels;
using NeuToDo.Views;
using Xamarin.Forms;

namespace NeuToDo
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }
    }
}