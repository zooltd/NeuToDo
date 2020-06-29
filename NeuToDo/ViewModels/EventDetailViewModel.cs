using System;
using System.Collections;
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
        private readonly IEventModelStorageProvider _eventStorage;

        public EventDetailViewModel(IEventModelStorageProvider eventModelStorageProvider)
        {
            _eventStorage = eventModelStorageProvider;
        }

        #region 绑定属性

        /// <summary>
        /// itemSource of Day picker
        /// </summary>
        private List<DayOfWeek> _dayItems;

        public List<DayOfWeek> DayItems => _dayItems ??= Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().ToList();

        private EventModel _selectedEvent;

        public EventModel SelectedEvent
        {
            get => _selectedEvent;
            set => Set(nameof(SelectedEvent), ref _selectedEvent, value);
        }

        private ObservableCollection<EventGroup> _eventGroupList;

        public ObservableCollection<EventGroup> EventGroupList
        {
            get => _eventGroupList ??= new ObservableCollection<EventGroup>();
            set => Set(nameof(EventGroupList), ref _eventGroupList, value);
        }

        #endregion

        #region 绑定命令

        private RelayCommand _pageAppearingCommand;

        public RelayCommand PageAppearingCommand =>
            _pageAppearingCommand ??= new RelayCommand(async () =>
                await PageAppearingCommandFunction()
            );

        private RelayCommand _addPeriod;

        public RelayCommand AddPeriod => _addPeriod ??= new RelayCommand((() => { }));

        private RelayCommand<EventGroup> _removePeriod;

        public RelayCommand<EventGroup> RemovePeriod => _removePeriod ??= new RelayCommand<EventGroup>(((g) =>
        {
            EventGroupList.Remove(g);
        }));

        private RelayCommand _editComplete;

        public RelayCommand EditComplete => _editComplete ??= new RelayCommand((() => { }));

        #endregion

        private async Task PageAppearingCommandFunction()
        {
            EventGroupList.Clear();
            if (SelectedEvent.GetType().Name == nameof(NeuEvent))
            {
                var neuStorage = await _eventStorage.GetEventModelStorage<NeuEvent>();
                var courses = await neuStorage.GetAllAsync(SelectedEvent.Code);
                var courseGroupList = courses.GroupBy(c => new {c.Day, c.Detail})
                    .OrderBy(p => p.Key.Day);
                foreach (var group in courseGroupList)
                {
                    EventGroupList.Add(
                        new EventGroup
                        {
                            Day = (DayOfWeek) group.Key.Day,
                            Detail = group.Key.Detail,
                            WeekNo = string.Join(",", group.ToList().ConvertAll(x => x.Week))
                        });
                }
            }
        }
    }

    public class EventGroup
    {
        public string Detail { get; set; }
        public DayOfWeek Day { get; set; }
        public string WeekNo { get; set; }
    }
}