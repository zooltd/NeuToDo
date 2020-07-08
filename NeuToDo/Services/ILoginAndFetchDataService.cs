using NeuToDo.Models;
using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public interface ILoginAndFetchDataService
    {
        Task<bool> LoginAndFetchDataAsync(ServerType serverType, string userId, string password);
    }
}