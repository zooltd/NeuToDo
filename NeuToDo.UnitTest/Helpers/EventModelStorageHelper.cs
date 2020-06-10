using System.IO;
using System.Threading.Tasks;
using NeuToDo.Models;
using NeuToDo.Services;

namespace NeuToDo.UnitTest.Helpers
{
    public class EventModelStorageHelper
    {
        public static readonly string DbPath = EventModelStorage<NeuEventModel>.DbPath;

        private static IEventModelStorageProvider _storageProvider = new EventModelStorageProvider();

        public static IEventModelStorage<NeuEventModel> GetNeuEventModelStorage() => new EventModelStorage<NeuEventModel>();

        public static void RemoveDatabaseFile() => File.Delete(DbPath);
    }
}