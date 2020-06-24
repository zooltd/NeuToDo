using NeuToDo.Models;
using SQLite;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public class EventModelStorageProvider : IEventModelStorageProvider
    {
        private const string DbName = "events.sqlite3";

        public static readonly string DbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder
                .LocalApplicationData), DbName);

        private readonly Lazy<SQLiteAsyncConnection> _databaseConnection =
            new Lazy<SQLiteAsyncConnection>(() =>
                new SQLiteAsyncConnection(DbPath));

        public async Task<IEventModelStorage<T>> GetEventModelStorage<T>()
            where T : EventModel, new()
        {
            if (!await TableExists(typeof(T).Name, _databaseConnection.Value))
            {
                await _databaseConnection.Value
                    .CreateTablesAsync(CreateFlags.None, typeof(T));
            }

            return new EventModelStorage<T>(_databaseConnection.Value);
        }

        public async Task CloseConnectionAsync()
        {
            await _databaseConnection.Value.CloseAsync();
        }


        private static async Task<bool> TableExists(string tableName,
            SQLiteAsyncConnection connection)
        {
            var sql =
                $"SELECT name FROM sqlite_master WHERE type = 'table' AND name = '{tableName}'";
            return await connection.ExecuteScalarAsync<string>(sql).ConfigureAwait(true) != null;
        }

        public event EventHandler UpdateData;

        public virtual void OnUpdateData()
        {
            UpdateData?.Invoke(this, EventArgs.Empty);
        }
    }
}