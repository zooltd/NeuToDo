﻿using NeuToDo.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public interface IEventModelStorage<T> where T : EventModel
    {
        Task InsertAsync(T t);

        Task InsertOrReplaceAsync(T t);

        Task InsertAllAsync(IEnumerable<T> eventList);

        Task UpdateAsync(T t);

        Task UpdateAll(IEnumerable<T> eventList);

        Task DeleteAllAsync(Expression<Func<T, bool>> predExpr);

        Task DeleteAsync(T t);

        Task ClearTableAsync();

        Task<List<T>> GetAllAsync();

        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predExpr);

        Task MergeAsync(IEnumerable<T> eventList);
    }
}