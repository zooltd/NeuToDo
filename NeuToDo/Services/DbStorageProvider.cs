using NeuToDo.Models;
using SQLite;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public class DbStorageProvider : IDbStorageProvider
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

        private IEventModelStorage<NeuEvent> _neuStorage;
        private IEventModelStorage<MoocEvent> _moocStorage;
        private IEventModelStorage<UserEvent> _userStorage;

        public IEventModelStorage<T> GetEventModelStorage<T>()
            where T : EventModel, new()
        {
            return typeof(T).Name switch
            {
                nameof(NeuEvent) => (_neuStorage ??= new EventModelStorage<NeuEvent>(_databaseConnection.Value)) as
                IEventModelStorage<T>,
                nameof(MoocEvent) => (_moocStorage ??= new EventModelStorage<MoocEvent>(_databaseConnection.Value)) as
                IEventModelStorage<T>,
                nameof(UserEvent) => (_userStorage ??= new EventModelStorage<UserEvent>(_databaseConnection.Value)) as
                IEventModelStorage<T>,
                _ => null
            };
        }

        private ISemesterStorage _semesterStorage;

        public ISemesterStorage GetSemesterStorage()
        {
            return _semesterStorage ??= new SemesterStorage(_databaseConnection.Value);
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