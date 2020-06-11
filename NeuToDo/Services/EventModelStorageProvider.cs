using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NeuToDo.Models;
using SQLite;

namespace NeuToDo.Services
{
    //TODO 存在用意
    public class EventModelStorageProvider : IEventModelStorageProvider
    {
        public const string DbName = "events.sqlite3";

        public static readonly string DbPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DbName);

        public static readonly Lazy<SQLiteAsyncConnection> DbConnectionHolder =
            new Lazy<SQLiteAsyncConnection>(() => new SQLiteAsyncConnection(DbPath));

        static SQLiteAsyncConnection DatabaseConnection => DbConnectionHolder.Value;

        public async Task<IEventModelStorage<T>> GetDatabaseConnection<T>() where T : EventModel, new()
        {
            if (DatabaseConnection.TableMappings.All(x => x.MappedType.Name != typeof(T).Name))
            {
                await DatabaseConnection.CreateTablesAsync(CreateFlags.None, typeof(T)).ConfigureAwait(false);
            }

            return new EventModelStorage<T>(DatabaseConnection);
        }
    }
}