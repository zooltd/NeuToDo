using NeuToDo.Models;
using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public interface ICampusStorageService
    {
        Task<Campus> GetOrSelectCampus();

        Campus GetCampus();

        Task UpdateCampus();

        void SaveCampus(Campus campus);
    }
}