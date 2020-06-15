using System.Collections.ObjectModel;
using System.Threading.Tasks;
using NeuToDo.Views;
using Xamarin.Forms;

namespace NeuToDo.Services
{
    public class EventDetailNavigationService : IEventDetailNavigationService
    {
        private ObservableCollection<Element> _tabbedPages;

        public ObservableCollection<Element> TabbedPages =>
            _tabbedPages ??= Application.Current.MainPage.InternalChildren;

        private ContentPage _eventDetailPage;

        public ContentPage EventDetailPage => _eventDetailPage ??= new EventDetailPage();

        public async Task PushAsync(string sourceKey)
        {
            if (TabbedPages[TabbedPageConstants.PageIndexDictionary[sourceKey]] is NavigationPage page)
                await page.Navigation.PushAsync(EventDetailPage);
        }

        public async Task PushAsync(string sourceKey, object parameter)
        {
            if (TabbedPages[TabbedPageConstants.PageIndexDictionary[sourceKey]] is NavigationPage page)
            {
                NavigationContext.SetParameter(EventDetailPage, parameter);
                await page.Navigation.PushAsync(EventDetailPage);
            }
        }
    }
}