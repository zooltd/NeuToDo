using Microsoft.Extensions.Logging;
using Moq;
using NeuToDo.Services;
using NeuToDo.Utils;
using NeuToDo.ViewModels;

namespace NeuToDo.UnitTest.ViewModels
{
    public class EventDetailViewModelTest
    {
        public void Init()
        {
            var eventStorageMock = new Mock<IEventModelStorageProvider>();
            var mockEventStorage = eventStorageMock.Object;
            var popupNavigationServiceMock = new Mock<IPopupNavigationService>();
            var mockPopupNavigationService = popupNavigationServiceMock.Object;
            var academicCalendarMock = new Mock<IAcademicCalendar>();
            var mockAcademicCalendar = academicCalendarMock.Object;
            var alertServiceMock = new Mock<IAlertService>();
            var mockAlertService = alertServiceMock.Object;
            var eventDetailViewModel = new EventDetailViewModel(mockEventStorage, mockPopupNavigationService,
                mockAcademicCalendar, mockAlertService);
        }
    }
}