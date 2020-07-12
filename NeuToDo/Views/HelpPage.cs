using Xamarin.Forms;

namespace NeuToDo.Views
{
    public class HelpPage : ContentPage
    {
        public HelpPage()
        {
            Content = new WebView {Source = "http://help.jianguoyun.com/?p=2064"};
            Title = "帮助";
        }
    }
}