using NeuToDo.Models;
using NeuToDo.Services;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace NeuToDo.UnitTest.Services
{
    public class EventModelStorageTest
    {
        [SetUp, TearDown]
        public static void RemoveDatabaseFile() => File.Delete(EventModelStorageProvider.DbPath);

        // [Test]
        public async Task TestCrud()
        {
            IEventModelStorageProvider storageProvider = new EventModelStorageProvider();

            var neuEventModelStorage = await storageProvider.GetDatabaseConnection<NeuEvent>();
            var eventList = new List<NeuEvent>
            {
                new NeuEvent
                {
                    Id = 1, Code = "A101", Title = "A101", Detail = "A101", IsDone = false,
                    Starting = DateTimeOffset.Now
                },
                new NeuEvent
                {
                    Id = 2, Code = "A102", Title = "A102", Detail = "A102", IsDone = false,
                    Starting = DateTimeOffset.Now
                },
            };
            var e = new NeuEvent
                {Id = 3, Code = "A103", Title = "A103", Detail = "A103", IsDone = false, Starting = DateTimeOffset.Now};

            await neuEventModelStorage.InsertAsync(e);

            await neuEventModelStorage.InsertAllAsync(eventList);

            var eventListFromStorage = await neuEventModelStorage.GetAllAsync();

            Assert.AreEqual(eventListFromStorage.Count, eventList.Count + 1);

            await neuEventModelStorage.ClearTableAsync();

            var emptyEventList = await neuEventModelStorage.GetAllAsync();

            Assert.AreEqual(emptyEventList.Count, 0);

            await neuEventModelStorage.CloseConnectionAsync();
        }
    }
}