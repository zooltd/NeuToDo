using System.Collections.ObjectModel;
using System.Threading.Tasks;
using NeuToDo.Views;
using Xamarin.Forms;

namespace NeuToDo.Services
{
    public class EventDetailNavigationService : IEventDetailNavigationService
    {
        private ContentPage _eventDetailPage;

        public ContentPage EventDetailPage => _eventDetailPage ??= new EventDetailPage();

        private MainPage _mainPage;

        public MainPage MainPage => _mainPage ??= Application.Current.MainPage as MainPage;

        public async Task PushAsync()
        {
            await MainPage.CurrentPage.Navigation.PushAsync(EventDetailPage);
        }

        public async Task PushAsync(object parameter)
        {
            NavigationContext.SetParameter(EventDetailPage, parameter);
            await MainPage.CurrentPage.Navigation.PushAsync(EventDetailPage);
        }
    }
}