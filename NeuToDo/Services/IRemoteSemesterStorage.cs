using System.Collections.Generic;
using System.Threading.Tasks;
using NeuToDo.Models;

namespace NeuToDo.Services
{
    public interface IRemoteSemesterStorage
    {
        Task<List<Semester>> GetSemesterListAsync();
    }
}