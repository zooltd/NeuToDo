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

            var eventModelStorageProviderMock = new Mock<IDbStorageProvider>();
            var accountStorageService = new Mock<IAccountStorageService>();
            var alertServiceMock = new Mock<IDialogService>();
            var backupServiceMock = new Mock<IBackupService>();

            var mockPopupNavigation = popupNavigationMock.Object;
            var mockEventModelStorageProvider = eventModelStorageProviderMock.Object;
            var mockAccountStorageService = accountStorageService.Object;
            var mockAlertService = alertServiceMock.Object;
            var mockBackupService = backupServiceMock.Object;

            var item = new ServerPlatform();
            var settingsViewModel = new SettingsViewModel(mockPopupNavigation, mockAccountStorageService,
                mockEventModelStorageProvider, mockAlertService, mockBackupService);
            settingsViewModel.Command1Function(item);
            popupNavigationMock.Verify(p => p.PushAsync(PopupPageNavigationConstants.LoginPopupPage, item), Times.Once);
        }
    }
}