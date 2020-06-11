using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NeuToDo.Models;
using SQLite;

namespace NeuToDo.Services
{
    public class EventModelStorageProvider : IEventModelStorageProvider
    {
        public const string DbName = "events.sqlite3";

        public static readonly string DbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder
                .LocalApplicationData), DbName);

        private readonly Lazy<SQLiteAsyncConnection> _databaseConnection =
            new Lazy<SQLiteAsyncConnection>(() =>
                new SQLiteAsyncConnection(DbPath));

        public async Task<IEventModelStorage<T>> GetDatabaseConnection<T>()
            where T : EventModel, new()
        {
            if (_databaseConnection.Value.TableMappings.All(x =>
                x.MappedType.Name != typeof(T).Name))
            {
                await _databaseConnection.Value
                    .CreateTablesAsync(CreateFlags.None, typeof(T))
                    .ConfigureAwait(false);
            }

            return new EventModelStorage<T>(_databaseConnection.Value);
        }
    }
}