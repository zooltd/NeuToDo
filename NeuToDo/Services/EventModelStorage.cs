﻿using System;
using NeuToDo.Models;
using SQLite;
using System.Collections.Generic;
using System.Diagnostics;
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

        public async Task InsertAllAsync(IEnumerable<T> eventList)
        {
            await _connection.InsertAllAsync(eventList);
        }

        public async Task DeleteAllAsync(Expression<Func<T, bool>> predExpr)
        {
            await _connection.Table<T>().DeleteAsync(predExpr);
        }

        public async Task ClearTableAsync()
        {
            await _connection.DeleteAllAsync<T>();
        }

        public async Task<List<T>> GetAllAsync() =>
            await _connection.Table<T>().ToListAsync();

        public async Task<List<T>> GetAllAsync(string code)
        {
            return await _connection.Table<T>().Where(e => (e.Code == code)).ToListAsync();
        }

        public async Task MergeAsync(IList<T> eventList)
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

            await ClearTableAsync();
            await InsertAllAsync(dataInDb);
        }
    }
}