using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models;
using NeuToDo.Services;
using Xamarin.Forms.Internals;

namespace NeuToDo.ViewModels
{
    public class UserEventDetailViewModel : ViewModelBase
    {
        private readonly IDbStorageProvider _dbStorageProvider;
        private readonly IEventModelStorage<UserEvent> _userStorage;
        private readonly IDialogService _dialogService;
        private readonly IContentPageNavigationService _contentPageNavigationService;

        public UserEventDetailViewModel(IDbStorageProvider dbStorageProvider,
            IDialogService dialogService,
            IContentPageNavigationService contentPageNavigationService)
        {
            _dbStorageProvider = dbStorageProvider;
            _userStorage = dbStorageProvider.GetEventModelStorage<UserEvent>();
            _dialogService = dialogService;
            _contentPageNavigationService = contentPageNavigationService;
        }


        public async Task PageAppearingCommandFunction()
        {
            UserEventDetail = new UserEventWrapper(SelectedEvent);
            if (!UserEventDetail.IsRepeat) return;
            var userEvents = await _userStorage.GetAllAsync(x => x.Code == UserEventDetail.Code && !x.IsDeleted);
            var userEventGroupList = userEvents
                .GroupBy(x => new {x.PeriodId, x.StartDate, x.EndDate, x.TimeOfDay, x.DaySpan})
                .OrderBy(x => x.Key.StartDate).ToList();
            foreach (var group in userEventGroupList)
            {
                UserEventDetail.EventPeriods.Add(new UserEventPeriod
                {
                    PeriodId = group.Key.PeriodId,
                    StartDate = group.Key.StartDate,
                    EndDate = group.Key.EndDate,
                    TimeOfDay = group.Key.TimeOfDay,
                    DaySpan = group.Key.DaySpan
                });
            }
        }

        #region 绑定命令

        private RelayCommand _deleteAll;

        public RelayCommand DeleteAll =>
            _deleteAll ??= new RelayCommand(async () => await DeleteAllFunction());

        public async Task DeleteAllFunction()
        {
            var toDelete = await _dialogService.DisplayAlert("警告", "确定删除有关本事件的所有时间段？", "Yes", "No");
            if (!toDelete) return;
            var oldList = await _userStorage.GetAllAsync(x => x.Code == UserEventDetail.Code && !x.IsDeleted);

            await MarkDeleted(oldList);

            _dbStorageProvider.OnUpdateData();
            await _contentPageNavigationService.PopToRootAsync();
        }

        private async Task MarkDeleted(IList<UserEvent> oldList)
        {
            oldList.ForEach(x =>
            {
                x.Title = null;
                x.Detail = null;
                x.Code = null;
                x.IsDeleted = true;
                x.LastModified = DateTime.Now;
            });
            await _userStorage.UpdateAllAsync(oldList);
        }

        private RelayCommand _addPeriod;

        public RelayCommand AddPeriod =>
            _addPeriod ??= new RelayCommand(AddPeriodFunction);

        public void AddPeriodFunction()
        {
            var maxPeriodId = UserEventDetail.EventPeriods.Max(x => x.PeriodId);
            UserEventDetail.EventPeriods.Add(new UserEventPeriod
            {
                StartDate = DateTime.Today, EndDate = DateTime.Today.AddMonths(1),
                TimeOfDay = TimeSpan.Zero, DaySpan = 1, PeriodId = maxPeriodId + 1
            });
        }

        private RelayCommand _editDone;

        public RelayCommand EditDone =>
            _editDone ??= new RelayCommand(async () => await EditDoneFunction());

        public async Task EditDoneFunction()
        {
            if (string.IsNullOrWhiteSpace(UserEventDetail.Title))
            {
                _dialogService.DisplayAlert("警告", "事件标题不能为空", "OK");
                return;
            }

            var newList = UserEventDetail.GetUserEvents();


            if (newList.Count == 0)
            {
                _dialogService.DisplayAlert("警告", "请至少添加一个时间段", "OK");
                return;
            }

            newList.ForEach(x =>
            {
                x.Uuid = Guid.NewGuid().ToString();
                x.LastModified = DateTime.Now;
            });


            var oldList = await _userStorage.GetAllAsync(x => x.Code == UserEventDetail.Code && !x.IsDeleted);
            oldList.ForEach(x =>
            {
                x.IsDeleted = true;
                x.LastModified = DateTime.Now;
            });
            await _userStorage.UpdateAllAsync(oldList);
            await _userStorage.InsertAllAsync(newList);

            _dbStorageProvider.OnUpdateData();
            await _contentPageNavigationService.PopToRootAsync();
        }

        private RelayCommand<UserEventPeriod> _removePeriod;

        public RelayCommand<UserEventPeriod> RemovePeriod =>
            _removePeriod ??= new RelayCommand<UserEventPeriod>(RemovePeriodFunction);

        public void RemovePeriodFunction(UserEventPeriod p)
        {
            UserEventDetail.EventPeriods.Remove(p);
        }

        // private RelayCommand _toggleCommand;
        //
        // public RelayCommand ToggleCommand =>
        //     _toggleCommand ??= new RelayCommand(ToggleCommandFunction);
        //
        // private void ToggleCommandFunction()
        // {
        //     if (!UserEventDetail.IsRepeat)
        //         UserEventDetail.EventPeriods.Clear();
        // }

        #endregion

        #region 绑定属性

        public List<int> DaySpans => Enumerable.Range(1, 31).ToList();

        private UserEvent _selectedEvent;

        public UserEvent SelectedEvent
        {
            get => _selectedEvent;
            set => Set(nameof(SelectedEvent), ref _selectedEvent, value);
        }

        private UserEventWrapper _userEventDetail;

        public UserEventWrapper UserEventDetail
        {
            get => _userEventDetail;
            set => Set(nameof(UserEventDetail), ref _userEventDetail, value);
        }

        private RelayCommand _pageAppearingCommand;

        public RelayCommand PageAppearingCommand =>
            _pageAppearingCommand ??= new RelayCommand(async () => await PageAppearingCommandFunction());

        #endregion
    }
}