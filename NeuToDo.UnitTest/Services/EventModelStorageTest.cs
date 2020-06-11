using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NeuToDo.Models;
using NeuToDo.Services;
using NeuToDo.UnitTest.Helpers;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace NeuToDo.UnitTest.Services
{
    public class EventModelStorageTest
    {
        [SetUp, TearDown]
        public static void RemoveDatabaseFile() => EventModelStorageHelper.RemoveDatabaseFile();


        [Test]
        public async Task TestCreateTableAsync()
        {
            Assert.IsFalse(File.Exists(EventModelStorageHelper.DbPath));

            var neuEventModelStorage =
                await EventModelStorageHelper.StorageProvider.GetDatabaseConnection<NeuEventModel>();
            Assert.IsTrue(File.Exists(EventModelStorageHelper.DbPath));
            await neuEventModelStorage.CloseAsync();
        }

        [Test]
        public async Task TestCrud()
        {
            var neuEventModelStorage =
                await EventModelStorageHelper.StorageProvider.GetDatabaseConnection<NeuEventModel>();
            var eventList = new List<NeuEventModel>
            {
                new NeuEventModel
                {
                    Id = 1, Code = "A101", Title = "A101", Detail = "A101", IsDone = false,
                    Starting = DateTimeOffset.Now
                },
                new NeuEventModel
                {
                    Id = 2, Code = "A102", Title = "A102", Detail = "A102", IsDone = false,
                    Starting = DateTimeOffset.Now
                },
            };
            var e = new NeuEventModel
                {Id = 3, Code = "A103", Title = "A103", Detail = "A103", IsDone = false, Starting = DateTimeOffset.Now};

            await neuEventModelStorage.InsertAsync(e);
            await neuEventModelStorage.InsertAllAsync(eventList);

            var eventListFromStorage = await neuEventModelStorage.GetAllAsync();

            Assert.AreEqual(eventListFromStorage.Count, eventList.Count + 1);

            await neuEventModelStorage.CloseAsync();
        }
    }
}