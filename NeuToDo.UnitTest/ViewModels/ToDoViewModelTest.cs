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
    public class ToDoViewModelTest
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
            var contentPageNavigationServiceMock = new Mock<IContentPageNavigationService>();
            var academicCalendarService = new AcademicCalendarService(dbStorageProvider);
            var fetchSemesterDataServiceMock = new Mock<IFetchSemesterDataService>();
            var toDoViewModel = new ToDoViewModel(dbStorageProvider, contentPageNavigationServiceMock.Object,
                academicCalendarService, fetchSemesterDataServiceMock.Object);

            await toDoViewModel.PageAppearingCommandFunction();

            Assert.AreEqual(AcademicCalendarService.EmptySemester, toDoViewModel.Semester);
            Assert.AreEqual(0, toDoViewModel.WeekNo);

            await dbStorageProvider.CloseConnectionAsync();
        }

        [Test]
        public async Task SearchCommandFunction()
        {
            var dbStorageProviderMock = new Mock<IDbStorageProvider>();
            var contentPageNavigationServiceMock = new Mock<IContentPageNavigationService>();
            var academicCalendarServiceMock = new Mock<IAcademicCalendarService>();
            var fetchSemesterDataServiceMock = new Mock<IFetchSemesterDataService>();
            var toDoViewModel = new ToDoViewModel(dbStorageProviderMock.Object, contentPageNavigationServiceMock.Object,
                academicCalendarServiceMock.Object, fetchSemesterDataServiceMock.Object);
            toDoViewModel.EventDict = new Dictionary<DateTime, List<EventModel>>
            {
                {
                    DateTime.Today,
                    new List<EventModel>
                    {
                        new NeuEvent {Title = "A101", Detail = "A101"}, new MoocEvent {Title = "B102", Detail = "A102"}
                    }
                },
                {
                    DateTime.Today.AddDays(-1),
                    new List<EventModel>
                    {
                        new UserEvent {Title = "C103", Detail = "C103"}, new MoocEvent {Title = "A104", Detail = "D104"}
                    }
                }
            };
            toDoViewModel.QueryString = "A";
            toDoViewModel.SearchCommandFunction();
            await Task.Delay(100);
            Assert.AreEqual(3, toDoViewModel.SearchResult.Count);
        }
    }
}