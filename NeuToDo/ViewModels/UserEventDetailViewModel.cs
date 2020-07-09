using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models;
using NeuToDo.Services;

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

        private async Task PageAppearingCommandFunction()
        {
            UserEventDetail = new UserEventWrapper(SelectedEvent);
            if (!UserEventDetail.IsRepeat) return;
            var userEvents = await _userStorage.GetAllAsync(x => x.Code == UserEventDetail.Code);
            var userEventGroupList = userEvents.GroupBy(x => new {x.StartDate, x.EndDate, x.TimeOfDay, x.DaySpan})
                .OrderBy(x => x.Key.StartDate).ToList();
            foreach (var group in userEventGroupList)
            {
                UserEventDetail.EventPeriods.Add(new Period
                {
                    StartDate = group.Key.StartDate,
                    EndDate = group.Key.EndDate,
                    TimeOfDay = group.Key.TimeOfDay,
                    DaySpan = group.Key.DaySpan
                });
            }
        }

        private RelayCommand _deleteAll;

        public RelayCommand DeleteAll =>
            _deleteAll ??= new RelayCommand(async () => await DeleteAllFunction());

        private async Task DeleteAllFunction()
        {
            var toDelete = await _dialogService.DisplayAlert("警告", "确定删除有关本事件的所有时间段？", "Yes", "No");
            if (!toDelete) return;
            await _userStorage.DeleteAllAsync(e => e.Code == UserEventDetail.Code);
            _dbStorageProvider.OnUpdateData();
            await _contentPageNavigationService.PopToRootAsync();
        }

        private RelayCommand _addPeriod;

        public RelayCommand AddPeriod =>
            _addPeriod ??= new RelayCommand(() =>
            {
                UserEventDetail.EventPeriods.Add(new Period
                {
                    StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(7),
                    TimeOfDay = TimeSpan.Zero, DaySpan = 1
                });
            });

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

            var userEvents = UserEventDetail.GetUserEvents();

            await _userStorage.InsertAllAsync(userEvents);

            _dbStorageProvider.OnUpdateData();
            await _contentPageNavigationService.PopToRootAsync();
        }

        private RelayCommand<Period> _removePeriod;

        public RelayCommand<Period> RemovePeriod =>
            _removePeriod ??= new RelayCommand<Period>((p) => { UserEventDetail.EventPeriods.Remove(p); });
    }
}