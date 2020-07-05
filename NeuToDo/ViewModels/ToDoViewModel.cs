﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models;
using NeuToDo.Services;
using NeuToDo.Utils;
using Xamarin.Plugin.Calendar.Models;

namespace NeuToDo.ViewModels
{
    public class ToDoViewModel : ViewModelBase
    {
        public ToDoViewModel(IStorageProvider storageProvider,
            IEventDetailNavigationService eventDetailNavigationService,
            IPreferenceStorageProvider preferenceStorageProvider)
        {
            _storageProvider = storageProvider;
            _eventDetailNavigationService = eventDetailNavigationService;
            _preferenceStorageProvider = preferenceStorageProvider;
            storageProvider.UpdateData += OnGetData;
            _today = DateTime.Today;
            ThisSunday = _today.AddDays(-(int) _today.DayOfWeek); //本周日
            ThisSaturday = ThisSunday.AddDays(6);
        }

        #region 私有变量

        private readonly IStorageProvider _storageProvider;

        private readonly IEventDetailNavigationService _eventDetailNavigationService;

        private readonly IPreferenceStorageProvider _preferenceStorageProvider;

        private Dictionary<DateTime, List<EventModel>> EventDict { get; set; } =
            new Dictionary<DateTime, List<EventModel>>();

        private bool _isLoaded;

        private readonly DateTime _today;

        private List<Semester> _semesters;

        private int _semesterIndex;

        private static readonly Semester EmptySemester = new Semester
            {SchoolYear = "未知的时间裂缝", Season = "请关联教务处", BaseDate = DateTime.MinValue, SemesterId = 0};

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
            var neuStorage = await _storageProvider.GetEventModelStorage<NeuEvent>();
            var moocStorage = await _storageProvider.GetEventModelStorage<MoocEvent>();
            var totalEventList = new List<EventModel>();
            totalEventList.AddRange(await neuStorage.GetAllAsync());
            totalEventList.AddRange(await moocStorage.GetAllAsync());
            EventDict = totalEventList.GroupBy(e => e.Time.Date).ToDictionary(g => g.Key, g => g.ToList());

            var semesterStorage = await _storageProvider.GetSemesterStorage();
            _semesters = await semesterStorage.GetAllOrderedByBaseDateAsync();

            UpdateCalendarData();
            UpdateSemester();
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

        private void UpdateSemester()
        {
            ThisSunday = _today.AddDays(-(int) _today.DayOfWeek); //本周日
            ThisSaturday = ThisSunday.AddDays(6);

            _semesterIndex = 0;
            if (_semesters.Count <= 0)
            {
                Semester = EmptySemester;
                WeekNo = 0;
            }
            else
            {
                Semester = _semesters[_semesterIndex];
                WeekNo = Calculator.CalculateWeekNo(Semester.BaseDate, DateTime.Today);
            }
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

        #endregion

        #region 共有绑定命令

        private RelayCommand<EventModel> _eventTappedCommand;

        public RelayCommand<EventModel> EventTappedCommand => _eventTappedCommand ??= new RelayCommand<EventModel>(
            ((e) => { _eventDetailNavigationService.PushAsync(e); }));

        private RelayCommand _addEventCommand;

        public RelayCommand AddEventCommand => _addEventCommand ??=
            new RelayCommand((() => { _eventDetailNavigationService.PushAsync(new UserEvent()); }));

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
            ThisSunday = ThisSunday.AddDays(-7);
            ThisSaturday = ThisSaturday.AddDays(-7);

            if (WeekNo <= 0)
            {
                if (_semesters.Count > _semesterIndex + 1)
                {
                    Semester = _semesters[++_semesterIndex];
                    WeekNo = Calculator.CalculateWeekNo(Semester.BaseDate, ThisSunday);
                }
                else
                {
                    Semester = EmptySemester;
                    WeekNo--;
                }
            }
            else
            {
                WeekNo--;
            }

            UpdateListData();
        }));

        private RelayCommand _toNextWeek;

        public RelayCommand ToNextWeek => _toNextWeek ??= new RelayCommand((() =>
        {
            ThisSunday = ThisSunday.AddDays(7);
            ThisSaturday = ThisSaturday.AddDays(7);


            if (WeekNo >= 0)
            {
                var maxThisSemesterSunday = _semesterIndex >= 1
                    ? _semesters[_semesterIndex - 1].BaseDate.AddDays(-7)
                    : DateTime.MaxValue;
                if (ThisSunday > maxThisSemesterSunday)
                {
                    Semester = _semesters[--_semesterIndex];
                    WeekNo = 0;
                }
                else
                {
                    WeekNo++;
                }
            }
            else
            {
                WeekNo++;
            }

            UpdateListData();
        }));

        private RelayCommand _navigateToNewUserEventPage;

        public RelayCommand NavigateToNewUserEventPage =>
            _navigateToNewUserEventPage ??= new RelayCommand((() =>
            {
                _eventDetailNavigationService.PushAsync(new UserEvent());
            }));

        private RelayCommand _navigateToNewNeuEventPage;

        public RelayCommand NavigateToNewNeuEventPage =>
            _navigateToNewNeuEventPage ??= new RelayCommand((() =>
            {
                _eventDetailNavigationService.PushAsync(new NeuEvent {SemesterId = _semesters[0].SemesterId});
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

        private Semester _semester;

        public Semester Semester
        {
            get => _semester;
            set => Set(nameof(Semester), ref _semester, value);
        }

        private string _weeklySummary;

        public string WeeklySummary
        {
            get => _weeklySummary;
            set => Set(nameof(WeeklySummary), ref _weeklySummary, value);
        }

        #endregion
    }
}