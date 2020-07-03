using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NeuToDo.Components;
using NeuToDo.Models;
using NeuToDo.Services;
using NeuToDo.ViewModels;
using NUnit.Framework;
using Xamarin.Forms;

namespace NeuToDo.UnitTest.ViewModels
{
    public class EventDetailViewModelTest
    {
        [SetUp, TearDown]
        public static void RemoveDatabaseFile()
        {
            File.Delete(EventModelStorageProvider.DbPath);
        }

        [Test]
        public async Task PageAppearingCommandFunctionTest()
        {
            var storageProvider = new EventModelStorageProvider();
            var popupNavigationServiceMock = new Mock<IPopupNavigationService>();
            var mockPopupNavigationService = popupNavigationServiceMock.Object;
            var academicCalendarMock = new Mock<IAcademicCalendar>();
            var mockAcademicCalendar = academicCalendarMock.Object;
            var alertServiceMock = new Mock<IAlertService>();
            var mockAlertService = alertServiceMock.Object;
            var eventDetailViewModel = new EventDetailViewModel(storageProvider, mockPopupNavigationService,
                mockAcademicCalendar, mockAlertService)
            {
                EventGroupList = new ObservableCollection<EventGroup>(), SelectedEvent = new NeuEvent {Code = "A101"}
            };
            var neuStorage = await storageProvider.GetEventModelStorage<NeuEvent>();
            await neuStorage.InsertAllAsync(new List<NeuEvent>
            {
                new NeuEvent
                {
                    Id = 1, Code = "A101", Title = "A101", Detail = "C101", IsDone = false,
                    Time = DateTime.Today
                },
                new NeuEvent
                {
                    Id = 2, Code = "A101", Title = "A101", Detail = "C101", IsDone = false,
                    Time = DateTime.Today
                },
                new NeuEvent
                {
                    Id = 3, Code = "A101", Title = "A101", Detail = "A101", IsDone = false,
                    Time = DateTime.Today.AddDays(-1)
                },
                new NeuEvent
                {
                    Id = 4, Code = "B101", Title = "A102", Detail = "A102", IsDone = false,
                    Time = DateTime.Today.AddDays(-1)
                }
            });
            await eventDetailViewModel.PageAppearingCommandFunction();
            var eventGroupList = eventDetailViewModel.EventGroupList;
            Assert.AreEqual(eventGroupList.Count, 2);
            Assert.AreEqual(eventGroupList.First(x => x.Detail.Equals("C101")).WeekNo.Count, 2);
            Assert.AreEqual(eventGroupList.First(x => x.Detail.Equals("A101")).WeekNo.Count, 1);
            await storageProvider.CloseConnectionAsync();
        }

        [Test]
        public async Task DeleteCourseFunctionTest()
        {
            var storageProvider = new EventModelStorageProvider();
            var popupNavigationServiceMock = new Mock<IPopupNavigationService>();
            var mockPopupNavigationService = popupNavigationServiceMock.Object;
            var academicCalendarMock = new Mock<IAcademicCalendar>();
            var mockAcademicCalendar = academicCalendarMock.Object;
            var alertServiceMock = new Mock<IAlertService>();
            var mockAlertService = alertServiceMock.Object;
            var eventDetailViewModel = new EventDetailViewModel(storageProvider, mockPopupNavigationService,
                mockAcademicCalendar, mockAlertService) {SelectedEvent = new NeuEvent {Code = "A101"}};
            var neuStorage = await storageProvider.GetEventModelStorage<NeuEvent>();
            await neuStorage.InsertAllAsync(new List<NeuEvent>
            {
                new NeuEvent
                {
                    Id = 1, Code = "A101", Title = "A101", Detail = "C101", IsDone = false,
                    Time = DateTime.Today
                },
                new NeuEvent
                {
                    Id = 2, Code = "A101", Title = "A101", Detail = "C101", IsDone = false,
                    Time = DateTime.Today
                },
                new NeuEvent
                {
                    Id = 3, Code = "A101", Title = "A101", Detail = "A101", IsDone = false,
                    Time = DateTime.Today.AddDays(-1)
                },
                new NeuEvent
                {
                    Id = 4, Code = "B101", Title = "A102", Detail = "A102", IsDone = false,
                    Time = DateTime.Today.AddDays(-1)
                }
            });
            var dbData = await neuStorage.GetAllAsync();
            Assert.AreEqual(dbData.Count, 4);
            alertServiceMock.Setup(x => x.DisplayAlert("警告", "确定删除有关本课程的所有时间段？", "Yes", "No"))
                .ReturnsAsync(true);
            await eventDetailViewModel.DeleteCourseFunction();
            dbData = await neuStorage.GetAllAsync();
            Assert.AreEqual(dbData.Count, 1);

            alertServiceMock.Setup(x => x.DisplayAlert("警告", "确定删除有关本课程的所有时间段？", "Yes", "No"))
                .ReturnsAsync(false);
            eventDetailViewModel.SelectedEvent.Code = "B101";
            await eventDetailViewModel.DeleteCourseFunction();
            dbData = await neuStorage.GetAllAsync();
            Assert.AreEqual(dbData.Count, 1);

            await storageProvider.CloseConnectionAsync();
        }

