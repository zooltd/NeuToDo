using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NeuToDo.Models;

namespace NeuToDo.Services
{
    public interface ISemesterStorage
    {
        Task InsertAsync(Semester semester);

        Task<List<Semester>> GetAllAsync(Expression<Func<Semester, bool>> predicate);

        Task<Semester> GetSemesterByMaxBaseDateAsync();

        Task<List<Semester>> GetAllAsync();
    }
}