using System.Threading.Tasks;
using NeuToDo.Views;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using NeuToDo.Services;

namespace NeuToDo.Services
{
    public class NavigationService : INavigationService
    {
        private MainPage _mainPage;

        public MainPage MainPage =>
            _mainPage ?? (_mainPage = Application.Current.MainPage as MainPage);

        private readonly IPageActivationService _pageActivationService;

        public NavigationService(IPageActivationService pageActivationService)
        {
            _pageActivationService = pageActivationService;
        }

        public async Task NavigateToContentPageAsync(string pageKey) =>
            await MainPage.Navigation.PushAsync(_pageActivationService.ActivateContentPage(pageKey));


        public async Task NavigateToPopupPageAsync(string pageKey) =>
            await PopupNavigation.Instance.PushAsync(_pageActivationService.ActivatePopupPage(pageKey));

        public async Task NavigateToContentPageAsync(string pageKey, object parameter)
        {
            var page = _pageActivationService.ActivateContentPage(pageKey);
            NavigationContext.SetParameter(page, parameter);
            await MainPage.Navigation.PushAsync(page);
        }

        public async Task NavigateToPopupPageAsync(string pageKey, object parameter)
        {
            var page = _pageActivationService.ActivatePopupPage(pageKey);
            NavigationContext.SetParameter(page, parameter);
            await PopupNavigation.Instance.PushAsync(page);
        }
    }
}