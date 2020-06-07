using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NeuToDo.Models;
using SQLite;

namespace NeuToDo.Services
{
    public class CourseEventStorage : IEventStorage
    {
        private SQLiteAsyncConnection _connection = null;

        public SQLiteAsyncConnection Connection => _connection ?? (_connection = new SQLiteAsyncConnection(DbPath));

        private const string DbName = "events.sqlite3";

        private static readonly string DbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DbName);


        public async Task CreateDatabase()
        {
            await Connection.CreateTableAsync<CourseEventModel>();
        }

        public async Task ClearDatabase()
        {
            await Connection.DeleteAllAsync<CourseEventModel>();
        }

        public async Task Insert(EventModel e)
        {
            await Connection.InsertAsync(e);
        }

        public async Task InsertAll(IList<EventModel> eventList)
        {
            await Connection.InsertAllAsync(eventList);
        }

        public async Task<List<EventModel>> GetAll()
        {
            var temp = await Connection.Table<CourseEventModel>().ToListAsync();
            return temp.ConvertAll(x => (EventModel) x);
        }
    }
}