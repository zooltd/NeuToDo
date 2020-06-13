using NeuToDo.Views;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NeuToDo.Services
{
    public class ContentNavigationService : IContentNavigationService
    {
        private MainPage _mainPage;

        public MainPage MainPage =>
            _mainPage ??= Application.Current.MainPage as MainPage;

        private readonly IPageActivationService _pageActivationService;

        public ContentNavigationService(IPageActivationService pageActivationService)
        {
            _pageActivationService = pageActivationService;
        }

        public async Task PushAsync(string pageKey)
        {
            await MainPage.Navigation.PushAsync(_pageActivationService.ActivateContentPage(pageKey));
        }

        public async Task PushAsync(string pageKey, object parameter)
        {
            var page = _pageActivationService.ActivateContentPage(pageKey);
            NavigationContext.SetParameter(page, parameter);
            await MainPage.Navigation.PushAsync(page);
        }
    }
}