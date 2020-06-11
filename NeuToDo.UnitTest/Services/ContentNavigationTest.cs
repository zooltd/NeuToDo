using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NeuToDo.Services;
using NeuToDo.Views.Popup;
using NUnit.Framework;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace NeuToDo.UnitTest.Services {
    /// <summary>
    /// 内容导航测试。
    /// </summary>
    [TestFixture]
    public class ContentNavigationTest {
        [Test]
        public async Task TestPushAsync() {
            var pageActivationMock = new Mock<IPageActivationService>();
            var mockPageActivation = pageActivationMock.Object;
            var testKey = PopupPageNavigationConstants.LoginPopupPage;
            pageActivationMock.Setup(p => p.ActivateContentPage(testKey))
                .Returns(
                    (ContentPage) Activator.CreateInstance(typeof(PopupPage)));
            // var contentNavigation =
            //     new ContentNavigationService(mockPageActivation);
            // await contentNavigation.PushAsync(PopupPageNavigationConstants
            //     .LoginPopupPage);
            //
            // pageActivationMock.Verify(
            //     p => p.ActivateContentPage(PopupPageNavigationConstants
            //         .LoginPopupPage), Times.Once);
        }
    }
}