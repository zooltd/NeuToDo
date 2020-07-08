using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace NeuToDo.Services
{
    public class ContentPageActivationService : IContentPageActivationService
    {
        /// <summary>
        /// 页面缓存。
        /// </summary>
        private readonly Dictionary<string, ContentPage> _cache = new Dictionary<string, ContentPage>();

        public ContentPage Activate(string typename) =>
            _cache.ContainsKey(typename)
                ? _cache[typename]
                : _cache[typename] =
                    (ContentPage)Activator.CreateInstance(ContentNavigationConstants.PageKeyTypeDictionary[typename]);
    }
}