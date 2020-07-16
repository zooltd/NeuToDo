using System;
using NeuToDo.Models;
using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public interface IAccountStorageService
    {
        [Obsolete]
        bool AccountExist(ServerType serverType);

        [Obsolete]
        string GetUserName(ServerType serverType);

        [Obsolete]
        string GetUpdateTime(ServerType serverType);

        [Obsolete]
        Task<string> GetPasswordAsync(ServerType serverType);

        void RemoveAccountHistory(ServerType serverType);

        [Obsolete]
        Task SaveAccountAsync(ServerType serverType, string userName, string password, string updateTime);

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