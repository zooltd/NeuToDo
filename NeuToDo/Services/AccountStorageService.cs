using System;
using NeuToDo.Models;
using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public class AccountStorageService : IAccountStorageService
    {
        private readonly IPreferenceStorageProvider _preferenceStorageProvider;
        private readonly ISecureStorageProvider _secureStorageProvider;

        public AccountStorageService(IPreferenceStorageProvider preferenceStorageProvider,
            ISecureStorageProvider secureStorageProvider)
        {
            _preferenceStorageProvider = preferenceStorageProvider;
            _secureStorageProvider = secureStorageProvider;
        }

        public bool AccountExist(ServerType serverType)
            => _preferenceStorageProvider.ContainsKey(serverType + "Id");

        public string GetUserName(ServerType serverType)
            => _preferenceStorageProvider.Get(serverType + "Id", string.Empty);

        public string GetUpdateTime(ServerType serverType)
            => _preferenceStorageProvider.Get(serverType + "Time", "未知的时间");

        public async Task<string> GetPasswordAsync(ServerType serverType)
            => await _secureStorageProvider.TryGetAsync(serverType + "Pd", string.Empty);

        public void RemoveAccountHistory(ServerType serverType)
        {
            _preferenceStorageProvider.Remove(serverType + "Id");
            _secureStorageProvider.Remove(serverType + "Pd");
            _preferenceStorageProvider.Remove(serverType + "Time");
        }

        public async Task SaveAccountAsync(ServerType serverType, string userName, string password, string updateTime)
        {
            _preferenceStorageProvider.Set(serverType + "Id", userName);
            await _secureStorageProvider.SetAsync(serverType + "Pd", password);
            _preferenceStorageProvider.Set(serverType + "Time", updateTime);
        }

        public async Task<Account> GetAccountAsync(ServerType serverType)
        {
            if (!_preferenceStorageProvider.ContainsKey(serverType + nameof(Account.UserName))) return null;

            return new Account
            {
                UserName =
                    _preferenceStorageProvider.Get(serverType + nameof(Account.UserName), string.Empty),
                Password =
                    await _secureStorageProvider.TryGetAsync(serverType + nameof(Account.Password), string.Empty),
                LastUpdateTime =
                    _preferenceStorageProvider.Get(serverType + nameof(Account.LastUpdateTime), string.Empty),
                BaseUri = serverType == ServerType.WebDav
                    ? _preferenceStorageProvider.Get(serverType + nameof(Account.BaseUri), string.Empty)
                    : string.Empty
            };
        }
    }

    public class Account
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string LastUpdateTime { get; set; }
        public string BaseUri { get; set; }
    }
}