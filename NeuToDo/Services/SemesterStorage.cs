using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NeuToDo.Models;
using SQLite;

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

        public async Task<List<Semester>> GetAllAsync(Expression<Func<Semester, bool>> predicate)
            => await _connection.Table<Semester>().Where(predicate).ToListAsync();

        public async Task<List<Semester>> GetAllOrderedByBaseDateAsync()
            => await _connection.Table<Semester>().OrderByDescending(x => x.BaseDate).ToListAsync();

        public async Task<List<Semester>> GetAllAsync()
            => await _connection.Table<Semester>().ToListAsync();
    }
}