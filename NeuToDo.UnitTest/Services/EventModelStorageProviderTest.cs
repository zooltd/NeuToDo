using Moq;
using NeuToDo.Models;
using NeuToDo.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NeuToDo.UnitTest.Services
{
    public class EventModelStorageProviderTest
    {
        [SetUp, TearDown]
        public static void RemoveDatabaseFile()
        {
            File.Delete(DbStorageProvider.DbPath);
        }

        [Test]
        public async Task GetDatabaseConnectionTest()
        {
            Assert.IsFalse(File.Exists(DbStorageProvider.DbPath));

            var storageProvider = new DbStorageProvider();
            await storageProvider.CheckInitialization();
            storageProvider.GetEventModelStorage<NeuEvent>();

            Assert.IsTrue(File.Exists(DbStorageProvider.DbPath));

            await storageProvider.CloseConnectionAsync();
        }

        [Test]
        public async Task GetSemesterTableConnectionTest()
        {
            Assert.IsFalse(File.Exists(DbStorageProvider.DbPath));

            var storageProvider = new DbStorageProvider();
            await storageProvider.CheckInitialization();
            storageProvider.GetSemesterStorage();

            Assert.IsTrue(File.Exists(DbStorageProvider.DbPath));

            await storageProvider.CloseConnectionAsync();
        }

        [Test]
        public void OnUpdateDataTest()
        {
            var storageProvider = new DbStorageProvider();
            var voidClassMock = new Mock<VoidClass>();
            var classMockMock = voidClassMock.Object;
            storageProvider.UpdateData += classMockMock.VoidFunc;
            storageProvider.OnUpdateData();
            // voidClassMock.Verify(x => x.VoidFunc(dbStorageProvider, EventArgs.Empty), Times.Once);
        }
    }

    public class VoidClass
    {
        public void VoidFunc(object? sender, EventArgs e)
        {
        }
    }
}