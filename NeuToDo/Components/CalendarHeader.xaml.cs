using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NeuToDo.Components
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CalendarHeader : DataTemplate
    {
        public CalendarHeader()
        {
            InitializeComponent();
        }
    }
}