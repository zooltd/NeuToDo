using Moq;
using NeuToDo.Models;
using NeuToDo.Services;
using NeuToDo.ViewModels;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NeuToDo.UnitTest.ViewModels
{
    public class NeuEventDetailViewModelTest
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
            var eventDetailViewModel = new NeuEventDetailViewModel(storageProvider, mockPopupNavigationService
                , mockAlertService, mockEventDetailNavigationService, mockCampusStorageService)
            {
                SelectedEvent = new NeuEvent {Code = "A101", SemesterId = 31}
            };
            // campusStorageServiceMock.Setup(x => x.GetOrSelectCampus()).ReturnsAsync(Campus.浑南);
            var neuStorage = storageProvider.GetEventModelStorage<NeuEvent>();
            var semesterStorage = storageProvider.GetSemesterStorage();
            await neuStorage.InsertAllAsync(new List<NeuEvent>
            {
                new NeuEvent
                {
                    Id = 1, Code = "A101", Title = "A101", Detail = "A101",
                    Day = (int) DayOfWeek.Saturday, Week = 1, ClassNo = 1, SemesterId = 31
                },
                new NeuEvent
                {
                    Id = 2, Code = "A101", Title = "A101", Detail = "A101",
                    Day = (int) DayOfWeek.Saturday, Week = 2, ClassNo = 1, SemesterId = 31
                },
                new NeuEvent
                {
                    Id = 3, Code = "A101", Title = "A101", Detail = "B101",
                    Day = (int) DayOfWeek.Sunday, Week = 1, ClassNo = 1, SemesterId = 31
                },
                new NeuEvent
                {
                    Id = 4, Code = "A101", Title = "A102", Detail = "C101",
                    Day = (int) DayOfWeek.Sunday, Week = 2, ClassNo = 1, SemesterId = 31
                },
                new NeuEvent
                {
                    Id = 5, Code = "A101", Title = "A102", Detail = "C101",
                    Day = (int) DayOfWeek.Sunday, Week = 3, ClassNo = 1, SemesterId = 31
                },
                new NeuEvent
                {
                    Id = 6, Code = "B101", Title = "A102", Detail = "A102", SemesterId = 31
                }
            });
            await semesterStorage.InsertOrReplaceAsync(new Semester
                {SemesterId = 31, SchoolYear = "2019-2020", Season = "春季", BaseDate = new DateTime(2020, 2, 16)});
            await eventDetailViewModel.PageAppearingCommandFunction();
            Assert.AreEqual(3, eventDetailViewModel.NeuEventDetail.EventPeriods.Count);

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
            var eventDetailViewModel = new NeuEventDetailViewModel(storageProvider, mockPopupNavigationService
                    , mockAlertService, mockEventDetailNavigationService, mockCampusStorageService)
                {SelectedEvent = new NeuEvent {Code = "A101"}};
            eventDetailViewModel.NeuEventDetail = new NeuEventWrapper(eventDetailViewModel.SelectedEvent);
            var neuStorage = storageProvider.GetEventModelStorage<NeuEvent>();
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
            Assert.AreEqual(0, dbData.Count(x => x.IsDeleted));
            alertServiceMock.Setup(x => x.DisplayAlert("警告", "确定删除有关本课程的所有时间段？", "Yes", "No"))
                .ReturnsAsync(true);
            await eventDetailViewModel.DeleteAllFunction();
            dbData = await neuStorage.GetAllAsync();
            Assert.AreEqual(3, dbData.Count(x => x.IsDeleted));

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
            var eventDetailViewModel = new NeuEventDetailViewModel(storageProvider, mockPopupNavigationService,
                mockAlertService, mockEventDetailNavigationService, mockCampusStorageService);
            campusStorageServiceMock.Setup(x => x.GetOrSelectCampus()).ReturnsAsync(Campus.浑南);

            eventDetailViewModel.SelectedEvent = new NeuEvent {Code = "A101", Title = ""};
            eventDetailViewModel.NeuEventDetail = new NeuEventWrapper(eventDetailViewModel.SelectedEvent)
            {
                EventPeriods = new ObservableCollection<NeuEventPeriod>
                {
                    new NeuEventPeriod
                    {
                        ClassIndex = 5, Day = DayOfWeek.Sunday, Detail = "5-8, someone, 信息A101(浑南校区)",
                        WeekNo = new List<int> {2, 3, 4}
                    },
                    new NeuEventPeriod
                    {
                        ClassIndex = 5, Day = DayOfWeek.Saturday, Detail = "5-8, someone, 信息A101(浑南校区)",
                        WeekNo = new List<int> {1, 2, 3, 7, 8}
                    }
                },
                EventSemester = new Semester
                    {SemesterId = 31, SchoolYear = "2019-2020", Season = "春季", BaseDate = new DateTime(2020, 2, 16)}
            };

            await eventDetailViewModel.EditDoneFunction();
            var neuStorage = storageProvider.GetEventModelStorage<NeuEvent>();
            var dbData = await neuStorage.GetAllAsync();
            Assert.AreEqual(dbData.Count, 0);

            eventDetailViewModel.NeuEventDetail.Title = "A101";
            eventDetailViewModel.NeuEventDetail.EventPeriods.Add(new NeuEventPeriod
            {
                ClassIndex = 0,
                Day = DayOfWeek.Monday,
                Detail = null,
                WeekNo = new List<int>()
            });
            await eventDetailViewModel.EditDoneFunction();
            dbData = await neuStorage.GetAllAsync();
            Assert.AreEqual(dbData.Count, 0);

            eventDetailViewModel.NeuEventDetail.EventPeriods[2].WeekNo.AddRange(new[] {1, 2, 3});
            await eventDetailViewModel.EditDoneFunction();
            dbData = await neuStorage.GetAllAsync();
            Assert.AreEqual(dbData.Count, 0);

            eventDetailViewModel.NeuEventDetail.EventPeriods[2].ClassIndex = 1;
            await eventDetailViewModel.EditDoneFunction();
            dbData = await neuStorage.GetAllAsync();
            Assert.AreEqual(dbData.Count, 11);

            await storageProvider.CloseConnectionAsync();
        }

        [Test]
        public void PeriodCrud()
        {
            var dbStorageProviderMock = new Mock<DbStorageProvider>();
            var mockDbStorageProvider = dbStorageProviderMock.Object;
            var popupNavigationServiceMock = new Mock<IPopupNavigationService>();
            var mockPopupNavigationService = popupNavigationServiceMock.Object;
            var alertServiceMock = new Mock<IDialogService>();
            var mockAlertService = alertServiceMock.Object;
            var eventDetailNavigationServiceMock = new Mock<IContentPageNavigationService>();
            var mockEventDetailNavigationService = eventDetailNavigationServiceMock.Object;
            var campusStorageServiceMock = new Mock<ICampusStorageService>();
            var mockCampusStorageService = campusStorageServiceMock.Object;
            var eventDetailViewModel = new NeuEventDetailViewModel(mockDbStorageProvider, mockPopupNavigationService,
                mockAlertService, mockEventDetailNavigationService, mockCampusStorageService)
            {
                NeuEventDetail = new NeuEventWrapper(new NeuEvent())
            };
            Assert.AreEqual(0, eventDetailViewModel.NeuEventDetail.EventPeriods.Count);

            eventDetailViewModel.AddPeriodFunction();
            Assert.AreEqual(1, eventDetailViewModel.NeuEventDetail.EventPeriods.Count);

            eventDetailViewModel.RemovePeriodFunction(eventDetailViewModel.NeuEventDetail.EventPeriods[0]);
            Assert.AreEqual(0, eventDetailViewModel.NeuEventDetail.EventPeriods.Count);
        }
    }
}