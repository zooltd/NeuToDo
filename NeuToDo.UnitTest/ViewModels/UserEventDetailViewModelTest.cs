using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NeuToDo.Models;
using NeuToDo.Services;
using NeuToDo.ViewModels;
using NUnit.Framework;

namespace NeuToDo.UnitTest.ViewModels
{
    public class UserEventDetailViewModelTest
    {
        [SetUp, TearDown]
        public static void RemoveDatabaseFile()
        {
            File.Delete(DbStorageProvider.DbPath);
        }

        [Test]
        public async Task PageAppearingCommandFunctionTest()
        {
            var dbStorageProvider = new DbStorageProvider();
            await dbStorageProvider.CheckInitialization();
            var dialogServiceMock = new Mock<IDialogService>();
            var mockDialogService = dialogServiceMock.Object;
            var contentPageNavigationServiceMock = new Mock<IContentPageNavigationService>();
            var mockContentPageNavigationServiceMock = contentPageNavigationServiceMock.Object;
            var userEventDetailViewModel = new UserEventDetailViewModel(dbStorageProvider, mockDialogService,
                mockContentPageNavigationServiceMock) {SelectedEvent = new UserEvent {Code = "A101"}};

            var userStorage = dbStorageProvider.GetEventModelStorage<UserEvent>();
            await userStorage.InsertAllAsync(new List<UserEvent>
            {
                new UserEvent
                {
                    Id = 1, Code = "A101", Title = "A101", Detail = "A101",
                    StartDate = DateTime.Today.AddDays(-1), EndDate = DateTime.Today.AddDays(7),
                    TimeOfDay = TimeSpan.Zero,
                    DaySpan = 2,
                    IsRepeat = true
                },
                new UserEvent
                {
                    Id = 2, Code = "A101", Title = "A102", Detail = "A102",
                    StartDate = DateTime.Today.AddDays(-1), EndDate = DateTime.Today.AddDays(7),
                    TimeOfDay = TimeSpan.Zero,
                    DaySpan = 2,
                    IsRepeat = true
                },
                new UserEvent
                {
                    Id = 3, Code = "A101", Title = "A103", Detail = "A103",
                    StartDate = DateTime.Today.AddDays(-1), EndDate = DateTime.Today.AddDays(7),
                    TimeOfDay = TimeSpan.Zero,
                    DaySpan = 2,
                    IsRepeat = true
                },
                new UserEvent
                {
                    Id = 4, Code = "A101", Title = "A104", Detail = "A104",
                    StartDate = DateTime.Today.AddDays(-1), EndDate = DateTime.Today.AddDays(7),
                    TimeOfDay = TimeSpan.Zero,
                    DaySpan = 3,
                    IsRepeat = true
                },
                new UserEvent
                {
                    Id = 5, Code = "B101", Title = "B101", Detail = "B101",
                    StartDate = DateTime.Today.AddDays(-1), EndDate = DateTime.Today.AddDays(7),
                    TimeOfDay = TimeSpan.Zero,
                    DaySpan = 2,
                    IsRepeat = false
                }
            });
            var dbData = await userStorage.GetAllAsync();
            Assert.AreEqual(5, dbData.Count);

            userEventDetailViewModel.SelectedEvent = new UserEvent {IsRepeat = true, Code = "A101"};
            await userEventDetailViewModel.PageAppearingCommandFunction();
            Assert.AreEqual(2, userEventDetailViewModel.UserEventDetail.EventPeriods.Count);

            await dbStorageProvider.CloseConnectionAsync();
        }

        [Test]
        public async Task CrudTest()
        {
            var dbStorageProvider = new DbStorageProvider();
            await dbStorageProvider.CheckInitialization();
            var dialogServiceMock = new Mock<IDialogService>();
            var mockDialogService = dialogServiceMock.Object;
            var contentPageNavigationServiceMock = new Mock<IContentPageNavigationService>();
            var mockContentPageNavigationServiceMock = contentPageNavigationServiceMock.Object;
            var userEventDetailViewModel = new UserEventDetailViewModel(dbStorageProvider, mockDialogService,
                mockContentPageNavigationServiceMock) {SelectedEvent = new UserEvent {Code = "A101"}};

            var userStorage = dbStorageProvider.GetEventModelStorage<UserEvent>();
            await userStorage.InsertAllAsync(new List<UserEvent>
            {
                new UserEvent
                {
                    Id = 1, Code = "A101", Title = "A101", Detail = "A101",
                    StartDate = DateTime.Today.AddDays(-1), EndDate = DateTime.Today.AddDays(3),
                    TimeOfDay = TimeSpan.Zero,
                    DaySpan = 2,
                    IsRepeat = true
                },
                new UserEvent
                {
                    Id = 2, Code = "A101", Title = "A101", Detail = "A101",
                    StartDate = DateTime.Today.AddDays(-1), EndDate = DateTime.Today.AddDays(3),
                    TimeOfDay = TimeSpan.Zero,
                    DaySpan = 2,
                    IsRepeat = true
                },
                new UserEvent
                {
                    Id = 3, Code = "A101", Title = "A101", Detail = "A101",
                    StartDate = DateTime.Today.AddDays(-1), EndDate = DateTime.Today.AddDays(3),
                    TimeOfDay = TimeSpan.Zero,
                    DaySpan = 2,
                    IsRepeat = true
                },
                new UserEvent
                {
                    Id = 4, Code = "A101", Title = "A101", Detail = "A101",
                    StartDate = DateTime.Today.AddDays(-1), EndDate = DateTime.Today,
                    TimeOfDay = TimeSpan.Zero,
                    DaySpan = 3,
                    IsRepeat = true
                },
                new UserEvent
                {
                    Id = 5, Code = "B101", Title = "B101", Detail = "B101",
                    StartDate = DateTime.Today.AddDays(-1), EndDate = DateTime.Today.AddDays(7),
                    TimeOfDay = TimeSpan.Zero,
                    DaySpan = 2,
                    IsRepeat = false
                }
            });
            var dbData = await userStorage.GetAllAsync();
            Assert.AreEqual(5, dbData.Count);

            userEventDetailViewModel.SelectedEvent = new UserEvent {IsRepeat = true, Code = "A101",Title = "A101"};
            await userEventDetailViewModel.PageAppearingCommandFunction();
            Assert.AreEqual(2, userEventDetailViewModel.UserEventDetail.EventPeriods.Count);

            userEventDetailViewModel.AddPeriodFunction();
            Assert.AreEqual(3, userEventDetailViewModel.UserEventDetail.EventPeriods.Count);

            userEventDetailViewModel.RemovePeriodFunction(userEventDetailViewModel.UserEventDetail.EventPeriods.Last());
            Assert.AreEqual(2, userEventDetailViewModel.UserEventDetail.EventPeriods.Count);

            userEventDetailViewModel.UserEventDetail.EventPeriods.Add(new UserEventPeriod
            {
                StartDate = DateTime.Today.AddDays(-1), EndDate = DateTime.Today, TimeOfDay = TimeSpan.Zero, DaySpan = 1
            });
            await userEventDetailViewModel.EditDoneFunction();
            dbData = await userStorage.GetAllAsync();
            Assert.AreEqual(5 + 2, dbData.Count);

            dialogServiceMock.Setup(x => x.DisplayAlert("警告", "确定删除有关本事件的所有时间段？", "Yes", "No")).ReturnsAsync(true);
            await userEventDetailViewModel.DeleteAllFunction();
            dbData = await userStorage.GetAllAsync();
            Assert.AreEqual(1, dbData.Count);

            await dbStorageProvider.CloseConnectionAsync();
        }
    }
}