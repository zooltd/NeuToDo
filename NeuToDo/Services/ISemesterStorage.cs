﻿using NeuToDo.Models;
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

        Task<int> GetCountAsync();

        Task InsertAsync(Semester semester);

        Task UpdateAsync(Semester semester);
    }
}