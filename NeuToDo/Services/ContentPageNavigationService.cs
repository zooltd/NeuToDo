using NeuToDo.Models;
using NeuToDo.Views;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NeuToDo.Services
{
    public class ContentPageNavigationService : IContentPageNavigationService
    {
        private readonly IContentPageActivationService _contentPageActivationService;

        public ContentPageNavigationService(IContentPageActivationService contentPageActivationService)
        {
            _contentPageActivationService = contentPageActivationService;
        }

        private MainPage _mainPage;

        public MainPage MainPage => _mainPage ??= Application.Current.MainPage as MainPage;

        public async Task PushAsync(EventModel e)
        {
            var page = _contentPageActivationService.Activate(e.GetType().Name);
            NavigationContext.SetParameter(page, e);
            await MainPage.CurrentPage.Navigation.PushAsync(page);
        }

        public async Task PopToRootAsync()
        {
            await MainPage.CurrentPage.Navigation.PopToRootAsync();
        }
    }
}