using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NeuToDo.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ToDoListPage : ContentPage
    {
        public ToDoListPage()
        {
            InitializeComponent();
            SizeChanged += (sender, args) => FirstRow.Height = Height * 0.85;
        }
    }
}