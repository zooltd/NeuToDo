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
            var eventModelStorageProviderMock = new Mock<IStorageProvider>();
            var preferenceStorageProviderMock = new Mock<IPreferenceStorageProvider>();
            var alertServiceMock = new Mock<IAlertService>();

            var mockPopupNavigation = popupNavigationMock.Object;
            var mockSecureStorageProvider = secureStorageProviderMock.Object;
            var mockEventModelStorageProvider = eventModelStorageProviderMock.Object;
            var mockPreferenceStorageProvider = preferenceStorageProviderMock.Object;
            var mockAlertService = alertServiceMock.Object;
            var item = new Platform();
            var settingsViewModel = new SettingsViewModel(mockPopupNavigation, mockSecureStorageProvider,
                mockPreferenceStorageProvider, mockEventModelStorageProvider, mockAlertService);
            settingsViewModel.Command1Function(item);
            popupNavigationMock.Verify(p => p.PushAsync(PopupPageNavigationConstants.LoginPopupPage, item), Times.Once);
        }
    }
}