using NeuToDo.Models;
using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public interface IAccountStorageService
    {
        bool AccountExist(ServerType serverType);

        string GetUserName(ServerType serverType);

        string GetUpdateTime(ServerType serverType);

        Task<string> GetPasswordAsync(ServerType serverType);

        void RemoveAccountHistory(ServerType serverType);

        Task SaveAccountAsync(ServerType serverType, string userName, string password, string updateTime);
    }
}