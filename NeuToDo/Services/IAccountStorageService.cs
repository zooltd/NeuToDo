using System;
using NeuToDo.Models;
using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public interface IAccountStorageService
    {
        void RemoveAccountHistory(ServerType serverType);


        Task SaveAccountAsync(ServerType serverType, Account account);

        /// <summary>
        /// Get the specific account of the server type; Return null if it does not exist
        /// </summary>
        /// <param name="serverType"></param>
        /// <returns></returns>
        Task<Account> GetAccountAsync(ServerType serverType);


        public event EventHandler UpdateData;

        public void OnUpdateData();
    }
}