        [Test]
        public async Task EditDoneFunctionTest()
        {
            var storageProvider = new EventModelStorageProvider();
            var popupNavigationServiceMock = new Mock<IPopupNavigationService>();
            var mockPopupNavigationService = popupNavigationServiceMock.Object;
            var academicCalendarMock = new Mock<IAcademicCalendar>();
            var mockAcademicCalendar = academicCalendarMock.Object;
            var alertServiceMock = new Mock<IAlertService>();
            var mockAlertService = alertServiceMock.Object;
            var eventDetailViewModel = new EventDetailViewModel(storageProvider, mockPopupNavigationService,
                mockAcademicCalendar, mockAlertService);
            eventDetailViewModel.SelectedEvent = new EventModel {Code = "A101", Title = ""};
            eventDetailViewModel.EventGroupList = new ObservableCollection<EventGroup>
            {
                new EventGroup
                {
                    ClassIndex = 5, Day = DayOfWeek.Sunday, Detail = "5-8, someone, 信息A101(浑南校区)",
                    WeekNo = new List<int> {2, 3, 4}
                },
                new EventGroup
                {
                    ClassIndex = 5, Day = DayOfWeek.Saturday, Detail = "5-8, someone, 信息A101(浑南校区)",
                    WeekNo = new List<int> {1, 2, 3, 7, 8}
                }
            };
            await eventDetailViewModel.EditDoneFunction();
            var neuStorage = await storageProvider.GetEventModelStorage<NeuEvent>();
            var dbData = await neuStorage.GetAllAsync();
            Assert.AreEqual(dbData.Count, 0);

            eventDetailViewModel.SelectedEvent.Title = "A101";
            eventDetailViewModel.EventGroupList.Add(new EventGroup
            {
                ClassIndex = 0, Day = DayOfWeek.Monday, Detail = null,
                WeekNo = new List<int>()
            });
            await eventDetailViewModel.EditDoneFunction();
            dbData = await neuStorage.GetAllAsync();
            Assert.AreEqual(dbData.Count, 0);

            eventDetailViewModel.EventGroupList[2].WeekNo.AddRange(new[] {1, 2, 3});
            await eventDetailViewModel.EditDoneFunction();
            dbData = await neuStorage.GetAllAsync();
            Assert.AreEqual(dbData.Count, 0);

            eventDetailViewModel.EventGroupList[2].ClassIndex = 1;
            await eventDetailViewModel.EditDoneFunction();
            dbData = await neuStorage.GetAllAsync();
            Assert.AreEqual(dbData.Count, 11);
            await storageProvider.CloseConnectionAsync();
        }

        [Test]
        public void SelectWeekNoDoneFunctionTest()
        {
            var collection = new CollectionView();
            for (var i = 0; i < 3; i++)
            {
                collection.AddLogicalChild(new CustomButton {IsClicked = true});
            }

            for (var i = 0; i < 3; i++)
            {
                collection.AddLogicalChild(new CustomButton {IsClicked = false});
            }

            var storageProviderMock = new Mock<EventModelStorageProvider>();
            var mockStorageProvider = storageProviderMock.Object;
            var popupNavigationServiceMock = new Mock<IPopupNavigationService>();
            var mockPopupNavigationService = popupNavigationServiceMock.Object;
            var academicCalendarMock = new Mock<IAcademicCalendar>();
            var mockAcademicCalendar = academicCalendarMock.Object;
            var alertServiceMock = new Mock<IAlertService>();
            var mockAlertService = alertServiceMock.Object;
            var eventDetailViewModel = new EventDetailViewModel(mockStorageProvider, mockPopupNavigationService,
                    mockAcademicCalendar, mockAlertService)
                {SelectEventGroup = new EventGroup {WeekNo = new List<int> {2, 3, 4, 5}}};
            eventDetailViewModel.SelectWeekNoDoneFunction(collection);
            Assert.AreEqual(eventDetailViewModel.SelectEventGroup.WeekNo, new List<int> {1, 2, 3});
        }
    }
}