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
            File.Delete(DbStorageProvider.DbPath);
        }

        [Test]
        public async Task PageAppearingCommandFunctionTest()
        {
            var storageProvider = new DbStorageProvider();
            await storageProvider.CheckInitialization();
            var popupNavigationServiceMock = new Mock<IPopupNavigationService>();
            var mockPopupNavigationService = popupNavigationServiceMock.Object;
            var alertServiceMock = new Mock<IDialogService>();
            var mockAlertService = alertServiceMock.Object;
            var eventDetailNavigationServiceMock = new Mock<IContentPageNavigationService>();
            var mockEventDetailNavigationService = eventDetailNavigationServiceMock.Object;
            var campusStorageServiceMock = new Mock<ICampusStorageService>();
            var mockCampusStorageService = campusStorageServiceMock.Object;
            var eventDetailViewModel = new EventDetailViewModel(storageProvider, mockPopupNavigationService
                , mockAlertService, mockEventDetailNavigationService, mockCampusStorageService)
            {
                EventGroupList = new ObservableCollection<EventGroup>(),
                SelectedEvent = new NeuEvent {Code = "A101", SemesterId = 31}
            };
            // campusStorageServiceMock.Setup(x => x.GetCampus()).ReturnsAsync(Campus.Hunnan);
            var neuStorage =  storageProvider.GetEventModelStorage<NeuEvent>();
            var semesterStorage =  storageProvider.GetSemesterStorage();
            await neuStorage.InsertAllAsync(new List<NeuEvent>
            {
                new NeuEvent
                {
                    Id = 1, Code = "A101", Title = "A101", Detail = "A101",
                    Day = (int) DayOfWeek.Saturday, Week = 1, ClassNo = 1
                },
                new NeuEvent
                {
                    Id = 2, Code = "A101", Title = "A101", Detail = "A101",
                    Day = (int) DayOfWeek.Saturday, Week = 2, ClassNo = 1
                },
                new NeuEvent
                {
                    Id = 3, Code = "A101", Title = "A101", Detail = "B101",
                    Day = (int) DayOfWeek.Sunday, Week = 1, ClassNo = 1
                },
                new NeuEvent
                {
                    Id = 4, Code = "A101", Title = "A102", Detail = "C101",
                    Day = (int) DayOfWeek.Sunday, Week = 2, ClassNo = 1
                },
                new NeuEvent
                {
                    Id = 5, Code = "A101", Title = "A102", Detail = "C101",
                    Day = (int) DayOfWeek.Sunday, Week = 3, ClassNo = 1
                },
                new NeuEvent
                {
                    Id = 6, Code = "B101", Title = "A102", Detail = "A102",
                }
            });
            await semesterStorage.InsertOrReplaceAsync(new Semester
                {SemesterId = 31, SchoolYear = "2019-2020", Season = "春季", BaseDate = new DateTime(2020, 2, 16)});
            await eventDetailViewModel.PageAppearingCommandFunction();
            var eventGroupList = eventDetailViewModel.EventGroupList;
            Assert.AreEqual(eventGroupList.Count, 3);

            await storageProvider.CloseConnectionAsync();
        }

        [Test]
        public async Task DeleteCourseFunctionTest()
        {
            var storageProvider = new DbStorageProvider();
            await storageProvider.CheckInitialization();
            var popupNavigationServiceMock = new Mock<IPopupNavigationService>();
            var mockPopupNavigationService = popupNavigationServiceMock.Object;
            var alertServiceMock = new Mock<IDialogService>();
            var mockAlertService = alertServiceMock.Object;
            var eventDetailNavigationServiceMock = new Mock<IContentPageNavigationService>();
            var mockEventDetailNavigationService = eventDetailNavigationServiceMock.Object;
            var campusStorageServiceMock = new Mock<ICampusStorageService>();
            var mockCampusStorageService = campusStorageServiceMock.Object;
            var eventDetailViewModel = new EventDetailViewModel(storageProvider, mockPopupNavigationService
                    , mockAlertService, mockEventDetailNavigationService, mockCampusStorageService)
                {SelectedEvent = new NeuEvent {Code = "A101"}};

            var neuStorage =  storageProvider.GetEventModelStorage<NeuEvent>();
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
            var storageProvider = new DbStorageProvider();
            await storageProvider.CheckInitialization();
            var popupNavigationServiceMock = new Mock<IPopupNavigationService>();
            var mockPopupNavigationService = popupNavigationServiceMock.Object;
            var alertServiceMock = new Mock<IDialogService>();
            var mockAlertService = alertServiceMock.Object;
            var eventDetailNavigationServiceMock = new Mock<IContentPageNavigationService>();
            var mockEventDetailNavigationService = eventDetailNavigationServiceMock.Object;
            var campusStorageServiceMock = new Mock<ICampusStorageService>();
            var mockCampusStorageService = campusStorageServiceMock.Object;
            var eventDetailViewModel = new EventDetailViewModel(storageProvider, mockPopupNavigationService,
                mockAlertService, mockEventDetailNavigationService, mockCampusStorageService);
            campusStorageServiceMock.Setup(x => x.GetCampus()).ReturnsAsync(Campus.Hunnan);

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
            eventDetailViewModel.EventSemester = new Semester
                {SemesterId = 31, SchoolYear = "2019-2020", Season = "春季", BaseDate = new DateTime(2020, 2, 16)};
            await eventDetailViewModel.EditDoneFunction();
            var neuStorage =  storageProvider.GetEventModelStorage<NeuEvent>();
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
    }
}