﻿using System;
using System.Threading.Tasks;
using NeuToDo.Models;
using NeuToDo.Models.SettingsModels;

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
    }
}