using Moq;
using NeuToDo.Models;
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
            var contentPageNavigationServiceMock = new Mock<IContentPageNavigationService>();
            var campusStorageServiceMock = new Mock<ICampusStorageService>();
            var calendarStorageProviderMock = new Mock<ICalendarStorageProvider>();

            var mockPopupNavigation = popupNavigationMock.Object;
            var mockEventModelStorageProvider = eventModelStorageProviderMock.Object;
            var mockAccountStorageService = accountStorageService.Object;
            var mockAlertService = alertServiceMock.Object;
            var mockContentPageNavigationService = contentPageNavigationServiceMock.Object;
            var mockCampusStorageService = campusStorageServiceMock.Object;
            var mockCalendarStorageProvider = calendarStorageProviderMock.Object;

            var item = new ServerPlatform();
            var settingsViewModel = new SettingsViewModel(mockPopupNavigation, mockAccountStorageService,
                mockEventModelStorageProvider, mockAlertService, mockContentPageNavigationService,
                mockCampusStorageService, mockCalendarStorageProvider);
            settingsViewModel.Command1Function(item);
            popupNavigationMock.Verify(p => p.PushAsync(PopupPageNavigationConstants.LoginPopupPage, item), Times.Once);
        }
    }
}