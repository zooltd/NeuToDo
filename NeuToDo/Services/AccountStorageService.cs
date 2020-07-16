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
            _preferenceStorageProvider.Remove(serverType + nameof(Account.UserName));
            _secureStorageProvider.Remove(serverType + nameof(Account.Password));
            _preferenceStorageProvider.Remove(serverType + nameof(Account.LastUpdateTime));
            if (serverType == ServerType.WebDav)
            {
                _preferenceStorageProvider.Remove(serverType + nameof(Account.BaseUri));
                _preferenceStorageProvider.Remove(serverType + nameof(Account.Remarks));
            }
        }

        public async Task SaveAccountAsync(ServerType serverType, string userName, string password, string updateTime)
        {
            _preferenceStorageProvider.Set(serverType + "Id", userName);
            await _secureStorageProvider.SetAsync(serverType + "Pd", password);
            _preferenceStorageProvider.Set(serverType + "Time", updateTime);
        }


        public async Task SaveAccountAsync(ServerType serverType, Account account)
        {
            _preferenceStorageProvider.Set(serverType + nameof(Account.UserName), account.UserName);
            await _secureStorageProvider.SetAsync(serverType + nameof(Account.Password), account.Password);
            _preferenceStorageProvider.Set(serverType + nameof(Account.LastUpdateTime), account.LastUpdateTime);
            if (serverType == ServerType.WebDav)
            {
                _preferenceStorageProvider.Set(serverType + nameof(Account.BaseUri), account.BaseUri);
                _preferenceStorageProvider.Set(serverType + nameof(Account.Remarks), account.Remarks);
            }
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
                    : string.Empty,
                Remarks = serverType == ServerType.WebDav
                    ? _preferenceStorageProvider.Get(serverType + nameof(Account.Remarks), string.Empty)
                    : string.Empty
            };
        }


        public event EventHandler UpdateData;

        public virtual void OnUpdateData()
        {
            UpdateData?.Invoke(this, EventArgs.Empty);
        }


        public static readonly Account DefaultAccount = new Account
            {Remarks = "我的私有云盘", BaseUri = "我的服务器地址", UserName = "我的用户名"};
    }
}