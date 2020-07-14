using NeuToDo.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public interface ISemesterStorage
    {
        Task InsertOrReplaceAsync(Semester semester);

        Task<Semester> GetAsync(int id);

        Task<List<Semester>> GetAllAsync(Expression<Func<Semester, bool>> predicate);

        Task<List<Semester>> GetAllOrderedByBaseDateAsync();

        Task<List<Semester>> GetAllAsync();

        [Obsolete]
        Task<int> GetCountAsync();

        Task InsertAsync(Semester semester);

        Task InsertAllAsync(IList<Semester> semesters);

        [Obsolete]
        Task UpdateAsync(Semester semester);

        [Obsolete]
        Task UpdateAllAsync(IList<Semester> semesters);


        Task<bool> ExistAsync(Expression<Func<Semester, bool>> predExpr);
    }
}