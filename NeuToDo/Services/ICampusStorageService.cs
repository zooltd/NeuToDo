using System.Threading.Tasks;
using NeuToDo.Models;

namespace NeuToDo.Services
{
    public interface ICampusStorageService
    {
        Task<Campus> GetCampus();

        Task UpdateCampus();

        void SaveCampus(Campus campus);
    }
}