using System.IO;
using System.Threading.Tasks;
using NeuToDo.Models;
using NeuToDo.Services;
using NUnit.Framework;

namespace NeuToDo.UnitTest.Services
{
    public class EventModelStorageProviderTest
    {
        [SetUp, TearDown]
        public static void RemoveDatabaseFile() => File.Delete(EventModelStorageProvider.DbPath);

        [Test]
        public async Task TestGetDatabaseConnection()
        {
            Assert.IsFalse(File.Exists(EventModelStorageProvider.DbPath));

            var storageProvider = new EventModelStorageProvider();
            await storageProvider.GetDatabaseConnection<NeuEventModel>();

            Assert.IsTrue(File.Exists(EventModelStorageProvider.DbPath));

            await storageProvider.CloseConnectionAsync();
        }
    }
}