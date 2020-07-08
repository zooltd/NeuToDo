using NeuToDo.Models;
using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public interface ICampusStorageService
    {
        Task<Campus> GetCampus();

        Task UpdateCampus();

        void SaveCampus(Campus campus);
    }
}