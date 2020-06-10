using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models;
using NeuToDo.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using Xamarin.Forms.Internals;
using Xamarin.Plugin.Calendar.Models;

namespace NeuToDo.ViewModels
{
    public class ToDoCalendarViewModel : ViewModelBase
    {
        /// <remarks>
        /// 注意有坑 Events无法添加多个属于一天的DateTime
        /// 赋值操作无法触发Notify
        /// </remarks>
        public EventCollection Events { get; private set; } = new EventCollection();

        private readonly IEventModelStorage<NeuEventModel> _eventModelStorage;

        // public IUpdateCalendar UpdateCalendar { get; private set; } = new UpdateCalendar();

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

        //TODO  时间+12:00
        public ToDoCalendarViewModel(IEventModelStorageProvider eventModelStorageProvider)
        {
            _eventModelStorage = eventModelStorageProvider.GetNeuEventModelStorage();
        }

        #region 绑定命令

        private RelayCommand _pageAppearingCommand;

        public RelayCommand PageAppearingCommand
            => _pageAppearingCommand ?? (_pageAppearingCommand = new RelayCommand(async () =>
                await PageAppearingCommandFunction()));

        //TODO 需要检查是否已有DateTime
        private async Task PageAppearingCommandFunction()
        {
            try
            {
                var eventList = await _eventModelStorage.GetAllAsync();
                var eventDict = eventList.GroupBy(e => e.Starting.Date).ToDictionary(g => g.Key, g => g.ToList());
                foreach (var pair in eventDict)
                {
                    Events.Add(pair.Key, pair.Value);
                }
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        /// <summary>
        /// test
        /// </summary>
        private RelayCommand _updateCommand;

        public RelayCommand UpdateCommand =>
            _updateCommand ?? (_updateCommand = new RelayCommand(() => { }));

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