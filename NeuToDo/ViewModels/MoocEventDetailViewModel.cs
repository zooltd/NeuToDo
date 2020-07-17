using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models;
using NeuToDo.Services;
using Xamarin.Forms;

namespace NeuToDo.ViewModels
{
    public class MoocEventDetailViewModel : ViewModelBase
    {
        #region 构造函数

        private readonly IEventModelStorage<MoocEvent> _moocStorage;
        private readonly IDbStorageProvider _dbStorageProvider;
        private readonly IDialogService _dialogService;
        private readonly IContentPageNavigationService _contentPageNavigationService;

        public MoocEventDetailViewModel(IDbStorageProvider dbStorageProvider,
            IDialogService dialogService,
            IContentPageNavigationService contentPageNavigationService)
        {
            _moocStorage = dbStorageProvider.GetEventModelStorage<MoocEvent>();
            _dbStorageProvider = dbStorageProvider;
            _dialogService = dialogService;
            _contentPageNavigationService = contentPageNavigationService;
        }

        #endregion


        #region 绑定命令

        /// <summary>
        /// 页面显示命令。
        /// </summary>
        private RelayCommand _pageAppearingCommand;

        /// <summary>
        /// 页面显示命令。
        /// </summary>
        public RelayCommand PageAppearingCommand =>
            _pageAppearingCommand ??= new RelayCommand(PageAppearingCommandFunction);

        public void PageAppearingCommandFunction()
        {
            MoocEventDetail = new MoocEventWrapper(SelectedEvent);
        }

        /// <summary>
        /// 删除所有命令。
        /// </summary>
        private RelayCommand _deleteAll;

        /// <summary>
        /// 删除所有命令。
        /// </summary>
        public RelayCommand DeleteAll =>
            _deleteAll ??= new RelayCommand((async () => await DeleteAllFunction()));

        public async Task DeleteAllFunction()
        {
            var toDelete = await _dialogService.DisplayAlert("警告", "确定删除有关本课程的所有课程/作业/测试信息？", "Yes", "No");
            if (!toDelete) return;
            var oldList = await _moocStorage.GetAllAsync(x => x.Code == MoocEventDetail.Code && !x.IsDeleted);
            oldList.ForEach(x =>
            {
                x.IsDeleted = true;
                x.LastModified = DateTime.Now;
            });
            await _moocStorage.UpdateAllAsync(oldList);
            _dbStorageProvider.OnUpdateData();
            await _contentPageNavigationService.PopToRootAsync();
        }

        /// <summary>
        /// 删除该日程命令。
        /// </summary>
        private RelayCommand _deleteThisEvent;

        /// <summary>
        /// 删除该日程命令。
        /// </summary>
        public RelayCommand DeleteThisEvent =>
            _deleteThisEvent ??= new RelayCommand(async () => await DeleteThisEventFunction());

        public async Task DeleteThisEventFunction()
        {
            var toDelete = await _dialogService.DisplayAlert("警告", "确定删除本事件？", "Yes", "No");
            if (!toDelete) return;
            var newEvent = SelectedEvent;
            newEvent.IsDeleted = true;
            newEvent.LastModified=DateTime.Now;
            await _moocStorage.UpdateAsync(newEvent);

            _dbStorageProvider.OnUpdateData();
            await _contentPageNavigationService.PopToRootAsync();
        }

        /// <summary>
        /// 保存该日程命令。
        /// </summary>
        private RelayCommand _saveThisEvent;

        /// <summary>
        /// 保存该日程命令。
        /// </summary>
        public RelayCommand SaveThisEvent =>
            _saveThisEvent ??= new RelayCommand(async () => await SaveThisEventFunction());

        public async Task SaveThisEventFunction()
        {
            MoocEventDetail.Time = MoocEventDetail.EventDate + MoocEventDetail.EventTime;
            var newEvent = new MoocEvent(MoocEventDetail);
            newEvent.Uuid = Guid.NewGuid().ToString();
            newEvent.IsDeleted = true; 
            newEvent.LastModified=DateTime.Now;
            await _moocStorage.UpdateAsync(newEvent);

            _dbStorageProvider.OnUpdateData();
            await _contentPageNavigationService.PopToRootAsync();
        }

        #endregion


        #region 绑定属性

        /// <summary>
        /// 被选中日程。
        /// </summary>
        private MoocEvent _selectedEvent;

        /// <summary>
        /// 被选中日程。
        /// </summary>
        public MoocEvent SelectedEvent
        {
            get => _selectedEvent;
            set => Set(nameof(SelectedEvent), ref _selectedEvent, value);
        }

        /// <summary>
        /// 慕课日程详情。
        /// </summary>
        private MoocEventWrapper _moocEventDetail;

        /// <summary>
        /// 慕课日程详情。
        /// </summary>
        public MoocEventWrapper MoocEventDetail
        {
            get => _moocEventDetail;
            set => Set(nameof(MoocEventDetail), ref _moocEventDetail, value);
        }

        #endregion
    }
}