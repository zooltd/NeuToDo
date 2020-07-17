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
        public static void RemoveDatabaseFile()
        {
            File.Delete(DbStorageProvider.DbPath);
        }

        [Test]
        public async Task CrudTest()
        {
            var storageProvider = new DbStorageProvider();
            await storageProvider.CheckInitialization();
            var neuEventModelStorage = storageProvider.GetEventModelStorage<NeuEvent>();

            var eventList = new List<NeuEvent>
            {
                new NeuEvent
                {
                    Id = 1, Code = "A101", Title = "A101", Detail = "A101", IsDone = false,
                    Time = DateTime.Now
                },
                new NeuEvent
                {
                    Id = 2, Code = "A102", Title = "A102", Detail = "A102", IsDone = false,
                    Time = DateTime.Now
                },
            };
            var e = new NeuEvent
                {Id = 3, Code = "A103", Title = "A103", Detail = "A103", IsDone = false, Time = DateTime.Now};

            await neuEventModelStorage.InsertAsync(e);

            await neuEventModelStorage.InsertAllAsync(eventList);

            var eventListFromStorage = await neuEventModelStorage.GetAllAsync();

            Assert.AreEqual(2 + 1, eventListFromStorage.Count);

            await neuEventModelStorage.DeleteAllAsync();

            eventListFromStorage = await neuEventModelStorage.GetAllAsync();

            Assert.AreEqual(eventListFromStorage.Count, 0);

            eventList = new List<NeuEvent>
            {
                new NeuEvent
                {
                    Id = 1, Code = "A101", Title = "A101", Detail = "A101", IsDone = false,
                    Time = DateTime.Now
                },
                new NeuEvent
                {
                    Id = 2, Code = "A101", Title = "A102", Detail = "A102", IsDone = false,
                    Time = DateTime.Now
                },
                new NeuEvent
                {
                    Id = 3, Code = "A101", Title = "A101", Detail = "A101", IsDone = false,
                    Time = DateTime.Now
                },
                new NeuEvent
                {
                    Id = 4, Code = "A101", Title = "A102", Detail = "A102", IsDone = false,
                    Time = DateTime.Now
                }
            };
            await neuEventModelStorage.InsertAllAsync(eventList);

            eventListFromStorage = await neuEventModelStorage.GetAllAsync(x => x.Code == "A101");

            Assert.AreEqual(eventListFromStorage.Count, 4);

            var res = await neuEventModelStorage.ExistAsync(x => x.Code == "A101");
            Assert.AreEqual(true, res);

            await storageProvider.CloseConnectionAsync();
        }
    }
}