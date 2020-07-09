using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Moq;
using NeuToDo.Models;
using NeuToDo.Services;
using NeuToDo.ViewModels;
using NUnit.Framework;

namespace NeuToDo.UnitTest.ViewModels
{
    public class MoocEventDetailViewModelTest
    {
        [SetUp, TearDown]
        public static void RemoveDatabaseFile()
        {
            File.Delete(DbStorageProvider.DbPath);
        }

        [Test]
        public void PageAppearingCommandFunctionTest()
        {
            var dbStorageProviderMock = new Mock<DbStorageProvider>();
            var mockDbStorageProvider = dbStorageProviderMock.Object;
            var dialogServiceMock = new Mock<IDialogService>();
            var mockDialogService = dialogServiceMock.Object;
            var contentPageNavigationServiceMock = new Mock<IContentPageNavigationService>();
            var mockContentPageNavigationServiceMock = contentPageNavigationServiceMock.Object;
            var now = DateTime.Now;
            var moocEventDetailViewModel = new MoocEventDetailViewModel(mockDbStorageProvider, mockDialogService,
                mockContentPageNavigationServiceMock) {SelectedEvent = new MoocEvent {Time = now}};

            moocEventDetailViewModel.PageAppearingCommandFunction();
            Assert.AreEqual(DateTime.Today, moocEventDetailViewModel.MoocEventDetail.EventDate);
            Assert.AreEqual(now.TimeOfDay, moocEventDetailViewModel.MoocEventDetail.EventTime);
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
            var now = DateTime.Now;
            var temp = new MoocEvent
            {
                Id = 1, Code = "A101", Title = "A101", Detail = "C101", IsDone = false,
                Time = DateTime.Today
            };
            var moocEventDetailViewModel = new MoocEventDetailViewModel(dbStorageProvider, mockDialogService,
                mockContentPageNavigationServiceMock) {SelectedEvent = temp};
            moocEventDetailViewModel.PageAppearingCommandFunction();
            var moocStorage = dbStorageProvider.GetEventModelStorage<MoocEvent>();
            await moocStorage.InsertAllAsync(new List<MoocEvent>
            {
                new MoocEvent
                {
                    Id = 1, Code = "A101", Title = "A101", Detail = "C101", IsDone = false,
                    Time = DateTime.Today
                },
                new MoocEvent
                {
                    Id = 2, Code = "A101", Title = "A101", Detail = "C101", IsDone = false,
                    Time = DateTime.Today
                },
                new MoocEvent
                {
                    Id = 3, Code = "A101", Title = "A101", Detail = "A101", IsDone = false,
                    Time = DateTime.Today.AddDays(-1)
                },
                new MoocEvent
                {
                    Id = 4, Code = "B101", Title = "A102", Detail = "A102", IsDone = false,
                    Time = DateTime.Today.AddDays(-1)
                }
            });
            var dbData = await moocStorage.GetAllAsync();
            Assert.AreEqual(4, dbData.Count);

            dialogServiceMock.Setup(x => x.DisplayAlert("警告", "确定删除本事件？", "Yes", "No")).ReturnsAsync(true);
            await moocEventDetailViewModel.DeleteThisEventFunction();
            dbData = await moocStorage.GetAllAsync();
            Assert.AreEqual(3, dbData.Count);

            dialogServiceMock.Setup(x => x.DisplayAlert("警告", "确定删除有关本课程的所有课程/作业/测试信息？", "Yes", "No"))
                .ReturnsAsync(true);
            await moocEventDetailViewModel.DeleteAllFunction();
            dbData = await moocStorage.GetAllAsync();
            Assert.AreEqual(1, dbData.Count);

            moocEventDetailViewModel.SelectedEvent = new MoocEvent{Id = 5, Code = "C101", Title = "C101", Detail = "C101", IsDone = false,
                Time = DateTime.Today.AddDays(-2)};

            moocEventDetailViewModel.PageAppearingCommandFunction();
            await moocEventDetailViewModel.SaveThisEventFunction();
            dbData = await moocStorage.GetAllAsync();
            Assert.AreEqual(2, dbData.Count);

            await dbStorageProvider.CloseConnectionAsync();
        }
    }
}