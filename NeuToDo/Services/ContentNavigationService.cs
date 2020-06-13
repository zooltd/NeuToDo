using NeuToDo.Views;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NeuToDo.Services
{
    public class ContentNavigationService : IContentNavigationService
    {
        private Page _actionPage;

        public Page ActionPage =>
            _actionPage ??= Application.Current.MainPage.InternalChildren[0] as NavigationPage;

        private readonly IPageActivationService _pageActivationService;

        public ContentNavigationService(IPageActivationService pageActivationService)
        {
            _pageActivationService = pageActivationService;
        }

        public async Task PushAsync(string pageKey)
        {
            await ActionPage.Navigation.PushAsync(_pageActivationService.ActivateContentPage(pageKey));
        }

        public async Task PushAsync(string pageKey, object parameter)
        {
            var page = _pageActivationService.ActivateContentPage(pageKey);
            NavigationContext.SetParameter(page, parameter);
            await ActionPage.Navigation.PushAsync(page);
        }
    }
}