using System.Collections.ObjectModel;
using System.Threading.Tasks;
using NeuToDo.Models;
using NeuToDo.Views;
using Xamarin.Forms;

namespace NeuToDo.Services
{
    public class EventDetailNavigationService : IEventDetailNavigationService
    {
        private readonly IEventDetailPageActivationService _eventDetailPageActivationService;

        public EventDetailNavigationService(IEventDetailPageActivationService eventDetailPageActivationService)
        {
            _eventDetailPageActivationService = eventDetailPageActivationService;
        }

        private MainPage _mainPage;

        public MainPage MainPage => _mainPage ??= Application.Current.MainPage as MainPage;

        public async Task PushAsync(EventModel e)
        {
            var page = _eventDetailPageActivationService.Activate(e.GetType().Name);
            NavigationContext.SetParameter(page, e);
            await MainPage.CurrentPage.Navigation.PushAsync(page);
        }
    }
}