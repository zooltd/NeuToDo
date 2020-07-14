using NeuToDo.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public class SemesterStorage : ISemesterStorage
    {
        private readonly SQLiteAsyncConnection _connection;

        public SemesterStorage(SQLiteAsyncConnection databaseConnectionValue)
        {
            _connection = databaseConnectionValue;
        }

        public async Task InsertOrReplaceAsync(Semester semester)
            => await _connection.InsertOrReplaceAsync(semester);

        public async Task<Semester> GetAsync(int id)
            => await _connection.GetAsync<Semester>(id);

        public async Task<List<Semester>> GetAllAsync(Expression<Func<Semester, bool>> predicate) =>
            await _connection.Table<Semester>().Where(predicate).ToListAsync();

        public async Task<List<Semester>> GetAllOrderedByBaseDateAsync() =>
            await _connection.Table<Semester>().OrderByDescending(x => x.BaseDate).ToListAsync();

        public async Task<List<Semester>> GetAllAsync()
            => await _connection.Table<Semester>().ToListAsync();

        public async Task<int> GetCountAsync()
            => await _connection.Table<Semester>().CountAsync();

        public async Task InsertAsync(Semester semester)
            => await _connection.InsertAsync(semester);

        public async Task InsertAllAsync(IList<Semester> semesters)
            => await _connection.InsertAllAsync(semesters);

        public async Task UpdateAsync(Semester semester)
            => await _connection.UpdateAsync(semester);

        public async Task UpdateAllAsync(IList<Semester> semesters)
            => await _connection.UpdateAllAsync(semesters);

        public async Task<bool> ExistAsync(Expression<Func<Semester, bool>> predExpr)
            => await _connection.Table<Semester>().Where(predExpr).CountAsync() > 0;
    }
}