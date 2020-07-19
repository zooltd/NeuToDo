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

        /// <summary>
        /// 页面激活函数
        /// </summary>
        /// <param name="typeName">页面类型</param>
        /// <returns></returns>
        public ContentPage Activate(string typeName) =>
            _cache.ContainsKey(typeName)
                ? _cache[typeName]
                : _cache[typeName] =
                    (ContentPage) Activator.CreateInstance(ContentNavigationConstants.PageKeyTypeDictionary[typeName]);
    }
}