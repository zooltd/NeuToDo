﻿using System;
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

        private ObservableCollection<TimeTable> _eventPeriod;

        public ObservableCollection<TimeTable> EventPeriod
        {
            get => _eventPeriod ??= new ObservableCollection<TimeTable>();
            set => Set(nameof(EventPeriod), ref _eventPeriod, value);
        }

        #endregion

        #region 绑定命令

        private RelayCommand _pageAppearingCommand;

        public RelayCommand PageAppearingCommand =>
            _pageAppearingCommand ??= new RelayCommand(async () =>
                await PageAppearingCommandFunction()
            );

        private RelayCommand _addPeriod;

        public RelayCommand AddPeriod => _addPeriod ??= new RelayCommand((() => { EventPeriod.Add(new TimeTable()); }));

        private RelayCommand<TimeTable> _removePeriod;

        public RelayCommand<TimeTable> RemovePeriod => _removePeriod ??= new RelayCommand<TimeTable>(((t) =>
        {
            EventPeriod.Remove(t);
        }));

        private RelayCommand _editComplete;

        public RelayCommand EditComplete => _editComplete ??= new RelayCommand((() => { }));

        #endregion

        private async Task PageAppearingCommandFunction()
        {
            EventPeriod.Clear();
            if (SelectedEvent.GetType().Name == nameof(NeuEvent))
            {
                var neuStorage = await _eventStorage.GetEventModelStorage<NeuEvent>();
                var courses = await neuStorage.GetAllAsync(SelectedEvent.Code);
                var courseDict = courses.GroupBy(c => c.Day)
                    .ToDictionary(g => g.Key, g => g.ToList().ConvertAll(x => x.Week));
                foreach (var pair in courseDict)
                {
                    EventPeriod.Add(new TimeTable {Day = (DayOfWeek) pair.Key, WeekNo = string.Join(",", pair.Value)});
                }
            }
        }
    }

    public class TimeTable
    {
        public DayOfWeek Day { get; set; }

        public string WeekNo { get; set; }
    };
}