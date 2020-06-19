﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models;
using NeuToDo.Services;
using Xamarin.Essentials;

namespace NeuToDo.ViewModels
{
    public class ToDoListViewModel : ViewModelBase
    {
        public ToDoListViewModel(IEventModelStorageProvider eventModelStorageProvider,
            ILoginAndFetchDataService loginAndFetchDataService,
            IEventDetailNavigationService eventDetailNavigationService)
        {
            _eventModelStorageProvider = eventModelStorageProvider;
            _eventDetailNavigationService = eventDetailNavigationService;
            loginAndFetchDataService.GetData += OnGetData;
            WeeklyAgenda = new ObservableCollection<DailyAgenda>();
            ThisWeekSunday = _today.AddDays(-(int) _today.DayOfWeek); //本周日
            ThisWeekSaturday = ThisWeekSunday.AddDays(6);
        }

        #region List私有变量

        private readonly IEventModelStorageProvider _eventModelStorageProvider;

        private readonly IEventDetailNavigationService _eventDetailNavigationService;

        private bool _isLoaded;

        private DateTime _today = DateTime.Today;

        private Dictionary<DateTime, List<EventModel>> EventDict { get; set; } =
            new Dictionary<DateTime, List<EventModel>>();

        #endregion

        #region 绑定命令

        private RelayCommand<EventModel> _eventTappedCommand;

        public RelayCommand<EventModel> EventTappedCommand => _eventTappedCommand ??= new RelayCommand<EventModel>(
            ((e) =>
            {
                Console.WriteLine(e.ToString());
                _eventDetailNavigationService.PushAsync(e);
            }));

        private RelayCommand _pageAppearingCommand;

        public RelayCommand PageAppearingCommand => _pageAppearingCommand ??=
            new RelayCommand(async () => await PageAppearingCommandFunction());

        private RelayCommand _toLastWeek;

        public RelayCommand ToLastWeek => _toLastWeek ??= new RelayCommand((() =>
        {
            ThisWeekSaturday = ThisWeekSaturday.AddDays(-7);
            ThisWeekSunday = ThisWeekSunday.AddDays(-7);
            WeekNo = WeekNo <= 1 ? 0 : WeekNo - 1;
            GetWeeklyAgenda();
        }));

        private RelayCommand _toNextWeek;

        public RelayCommand ToNextWeek => _toNextWeek ??= new RelayCommand((() =>
        {
            ThisWeekSaturday = ThisWeekSaturday.AddDays(7);
            ThisWeekSunday = ThisWeekSunday.AddDays(7);
            WeekNo++;
            GetWeeklyAgenda();
        }));

        #endregion

        #region 绑定属性

        private DateTime _thisWeekSunday;

        public DateTime ThisWeekSunday
        {
            get => _thisWeekSunday;
            set => Set(nameof(ThisWeekSunday), ref _thisWeekSunday, value);
        }

        private DateTime _thisWeekSaturday;

        public DateTime ThisWeekSaturday
        {
            get => _thisWeekSaturday;
            set => Set(nameof(ThisWeekSaturday), ref _thisWeekSaturday, value);
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

        private ObservableCollection<DailyAgenda> _weeklyAgenda;

        public ObservableCollection<DailyAgenda> WeeklyAgenda
        {
            get => _weeklyAgenda;
            set => Set(nameof(WeeklyAgenda), ref _weeklyAgenda, value);
        }

        #endregion


        private async Task PageAppearingCommandFunction()
        {
            if (_isLoaded) return;
            await LoadData();
            _isLoaded = true;
        }

        private async void OnGetData(object sender, EventArgs e)
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            try
            {
                var lastUpdateWeekNo = Preferences.Get("weekNo", 0);
                var lastUpdateDate = Preferences.Get("updateDate", DateTime.Today);
                var daySpan = (_today - lastUpdateDate).Days - (int) _today.DayOfWeek + (int) lastUpdateDate.DayOfWeek;
                var weekSpan = daySpan / 7;
                WeekNo = lastUpdateWeekNo + weekSpan; //TODO 更好的方案？

                var neuStorage = await _eventModelStorageProvider.GetEventModelStorage<NeuEvent>();
                var moocStorage = await _eventModelStorageProvider.GetEventModelStorage<MoocEvent>();
                var totalEventList = new List<EventModel>();
                totalEventList.AddRange(await neuStorage.GetAllAsync());
                totalEventList.AddRange(await moocStorage.GetAllAsync());
                EventDict = totalEventList.GroupBy(e => e.Time.Date).ToDictionary(g => g.Key, g => g.ToList());


                GetWeeklyAgenda();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void GetWeeklyAgenda()
        {
            WeeklyAgenda.Clear();
            var cnt = 0;
            for (var i = 0; i < 7; i++)
            {
                var currDay = ThisWeekSunday.AddDays(i);
                WeeklyAgenda.Add(EventDict.ContainsKey(currDay)
                    ? new DailyAgenda(currDay, EventDict[currDay])
                    : new DailyAgenda(currDay, new List<EventModel>()));
                cnt += WeeklyAgenda[i].EventList.Count;
            }

            WeeklySummary = $"你本周有{cnt}个ToDo事项, 所谓债多不压身";
        }
    }
}