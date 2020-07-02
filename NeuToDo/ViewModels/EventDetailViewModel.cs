﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Components;
using NeuToDo.Models;
using NeuToDo.Services;
using Xamarin.Forms;

namespace NeuToDo.ViewModels
{
    public class EventDetailViewModel : ViewModelBase
    {
        private readonly IEventModelStorageProvider _eventStorage;
        private readonly IPopupNavigationService _popupNavigationService;

        public EventDetailViewModel(IEventModelStorageProvider eventModelStorageProvider,
            IPopupNavigationService popupNavigationService)
        {
            _eventStorage = eventModelStorageProvider;
            _popupNavigationService = popupNavigationService;
        }

        #region 绑定属性

        /// <summary>
        /// itemSource of Day picker
        /// </summary>
        private Array _dayItems;

        public Array DayItems => _dayItems ??= Enum.GetValues(typeof(DayOfWeek));

        private List<int> _classIndexItems;

        public List<int> ClassIndexItems => _classIndexItems ??= Enumerable.Range(1, 12).ToList();

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

        private ObservableCollection<int> _weekIndexInSelectionPage;

        public ObservableCollection<int> WeekIndexInSelectionPage
        {
            get => _weekIndexInSelectionPage;
            set => Set(nameof(WeekIndexInSelectionPage), ref _weekIndexInSelectionPage, value);
        }

        public EventGroup SelectEventGroup { get; set; }

        #endregion

        #region 绑定命令

        private RelayCommand _pageAppearingCommand;

        public RelayCommand PageAppearingCommand =>
            _pageAppearingCommand ??= new RelayCommand(async () =>
                await PageAppearingCommandFunction()
            );

        /// <summary>
        /// Neu
        /// </summary>
        private RelayCommand _addPeriod;

        public RelayCommand AddPeriod => _addPeriod ??= new RelayCommand((() =>
        {
            EventGroupList.Add(new EventGroup());
        }));

        /// <summary>
        /// Neu
        /// </summary>
        private RelayCommand<EventGroup> _removePeriod;

        public RelayCommand<EventGroup> RemovePeriod =>
            _removePeriod ??= new RelayCommand<EventGroup>(((g) => { EventGroupList.Remove(g); }));

        /// <summary>
        /// Neu
        /// </summary>
        private RelayCommand _deleteCourse;

        public RelayCommand DeleteCourse =>
            _deleteCourse ??= new RelayCommand((async () =>
            {
                var neuStorage = await _eventStorage.GetEventModelStorage<NeuEvent>();
                await neuStorage.DeleteAllAsync((e => e.Code == SelectedEvent.Code));
                _eventStorage.OnUpdateData();
                // TODO navigate
            }));


        private RelayCommand _editDone;

        public RelayCommand EditDone => _editDone ??= new RelayCommand((async () => await EditDoneFunction()));

        private async Task EditDoneFunction()
        {
            var neuStorage = await _eventStorage.GetEventModelStorage<NeuEvent>();
            await neuStorage.DeleteAllAsync((e => e.Code == SelectedEvent.Code));
            foreach (var eventGroup in EventGroupList)
            {
                foreach (var weekNo in eventGroup.WeekNo)
                {
                    
                }
            }
        }

        /// <summary>
        /// Neu
        /// </summary>
        private RelayCommand<EventGroup> _weekNoSelect;

        public RelayCommand<EventGroup> WeekNoSelect =>
            _weekNoSelect ??= new RelayCommand<EventGroup>((group) =>
            {
                SelectEventGroup = group;
                _popupNavigationService.PushAsync(PopupPageNavigationConstants.WeekNoSelectPopupPage);
            });

        /// <summary>
        /// SeekNoSelectPopupPage
        /// </summary>
        private RelayCommand _selectWeekNoCancel;

        public RelayCommand SelectWeekNoCancel =>
            _selectWeekNoCancel ??= new RelayCommand((() => { _popupNavigationService.PopAllAsync(); }));

        private RelayCommand<CollectionView> _selectWeekNoDone;

        public RelayCommand<CollectionView> SelectWeekNoDone =>
            _selectWeekNoDone ??= new RelayCommand<CollectionView>(((collection) =>
            {
                var temp = new List<int>();
                var buttons = collection.LogicalChildren.ToList().ConvertAll(x => (CustomButton) x);
                for (var i = 0; i < buttons.Count; i++)
                {
                    if (buttons[i].IsClicked) temp.Add(i + 1);
                }

                SelectEventGroup.WeekNo = new List<int>(temp);

                _popupNavigationService.PopAllAsync();
            }));

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
                    var detailSplit = group.Key.Detail.Split(',', '-');
                    int.TryParse(detailSplit[0], out var index);
                    EventGroupList.Add(
                        new EventGroup
                        {
                            Day = (DayOfWeek) group.Key.Day,
                            Detail = group.Key.Detail,
                            ClassIndex = index,
                            WeekNo = group.ToList().ConvertAll(x => x.Week)
                        });
                }
            }
        }
    }
}