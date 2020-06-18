using System;
using System.Threading.Tasks;
using NeuToDo.Models;

namespace NeuToDo.Services
{
    public interface ILoginAndFetchDataService
    {
        public event EventHandler GetData;

        Task<bool> LoginAndFetchDataAsync(ServerType serverType, string userId, string password);
    }
}