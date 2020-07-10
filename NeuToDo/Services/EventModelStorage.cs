using NeuToDo.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public class EventModelStorage<T> : IEventModelStorage<T>
        where T : EventModel, new()
    {
        private readonly SQLiteAsyncConnection _connection;

        public EventModelStorage(SQLiteAsyncConnection connection)
        {
            _connection = connection;
        }

        public async Task InsertAsync(T t)
        {
            await _connection.InsertAsync(t);
        }

        public async Task InsertOrReplaceAsync(T t)
        {
            await _connection.InsertOrReplaceAsync(t);
        }

        public async Task InsertAllAsync(IEnumerable<T> eventList)
        {
            await _connection.InsertAllAsync(eventList);
        }

        public async Task UpdateAsync(T t)
        {
            await _connection.UpdateAsync(t);
        }

        public async Task UpdateAll(IEnumerable<T> eventList)
        {
            await _connection.UpdateAllAsync(eventList);
        }

        public async Task DeleteAllAsync(Expression<Func<T, bool>> predExpr)
        {
            await _connection.Table<T>().DeleteAsync(predExpr);
        }

        public async Task DeleteAsync(T t)
        {
            await _connection.DeleteAsync(t);
        }

        public async Task ClearTableAsync()
        {
            await _connection.DeleteAllAsync<T>();
        }

        public async Task<List<T>> GetAllAsync() =>
            await _connection.Table<T>().ToListAsync();

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predExpr)
        {
            return await _connection.Table<T>().Where(predExpr).ToListAsync();
        }

        public async Task<bool> ExistAsync(Expression<Func<T, bool>> predExpr)
        {
            return await _connection.Table<T>().Where(predExpr).CountAsync() > 0;
        }

        public async Task MergeAsync(IEnumerable<T> eventList)
        {
            var dataInDb = await GetAllAsync();
            switch (typeof(T).Name)
            {
                case nameof(NeuEvent):
                    foreach (var e in eventList)
                    {
                        var index = dataInDb.FindIndex(x => (x.Time == e.Time) && (x.Code == e.Code));
                        if (index >= 0)
                            dataInDb[index] = e;
                        else
                            dataInDb.Add(e);
                    }

                    break;
                case nameof(MoocEvent):
                    //TODO 同名，同时间，同属某课程的event存在？
                    foreach (var e in eventList)
                    {
                        var index = dataInDb.FindIndex(x =>
                            (x.Title == e.Title) && (x.Time == e.Time) && (x.Code == e.Code));
                        if (index >= 0)
                            dataInDb[index] = e;
                        else
                            dataInDb.Add(e);
                    }

                    break;
            }

            //TODO transaction
            await _connection.RunInTransactionAsync((connection =>
            {
                connection.DeleteAll<T>();
                connection.InsertAll(dataInDb);
            }));
        }
    }
}