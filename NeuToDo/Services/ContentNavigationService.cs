using System;
using System.Collections.ObjectModel;
using System.Linq;
using NeuToDo.Views;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NeuToDo.Services
{
    public class ContentNavigationService : IContentNavigationService
    {
        private ObservableCollection<Element> _tabbedPages;

        public ObservableCollection<Element> TabbedPages =>
            _tabbedPages ??= Application.Current.MainPage.InternalChildren;

        private readonly IPageActivationService _pageActivationService;

        public ContentNavigationService(IPageActivationService pageActivationService)
        {
            _pageActivationService = pageActivationService;
        }

        public async Task PushAsync(string sourceKey, string pageKey)
        {
            if (TabbedPages[TabbedPageConstants.PageIndexDictionary[sourceKey]] is NavigationPage page)
                await page.Navigation.PushAsync(_pageActivationService.ActivateContentPage(pageKey));
        }

        public async Task PushAsync(string sourceKey, string pageKey, object parameter)
        {
            if (TabbedPages[TabbedPageConstants.PageIndexDictionary[sourceKey]] is NavigationPage page)
            {
                var newPage = _pageActivationService.ActivateContentPage(pageKey);
                NavigationContext.SetParameter(newPage, parameter);
                await page.Navigation.PushAsync(newPage);
            }
        }
    }
}