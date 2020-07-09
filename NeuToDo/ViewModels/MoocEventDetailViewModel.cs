using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models;
using NeuToDo.Services;

namespace NeuToDo.ViewModels
{
    public class MoocEventDetailViewModel : ViewModelBase
    {
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

        #region 绑定命令

        private RelayCommand _pageAppearingCommand;

        public RelayCommand PageAppearingCommand =>
            _pageAppearingCommand ??= new RelayCommand(PageAppearingCommandFunction);

        private void PageAppearingCommandFunction()
        {
            MoocEventDetail = new MoocEventWrapper(SelectedEvent);
        }

        private RelayCommand _deleteAll;

        public RelayCommand DeleteAll =>
            _deleteAll ??= new RelayCommand((async () => await DeleteAllFunction()));

        public async Task DeleteAllFunction()
        {
            var toDelete = await _dialogService.DisplayAlert("警告", "确定删除有关本课程的所有课程/作业/测试信息？", "Yes", "No");
            if (!toDelete) return;
            await _moocStorage.DeleteAllAsync(e => e.Code == MoocEventDetail.Code);
            _dbStorageProvider.OnUpdateData();
            await _contentPageNavigationService.PopToRootAsync();
        }

        private RelayCommand _deleteThisEvent;

        public RelayCommand DeleteThisEvent =>
            _deleteThisEvent ??= new RelayCommand(async () => await DeleteThisEventFunction());

        private async Task DeleteThisEventFunction()
        {
            var toDelete = await _dialogService.DisplayAlert("警告", "确定删除本事件？", "Yes", "No");
            if (!toDelete) return;
            await _moocStorage.DeleteAsync(SelectedEvent);

            _dbStorageProvider.OnUpdateData();
            await _contentPageNavigationService.PopToRootAsync();
        }

        private RelayCommand _saveThisEvent;

        public RelayCommand SaveThisEvent =>
            _saveThisEvent ??= new RelayCommand(async () => await SaveThisEventFunction());

        private async Task SaveThisEventFunction()
        {
            MoocEventDetail.Time = MoocEventDetail.EventDate + MoocEventDetail.EventTime;
            var newMoocEvent = new MoocEvent(MoocEventDetail);
            await _moocStorage.UpdateAsync(newMoocEvent);

            _dbStorageProvider.OnUpdateData();
            await _contentPageNavigationService.PopToRootAsync();
        }

        #endregion


        #region 绑定属性

        private MoocEvent _selectedEvent;

        public MoocEvent SelectedEvent
        {
            get => _selectedEvent;
            set => Set(nameof(SelectedEvent), ref _selectedEvent, value);
        }

        private MoocEventWrapper _moocEventDetail;

        public MoocEventWrapper MoocEventDetail
        {
            get => _moocEventDetail;
            set => Set(nameof(MoocEventDetail), ref _moocEventDetail, value);
        }

        #endregion
    }
}