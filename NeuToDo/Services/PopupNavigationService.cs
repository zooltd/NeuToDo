using Rg.Plugins.Popup.Services;
using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public class PopupNavigationService : IPopupNavigationService
    {
        private readonly IPopupActivationService _pageActivationService;

        public PopupNavigationService(IPopupActivationService pageActivationService)
        {
            _pageActivationService = pageActivationService;
        }

        public async Task PushAsync(string pageKey)
        {
            await PopupNavigation.Instance.PushAsync(_pageActivationService.ActivatePopupPage(pageKey));
        }

        public async Task PushAsync(string pageKey, object parameter)
        {
            var page = _pageActivationService.ActivatePopupPage(pageKey);
            NavigationContext.SetParameter(page, parameter);
            await PopupNavigation.Instance.PushAsync(page);
        }

        public async Task PopAllAsync()
        {
            await PopupNavigation.Instance.PopAllAsync();
        }
    }
}