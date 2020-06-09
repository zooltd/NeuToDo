using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models;
using NeuToDo.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using Xamarin.Plugin.Calendar.Models;

namespace NeuToDo.ViewModels
{
    public class ToDoCalendarViewModel : ViewModelBase
    {
        /// <remarks>
        /// 注意有坑 Events无法添加多个属于一天的DataTime
        /// 赋值操作无法触发Notify
        /// </remarks>
        public EventCollection Events { get; set; } = new EventCollection();

        public IUpdateCalendar UpdateCalendar { get; private set; } = new UpdateCalendar();

        /// <remarks>
        /// Events赋值操作阻塞，会影响UI渲染线程，造成死锁？ 😟 × 赋值操作无法触发Notify
        /// </remarks>
        // [PreferredConstructor]
        // public ToDoCalendarViewModel()
        // {
        //     Title = "NEU To Do";
        //     Task.Run((async () => { Events = await UpdateCalendar.GetData() ?? new EventCollection(); }));
        //     Console.WriteLine("hello");
        // }

        //TODO SQLite需配置 时间+12:00
        public ToDoCalendarViewModel()
        {
            Title = "NEU To Do";
            Task.Run((async () =>
            {
                var temp = await UpdateCalendar.GetData() ?? new EventCollection();
                foreach (var pair in temp)
                {
                    Events.Add(pair.Key, pair.Value);
                }
            }));
        }

        #region 绑定命令

        /// <summary>
        /// 
        /// </summary>
        private RelayCommand _updateCommand;

        public RelayCommand UpdateCommand =>
            _updateCommand ?? (_updateCommand = new RelayCommand((async () =>
            {
                var temp = await UpdateCalendar.GetData() ?? new EventCollection();
                foreach (var VARIABLE in temp)
                {
                    Events.Add(VARIABLE.Key, VARIABLE.Value);
                }

                Console.WriteLine("debug");
            })));

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