using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models;
using Xamarin.Plugin.Calendar.Models;

namespace NeuToDo.ViewModels
{
    public class ToDoCalendarViewModel : ViewModelBase
    {
        /// <remarks>
        /// 注意有坑 Events无法添加多个属于一天的DataTime
        /// </remarks>
        public EventCollection Events { get; set; }

        public ToDoCalendarViewModel()
        {
            Title = "NEU To Do";
            Events = new EventCollection
            {
                [DateTime.Today.AddDays(-1)] = new List<EventModel>
                {
                    new EventModel
                    {
                        Name = "event1",
                        Description = "This is event1's description!",
                        Starting = DateTime.Now
                    }
                },
                [DateTime.Today] = new List<EventModel>
                {
                    new EventModel
                    {
                        Name = "event1",
                        Description = "This is event1's description!",
                        Starting = DateTime.Now
                    },
                    new EventModel
                    {
                        Name = "event2",
                        Description = "This is event2's description!",
                        Starting = DateTime.Now
                    }
                }
            };
        }

        #region 绑定命令

        private RelayCommand _todayCommand;

        public RelayCommand TodayCommand =>
            _todayCommand ?? (_todayCommand = new RelayCommand((() => { SelectedDate = DateTime.Today; })));

        private RelayCommand _swipeLeftCommand;

        public RelayCommand SwipeLeftCommand =>
            _swipeLeftCommand ??
            (_swipeLeftCommand = new RelayCommand((() => { MonthYear = MonthYear.AddMonths(2); })));

        private RelayCommand _swipeRightCommand;

        public RelayCommand SwipeRightCommand =>
            _swipeRightCommand ??
            (_swipeRightCommand = new RelayCommand((() => { MonthYear = MonthYear.AddMonths(-2); })));

        private RelayCommand _swipeUpCommand;

        public RelayCommand SwipeUpCommand =>
            _swipeUpCommand ?? (_swipeUpCommand = new RelayCommand((() => { MonthYear = DateTime.Today; })));

        #endregion

        #region 绑定属性

        private string _title = string.Empty;

        public string Title
        {
            get => _title;
            set => Set(nameof(Title), ref _title, value);
        }

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
    }
}