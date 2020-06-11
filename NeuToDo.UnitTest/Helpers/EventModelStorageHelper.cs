using System.IO;
using System.Threading.Tasks;
using NeuToDo.Models;
using NeuToDo.Services;

namespace NeuToDo.UnitTest.Helpers
{
    public class EventModelStorageHelper
    {
        public static readonly string DbPath = EventModelStorageProvider.DbPath;

        public static readonly IEventModelStorageProvider StorageProvider = new EventModelStorageProvider();

        public static void RemoveDatabaseFile() => File.Delete(DbPath);
    }
}