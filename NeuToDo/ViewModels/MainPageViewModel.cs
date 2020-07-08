using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Services;

namespace NeuToDo.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly IDbStorageProvider _dbStorageProvider;

        public MainPageViewModel(IDbStorageProvider dbStorageProvider)
        {
            _dbStorageProvider = dbStorageProvider;
        }

        /// <summary>
        /// 页面显示命令。
        /// </summary>
        private RelayCommand _pageAppearingCommand;

        /// <summary>
        /// 页面显示命令。
        /// </summary>
        public RelayCommand PageAppearingCommand => _pageAppearingCommand ??=
            new RelayCommand(async () => await PageAppearingCommandFunction());

        /// <summary>
        /// 只在启动时调用一次
        /// </summary>
        /// <returns></returns>
        private async Task PageAppearingCommandFunction()
        {
            await _dbStorageProvider.CheckInitialization();
        }
    }
}