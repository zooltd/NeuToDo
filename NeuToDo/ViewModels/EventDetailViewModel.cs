using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models;
using NeuToDo.Services;

namespace NeuToDo.ViewModels
{
    public class EventDetailViewModel : ViewModelBase
    {
        private IEventModelStorageProvider _eventStorage;

        public EventDetailViewModel(IEventModelStorageProvider eventModelStorageProvider)
        {
            _eventStorage = eventModelStorageProvider;
        }

        #region 绑定属性

        /// <summary>
        /// itemSource of Day picker
        /// </summary>
        // public static List<DayOfWeek> DayItems = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList();;
        private EventModel _selectedEvent;

        public EventModel SelectedEvent
        {
            get => _selectedEvent;
            set => Set(nameof(SelectedEvent), ref _selectedEvent, value);
        }

        private ObservableCollection<TimeTable> _eventPeriod;

        public ObservableCollection<TimeTable> EventPeriod
        {
            get => _eventPeriod;
            set => Set(nameof(EventPeriod), ref _eventPeriod, value);
        }

        #endregion

        #region 绑定命令

        private RelayCommand _pageAppearingCommand;

        public RelayCommand PageAppearingCommand =>
            _pageAppearingCommand ??= new RelayCommand(async () =>
                await PageAppearingCommandFunction()
            );

        #endregion

        private async Task PageAppearingCommandFunction()
        {
            EventPeriod = new ObservableCollection<TimeTable>();
            if (SelectedEvent.GetType().Name == nameof(NeuEvent))
            {
                var neuStorage = await _eventStorage.GetEventModelStorage<NeuEvent>();
                var courses = await neuStorage.GetAllAsync(SelectedEvent.Code);
                var courseDict = courses.GroupBy(c => c.Day)
                    .ToDictionary(g => g.Key, g => g.ToList().ConvertAll(x => x.Week));
                foreach (var pair in courseDict)
                {
                    EventPeriod.Add(new TimeTable((DayOfWeek) pair.Key, string.Join(",", pair.Value)));
                }
            }
        }
    }

    public class TimeTable
    {
        public DayOfWeek Day { get; set; }

        public string WeekNo { get; set; }

        //TODO 
        public List<DayOfWeek> DayItems { get; set; }

        public TimeTable(DayOfWeek day, string weekNo)
        {
            Day = day;
            WeekNo = weekNo;
            DayItems = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList();
        }
    };
}