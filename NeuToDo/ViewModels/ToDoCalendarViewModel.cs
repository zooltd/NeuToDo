using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models;
using NeuToDo.Services;
using Xamarin.Essentials;
using Xamarin.Plugin.Calendar.Models;

namespace NeuToDo.ViewModels
{
    public class ToDoCalendarViewModel : ViewModelBase {
        // private IxxxService _xxxService;
        //
        // public ToDoCalendarViewModel(IxxxService xxxService) {
        //     _xxxService = xxxService;
        //     _xxxService.GotData += (sender, args) => {
        //         _xxxService.GetData();
        //     };
        // }
        
        
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
                // [DateTime.Today] = new List<EventModel>
                // {
                //     new EventModel
                //     {
                //         Name = "event1",
                //         Description = "This is event1's description!",
                //         Starting = DateTime.Now
                //     },
                //     new EventModel
                //     {
                //         Name = "event2",
                //         Description = "This is event2's description!",
                //         Starting = DateTime.Now
                //     }
                // }
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

        public void UpdateEvents()
        {
            var date = DateTime.Today;
            var currentDay = date.DayOfWeek;
            var currentWeek = NeuSyllabusGetter.TeachingTime.TeachingWeek;
            var syllabus = NeuSyllabusGetter.Syllabus;
            foreach (var course in syllabus.Values)
            {
                var courseSchedule = course.Schedule;
                foreach (var day in courseSchedule.Keys)
                {
                    var weekIndexes = FindAllIndexes(courseSchedule[day].Weeks, '1');
                    var daySpan = (int) day - (int) currentDay;
                    foreach (var weekIndex in weekIndexes)
                    {
                        //weekIndex: 第weekIndex周
                        var offsetWeek = weekIndex - currentWeek;
                        var offsetDay = offsetWeek * 7 + daySpan;
                        var d = DateTime.Today.AddDays(offsetDay);
                        var startingTime = GetStartingTime(d, courseSchedule[day].ClassTime.ToString());
                        if (Events.ContainsKey(d))
                        {
                            Events[d] = new ArrayList(Events[d])
                            {
                                new EventModel()
                                {
                                    Name = course.CourseName,
                                    Description = courseSchedule[day].ClassTime.ToString() + "," + course.RoomName +
                                                  "," + course.TeacherList[0].TeacherName,
                                    Starting = startingTime
                                }
                            };
                        }
                        else
                        {
                            Events.Add(d, new List<EventModel>()
                            {
                                new EventModel()
                                {
                                    Name = course.CourseName,
                                    Description = courseSchedule[day].ClassTime.ToString() + " " + course.RoomName +
                                                  "," + course.TeacherList[0].TeacherName,
                                    Starting = startingTime
                                }
                            });
                        }
                    }
                }
            }
        }

        private static IEnumerable<int> FindAllIndexes(string source, char key)
        {
            var i = source.IndexOf(key);
            var indexes = new List<int>();
            while (i != -1)
            {
                indexes.Add(i);
                i = source.IndexOf(key, i + 1);
            }

            return indexes;
        }

        private static DateTime GetStartingTime(DateTime date, string classTime)
        {
            if (classTime.Contains("First"))
                return new DateTime(date.Year, date.Month, date.Day, 8, 30, 0);

            if (classTime.Contains("Third"))
                return new DateTime(date.Year, date.Month, date.Day, 10, 40, 0);

            if (classTime.Contains("Fifth"))
                return new DateTime(date.Year, date.Month, date.Day, 14, 0, 0);

            if (classTime.Contains("Seventh"))
                return new DateTime(date.Year, date.Month, date.Day, 16, 10, 0);

            if (classTime.Contains("Ninth"))
                return new DateTime(date.Year, date.Month, date.Day, 18, 30, 0);

            if (classTime.Contains("Eleventh"))
                return new DateTime(date.Year, date.Month, date.Day, 20, 30, 0);

            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
        }
    }
}