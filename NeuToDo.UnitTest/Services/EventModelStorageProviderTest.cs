using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Moq;
using NeuToDo.Models;
using NeuToDo.Services;
using NUnit.Framework;

namespace NeuToDo.UnitTest.Services
{
    public class EventModelStorageProviderTest
    {
        [SetUp, TearDown]
        public static void RemoveDatabaseFile()
        {
            File.Delete(StorageProvider.DbPath);
        }

        [Test]
        public async Task GetDatabaseConnectionTest()
        {
            Assert.IsFalse(File.Exists(StorageProvider.DbPath));

            var storageProvider = new StorageProvider();
            await storageProvider.GetEventModelStorage<NeuEvent>();

            Assert.IsTrue(File.Exists(StorageProvider.DbPath));

            await storageProvider.CloseConnectionAsync();
        }

        [Test]
        public async Task GetSemesterTableConnectionTest()
        {
            Assert.IsFalse(File.Exists(StorageProvider.DbPath));

            var storageProvider = new StorageProvider();
            await storageProvider.GetSemesterStorage();

            Assert.IsTrue(File.Exists(StorageProvider.DbPath));

            await storageProvider.CloseConnectionAsync();
        }

        [Test]
        public void OnUpdateDataTest()
        {
            var storageProvider = new StorageProvider();
            var voidClassMock = new Mock<VoidClass>();
            var classMockMock = voidClassMock.Object;
            storageProvider.UpdateData += classMockMock.VoidFunc;
            storageProvider.OnUpdateData();
            // voidClassMock.Verify(x => x.VoidFunc(storageProvider, EventArgs.Empty), Times.Once);
        }
    }

    public class VoidClass
    {
        public void VoidFunc(object? sender, EventArgs e)
        {
        }
    }
}