﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models;
using NeuToDo.Services;
using Xamarin.Plugin.Calendar.Models;

namespace NeuToDo.ViewModels
{
    public class ToDoViewModel : ViewModelBase
    {
        public ToDoViewModel(IEventModelStorageProvider eventModelStorageProvider,
            IEventDetailNavigationService eventDetailNavigationService,
            ILoginAndFetchDataService loginAndFetchDataService,
            IPreferenceStorageProvider preferenceStorageProvider)
        {
            _eventModelStorageProvider = eventModelStorageProvider;
            _eventDetailNavigationService = eventDetailNavigationService;
            _preferenceStorageProvider = preferenceStorageProvider;
            loginAndFetchDataService.GetData += OnGetData;
            _today = DateTime.Today;
            ThisSunday = _today.AddDays(-(int) _today.DayOfWeek); //本周日
            ThisSaturday = ThisSunday.AddDays(6);
        }

        #region 私有变量

        private readonly IEventModelStorageProvider _eventModelStorageProvider;

        private readonly IEventDetailNavigationService _eventDetailNavigationService;

        private readonly IPreferenceStorageProvider _preferenceStorageProvider;

        private Dictionary<DateTime, List<EventModel>> EventDict { get; set; } =
            new Dictionary<DateTime, List<EventModel>>();

        private bool _isLoaded;

        private DateTime _today;

        #endregion

        #region 共用私有函数

        private async void OnGetData(object sender, EventArgs e)
        {
            await LoadData();
        }

        /// <summary>
        /// 从DB中读取数据到EventDict, 并更新两个视图的绑定属性
        /// </summary>
        /// <returns></returns>
        private async Task LoadData()
        {
            var neuStorage = await _eventModelStorageProvider.GetEventModelStorage<NeuEvent>();
            var moocStorage = await _eventModelStorageProvider.GetEventModelStorage<MoocEvent>();
            var totalEventList = new List<EventModel>();
            totalEventList.AddRange(await neuStorage.GetAllAsync());
            totalEventList.AddRange(await moocStorage.GetAllAsync());
            EventDict = totalEventList.GroupBy(e => e.Time.Date).ToDictionary(g => g.Key, g => g.ToList());
            UpdateCalendarData();
            UpdateTeachingWeekNo();
            UpdateListData();
        }

        private void UpdateCalendarData()
        {
            EventCollection.Clear();
            foreach (var pair in EventDict)
            {
                EventCollection.Add(pair.Key, pair.Value);
            }
        }

        private void UpdateTeachingWeekNo()
        {
            var lastUpdateWeekNo = _preferenceStorageProvider.Get("weekNo", 0);
            var lastUpdateDate = _preferenceStorageProvider.Get("updateDate", DateTime.Today);
            var daySpan = (_today - lastUpdateDate).Days - (int) _today.DayOfWeek + (int) lastUpdateDate.DayOfWeek;
            var weekSpan = daySpan / 7;
            WeekNo = lastUpdateWeekNo + weekSpan; //TODO 更好的方案？
        }

        private void UpdateListData()
        {
            var cnt = 0;
            WeeklyAgenda.Clear();
            for (var i = 0; i < 7; i++)
            {
                var currDay = ThisSunday.AddDays(i);
                WeeklyAgenda.Add(EventDict.ContainsKey(currDay)
                    ? new DailyAgenda(currDay, EventDict[currDay])
                    : new DailyAgenda(currDay, new List<EventModel>()));
                cnt += WeeklyAgenda[i].EventList.Count;
            }

            WeeklySummary = $"你本周有{cnt}个ToDo事项, 所谓债多不压身";
        }

        private async Task PageAppearingCommandFunction()
        {
            if (_isLoaded) return;
            await LoadData();
            _isLoaded = true;
        }

        #endregion

        #region 共有绑定属性

        private EventModel _selectedEvent;

        public EventModel SelectedEvent
        {
            get => _selectedEvent;
            set => Set(nameof(SelectedEvent), ref _selectedEvent, value);
        }

        #endregion

        #region 共有绑定命令

        private RelayCommand<EventModel> _eventTappedCommand;

        public RelayCommand<EventModel> EventTappedCommand => _eventTappedCommand ??= new RelayCommand<EventModel>(
            ((e) =>
            {
                SelectedEvent = e;
                IsRepeat = false || e.GetType() == typeof(NeuEvent);
                _eventDetailNavigationService.PushAsync();
            }));

        private RelayCommand _addEventCommand;

        public RelayCommand AddEventCommand => _addEventCommand ??=
            new RelayCommand((() =>
            {
                SelectedEvent = new UserEvent();
                _eventDetailNavigationService.PushAsync();
            }));

        private RelayCommand _pageAppearingCommand;

        public RelayCommand PageAppearingCommand => _pageAppearingCommand ??=
            new RelayCommand(async () => await PageAppearingCommandFunction());

        #endregion

        #region Calendar绑定命令

        private RelayCommand _monthYearTappedCommand;

        public RelayCommand MonthYearTappedCommand =>
            _monthYearTappedCommand ??= new RelayCommand((() => { SelectedDate = DateTime.Today; }));

        #endregion

        #region List绑定命令

        private RelayCommand _toLastWeek;

        public RelayCommand ToLastWeek => _toLastWeek ??= new RelayCommand((() =>
        {
            WeekNo = WeekNo <= 1 ? 0 : WeekNo - 1; //TODO
            ThisSaturday = ThisSaturday.AddDays(-7);
            ThisSunday = ThisSunday.AddDays(-7);
            UpdateListData();
        }));

        private RelayCommand _toNextWeek;

        public RelayCommand ToNextWeek => _toNextWeek ??= new RelayCommand((() =>
        {
            WeekNo++;
            ThisSaturday = ThisSaturday.AddDays(7);
            ThisSunday = ThisSunday.AddDays(7);
            UpdateListData();
        }));

        #endregion


        #region Calendar绑定属性

        public EventCollection EventCollection { get; } = new EventCollection();

        private DateTime _selectedDate = DateTime.Today;

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set => Set(nameof(SelectedDate), ref _selectedDate, value);
        }

        private DateTime _monthYear = DateTime.Today;

        public DateTime MonthYear
        {
            get => _monthYear;
            set => Set(nameof(MonthYear), ref _monthYear, value);
        }

        #endregion

        #region List绑定属性

        public ObservableCollection<DailyAgenda> WeeklyAgenda { get; } = new ObservableCollection<DailyAgenda>();

        private DateTime _thisSunday;

        public DateTime ThisSunday
        {
            get => _thisSunday;
            set => Set(nameof(ThisSunday), ref _thisSunday, value);
        }

        private DateTime _thisSaturday;

        public DateTime ThisSaturday
        {
            get => _thisSaturday;
            set => Set(nameof(ThisSaturday), ref _thisSaturday, value);
        }

        private int _weekNo;

        public int WeekNo
        {
            get => _weekNo;
            set => Set(nameof(WeekNo), ref _weekNo, value);
        }

        private string _weeklySummary;

        public string WeeklySummary
        {
            get => _weeklySummary;
            set => Set(nameof(WeeklySummary), ref _weeklySummary, value);
        }

        #endregion

        #region Detail绑定属性

        private bool _isRepeat;

        public bool IsRepeat
        {
            get => _isRepeat;
            set => Set(nameof(IsRepeat), ref _isRepeat, value);
        }

        #endregion
    }
}