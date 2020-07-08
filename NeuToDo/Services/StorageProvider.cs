using NeuToDo.Models;
using SQLite;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public class StorageProvider : IStorageProvider
    {
        public const string DbName = "events.sqlite3";

        public static readonly string DbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder
                .LocalApplicationData), DbName);

        private readonly Lazy<SQLiteAsyncConnection> _databaseConnection =
            new Lazy<SQLiteAsyncConnection>(() =>
                new SQLiteAsyncConnection(DbPath));


        public async Task CheckInitialization()
        {
            //TODO 一次查找完毕
            if (!await TableExists(nameof(NeuEvent), _databaseConnection.Value))
                await _databaseConnection.Value.CreateTablesAsync(CreateFlags.None, typeof(NeuEvent));
            if (!await TableExists(nameof(MoocEvent), _databaseConnection.Value))
                await _databaseConnection.Value.CreateTablesAsync(CreateFlags.None, typeof(MoocEvent));
            if (!await TableExists(nameof(UserEvent), _databaseConnection.Value))
                await _databaseConnection.Value.CreateTablesAsync(CreateFlags.None, typeof(UserEvent));
            if (!await TableExists(nameof(Semester), _databaseConnection.Value))
                await _databaseConnection.Value.CreateTablesAsync(CreateFlags.None, typeof(Semester));
        }


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

        private ISemesterStorage _semesterStorage;

        public async Task<ISemesterStorage> GetSemesterStorage()
        {
            if (_semesterStorage != null) return _semesterStorage;
            if (!await TableExists(nameof(Semester), _databaseConnection.Value))
            {
                await _databaseConnection.Value
                    .CreateTablesAsync(CreateFlags.None, typeof(Semester));
            }

            _semesterStorage = new SemesterStorage(_databaseConnection.Value);
            return _semesterStorage;
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