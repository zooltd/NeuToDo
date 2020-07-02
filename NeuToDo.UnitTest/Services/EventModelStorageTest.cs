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
            File.Delete(EventModelStorageProvider.DbPath);
        }

        [Test]
        public async Task CrudTest()
        {
            var storageProvider = new EventModelStorageProvider();
            var neuEventModelStorage = await storageProvider.GetEventModelStorage<NeuEvent>();

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

            Assert.AreEqual(eventListFromStorage.Count, eventList.Count + 1);

            await neuEventModelStorage.ClearTableAsync();

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

            await neuEventModelStorage.DeleteAllAsync(x => x.Code == "A101");

            eventListFromStorage = await neuEventModelStorage.GetAllAsync();

            Assert.AreEqual(eventListFromStorage.Count, 0);

            await storageProvider.CloseConnectionAsync();
        }

        [Test]
        public async Task MergeTest()
        {
            var storageProvider = new EventModelStorageProvider();
            var neuEventModelStorage = await storageProvider.GetEventModelStorage<NeuEvent>();

            var neuEventList = new List<NeuEvent>
            {
                new NeuEvent
                {
                    Id = 1, Code = "A101", Title = "A101", Detail = "A101", IsDone = false,
                    Time = new DateTime(2020, 6, 28)
                },
                new NeuEvent
                {
                    Id = 2, Code = "A102", Title = "A102", Detail = "A102", IsDone = false,
                    Time = new DateTime(2020, 6, 28)
                },
            };


            await neuEventModelStorage.InsertAllAsync(neuEventList);

            var newNeuEventList = new List<NeuEvent>
            {
                new NeuEvent
                {
                    Id = 0, Code = "A101", Title = "A101", Detail = "A101", IsDone = false,
                    Time = new DateTime(2020, 6, 28)
                },
                new NeuEvent
                {
                    Id = 0, Code = "A102", Title = "B102", Detail = "B102", IsDone = true,
                    Time = new DateTime(2020, 6, 28)
                },
                new NeuEvent
                {
                    Id = 0, Code = "A103", Title = "A103", Detail = "A103", IsDone = false,
                    Time = new DateTime(2020, 6, 28)
                }
            };

            await neuEventModelStorage.MergeAsync(newNeuEventList);

            var neuEventListFromStorage = await neuEventModelStorage.GetAllAsync();

            Assert.AreEqual(neuEventListFromStorage.Count, 3);


            var moocEventModelStorage = await storageProvider.GetEventModelStorage<MoocEvent>();

            var moocEventList = new List<MoocEvent>
            {
                new MoocEvent()
                {
                    Id = 1, Code = "A101", Title = "A101", Detail = "A101", IsDone = false,
                    Time = new DateTime(2020, 6, 28)
                },
                new MoocEvent()
                {
                    Id = 2, Code = "A102", Title = "A102", Detail = "A102", IsDone = false,
                    Time = new DateTime(2020, 6, 28)
                },
            };
            await moocEventModelStorage.InsertAllAsync(moocEventList);

            var newMoocEventList = new List<MoocEvent>
            {
                new MoocEvent
                {
                    Id = 0, Code = "A101", Title = "A101", Detail = "A101", IsDone = false,
                    Time = new DateTime(2020, 7, 28)
                },
                new MoocEvent
                {
                    Id = 0, Code = "A102", Title = "B102", Detail = "B102", IsDone = true,
                    Time = new DateTime(2020, 6, 28)
                },
                new MoocEvent
                {
                    Id = 0, Code = "A103", Title = "A103", Detail = "A103", IsDone = false,
                    Time = new DateTime(2020, 6, 28)
                },
                new MoocEvent
                {
                    Id = 0, Code = "A103", Title = "A103", Detail = "A103", IsDone = true,
                    Time = new DateTime(2020, 6, 28)
                }
            };

            await moocEventModelStorage.MergeAsync(newMoocEventList);

            var moocEventListFromStorage = await moocEventModelStorage.GetAllAsync();

            Assert.AreEqual(moocEventListFromStorage.Count, 5);

            await storageProvider.CloseConnectionAsync();
        }
    }
}