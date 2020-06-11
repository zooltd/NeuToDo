using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NeuToDo.Services;
using NeuToDo.ViewModels;
using NUnit.Framework;

namespace NeuToDo.UnitTest.ViewModels {
    [TestFixture]
    public class ToDoCalendarViewModelTest {
        [Test]
        public async Task TestPageAppearingCommandFunction() {
            var eventModelStorageProviderMock = new Mock<IEventModelStorageProvider>();
            var mockEventModelStorageProvider =
                eventModelStorageProviderMock.Object;
            var toDoCalendarViewModel = new ToDoCalendarViewModel(mockEventModelStorageProvider);
        }
    }
}