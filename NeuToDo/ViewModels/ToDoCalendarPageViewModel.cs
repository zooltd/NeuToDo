using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using NeuToDo.Models;
using Xamarin.Forms;
using Xamarin.Plugin.Calendar.Models;

namespace NeuToDo.ViewModels
{
    public class ToDoCalendarPageViewModel : BasePageViewModel, INotifyPropertyChanged
    {
        public ICommand TodayCommand => new Command(() =>
        {
            Year = DateTime.Today.Year;
            Month = DateTime.Today.Month;
            SelectedDate = DateTime.Today;
        });

        public ICommand SwipeLeftCommand => new Command(() => { MonthYear = MonthYear.AddMonths(2); });
        public ICommand SwipeRightCommand => new Command(() => { MonthYear = MonthYear.AddMonths(-2); });
        public ICommand SwipeUpCommand => new Command(() => { MonthYear = DateTime.Today; });

        //lack _minimumDate, _maximumDate
        public ToDoCalendarPageViewModel() : base()
        {
            Title = "NEU To Do";
            Events = new EventCollection
            {
                [DateTime.Now.AddDays(-3)] = new List<EventModel>(GenerateEvents(10, "Cool")),
                [DateTime.Now.AddDays(-6)] = new DayEventCollection<EventModel>(Color.Purple, Color.Purple)
                {
                    new EventModel
                    {
                        Name = "Cool event1", Description = "This is Cool event1's description!",
                        Starting = new DateTime()
                    },
                    new EventModel
                    {
                        Name = "Cool event2", Description = "This is Cool event2's description!",
                        Starting = new DateTime()
                    }
                }
            };

            Events.Add(DateTime.Now.AddDays(-2),
                new DayEventCollection<EventModel>(GenerateEvents(10, "Cool"))
                    {EventIndicatorColor = Color.Blue, EventIndicatorSelectedColor = Color.Blue});
            Events.Add(DateTime.Now.AddDays(-4),
                new DayEventCollection<EventModel>(GenerateEvents(10, "Cool"))
                    {EventIndicatorColor = Color.Green, EventIndicatorSelectedColor = Color.White});
            Events.Add(DateTime.Now.AddDays(-5),
                new DayEventCollection<EventModel>(GenerateEvents(10, "Cool"))
                    {EventIndicatorColor = Color.Orange, EventIndicatorSelectedColor = Color.Orange});

            Events.Add(DateTime.Now.AddDays(-1), new List<EventModel>(GenerateEvents(5, "Cool")));

            Events[DateTime.Now] = new List<EventModel>(GenerateEvents(2, "Boring"));
        }

        private IEnumerable<EventModel> GenerateEvents(int count, string name)
        {
            return Enumerable.Range(1, count).Select(x => new EventModel
            {
                Name = $"{name} event{x}",
                Description = $"This is {name} event{x}'s description!"
            });
        }

        private int _month = DateTime.Today.Month;

        public int Month
        {
            get => _month;
            set => SetProperty(ref _month, value);
        }

        private int _year = DateTime.Today.Year;

        public int Year
        {
            get => _year;
            set => SetProperty(ref _year, value);
        }

        private DateTime _selectedDate = DateTime.Today;

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set => SetProperty(ref _selectedDate, value);
        }

        private DateTime _monthYear = DateTime.Today;

        public DateTime MonthYear
        {
            get => _monthYear;
            set => SetProperty(ref _monthYear, value);
        }
    }
}