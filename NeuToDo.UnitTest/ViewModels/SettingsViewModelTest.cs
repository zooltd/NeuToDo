using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NeuToDo.Models.SettingsModels;
using NeuToDo.Services;
using NeuToDo.ViewModels;
using NUnit.Framework;

namespace NeuToDo.UnitTest.ViewModels
{
    public class SettingsViewModelTest
    {
        [Test]
        public void TestCommand1()
        {
            var popupNavigationMock = new Mock<IPopupNavigationService>();
            var secureStorageProviderMock = new Mock<ISecureStorageProvider>();
            var eventModelStorageProvider = new Mock<IEventModelStorageProvider>();
            var mockPopupNavigation = popupNavigationMock.Object;
            var mockSecureStorageProvider = secureStorageProviderMock.Object;
            var mockEventModelStorageProvider = eventModelStorageProvider.Object;
            var item = new SettingItem();
            var settingsViewModel = new SettingsViewModel(mockPopupNavigation, mockSecureStorageProvider,
                mockEventModelStorageProvider);
            settingsViewModel.Command1Function(item);
            popupNavigationMock.Verify(p => p.PushAsync(PopupPageNavigationConstants.LoginPopupPage, item), Times.Once);
        }
    }
}