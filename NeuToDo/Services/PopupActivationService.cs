using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace NeuToDo.Services
{
    public class PopupActivationService : IPopupActivationService
    {
        private readonly Dictionary<string, PopupPage> _popupPageCache = new Dictionary<string, PopupPage>();

        public PopupPage ActivatePopupPage(string pageKey) => _popupPageCache.ContainsKey(pageKey)
            ? _popupPageCache[pageKey]
            : _popupPageCache[pageKey] =
                (PopupPage) Activator.CreateInstance(PopupPageNavigationConstants.PageKeyTypeDictionary[pageKey]);
    }
}