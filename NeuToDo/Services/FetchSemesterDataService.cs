using GalaSoft.MvvmLight.Ioc;
using NeuToDo.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public class FetchSemesterDataService : IFetchSemesterDataService
    {
        private readonly ISemesterStorage _localSemesterStorage;
        private readonly IRemoteSemesterStorage _remoteSemesterStorage;

        public FetchSemesterDataService(IDbStorageProvider dbStorageProvider,
            IRemoteSemesterStorage remoteSemesterStorage)
        {
            _localSemesterStorage = dbStorageProvider.GetSemesterStorage();
            _remoteSemesterStorage = remoteSemesterStorage;
        }

        /// <summary>
        /// 从远程Semester存储获取Semester信息并覆盖本地数据库
        /// </summary>
        /// <returns></returns>
        public async Task FetchSemesterAsync()
        {
            var semesterList = await _remoteSemesterStorage.GetSemesterListAsync();

            if (semesterList.Count == 0) return;

            await _localSemesterStorage.DeleteAllAsync();
            await _localSemesterStorage.InsertAllAsync(semesterList);
        }
    }
}