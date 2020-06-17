using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models;
using NeuToDo.Services;
using Xamarin.Forms;

namespace NeuToDo.ViewModels
{
    public class ToDoListViewModel : ViewModelBase
    {
        private readonly IEventModelStorageProvider _eventModelStorageProvider;

        private bool _isLoaded;

        public ToDoListViewModel(IEventModelStorageProvider eventModelStorageProvider)
        {
            _eventModelStorageProvider = eventModelStorageProvider;
            WeeklyAgenda = new ObservableCollection<DailyAgenda>();
        }

        #region 绑定命令

        private RelayCommand _pageAppearingCommand;

        public RelayCommand PageAppearingCommand
            => _pageAppearingCommand ??= new RelayCommand(async () =>
                await PageAppearingCommandFunction());

        #endregion

        private async Task PageAppearingCommandFunction()
        {
            if (_isLoaded) return;
                try
            {
                var neuStorage = await _eventModelStorageProvider.GetEventModelStorage<NeuEvent>();
                var neuEventList = await neuStorage.GetAllAsync();
                var neuEventDict = neuEventList.GroupBy(e => e.Time.Date).ToDictionary(g => g.Key, g => g.ToList());
                foreach (var pair in neuEventDict)
                {
                    WeeklyAgenda.Add(new DailyAgenda(pair.Key, pair.Value));
                }

                //shuffle
                var rand = new Random();
                WeeklyAgenda =
                    new ObservableCollection<DailyAgenda>(WeeklyAgenda.OrderBy(agenda => rand.Next()).ToList().Take(7));
                _isLoaded = true;
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        private ObservableCollection<DailyAgenda> _weeklyAgenda;

        public ObservableCollection<DailyAgenda> WeeklyAgenda
        {
            get => _weeklyAgenda;
            set => Set(nameof(WeeklyAgenda), ref _weeklyAgenda, value);
        }

        public class DailyAgenda
        {
            public string Topic { get; set; }
            public string Duration { get; set; }
            public DateTime Date { get; set; }
            public ObservableCollection<EventModel> EventList { get; set; }
            public string Color { get; set; }
            public int HeightRequest { get; set; }

            public DailyAgenda(DateTime date, List<NeuEvent> eventList)
            {
                Topic = "今日计划";
                Duration = "00:00 - 24:00";
                Date = date;
                EventList = new ObservableCollection<EventModel>(eventList);
                Color = DayColor[date.DayOfWeek];
                HeightRequest = eventList.Count * 35 + 15;
            }
        }

        public static readonly Dictionary<DayOfWeek, string> DayColor = new Dictionary<DayOfWeek, string>
        {
            {DayOfWeek.Sunday, "#B96CBD"},
            {DayOfWeek.Monday, "#49A24D"},
            {DayOfWeek.Tuesday, "#FDA838"},
            {DayOfWeek.Wednesday, "#F75355"},
            {DayOfWeek.Thursday, "#00C6AE"},
            {DayOfWeek.Friday, "#455399"},
            {DayOfWeek.Saturday, "#FFA500"},
        };
    }
}