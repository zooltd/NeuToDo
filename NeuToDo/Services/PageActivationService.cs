using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace NeuToDo.Services
{
    public class PageActivationService : IPageActivationService
    {
        private readonly Dictionary<string, ContentPage> _contentPageCache = new Dictionary<string, ContentPage>();
        private readonly Dictionary<string, PopupPage> _popupPageCache = new Dictionary<string, PopupPage>();

        public ContentPage ActivateContentPage(string pageKey) => _contentPageCache.ContainsKey(pageKey)
            ? _contentPageCache[pageKey]
            : _contentPageCache[pageKey] =
                (ContentPage)Activator.CreateInstance(PopupPageNavigationConstants.PageKeyTypeDictionary[pageKey]);

        public PopupPage ActivatePopupPage(string pageKey) => _popupPageCache.ContainsKey(pageKey)
            ? _popupPageCache[pageKey]
            : _popupPageCache[pageKey] =
                (PopupPage)Activator.CreateInstance(PopupPageNavigationConstants.PageKeyTypeDictionary[pageKey]);
    }
}