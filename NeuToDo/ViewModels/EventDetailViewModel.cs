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

        private RelayCommand _addPeriod;

        public RelayCommand AddPeriod => _addPeriod ??= new RelayCommand((() =>
        {
            EventGroupList.Add(new EventGroup());
        }));

        private RelayCommand<EventGroup> _removePeriod;

        public RelayCommand<EventGroup> RemovePeriod => _removePeriod ??= new RelayCommand<EventGroup>(((g) =>
        {
            EventGroupList.Remove(g);
        }));

        private RelayCommand _editComplete;

        public RelayCommand EditComplete => _editComplete ??= new RelayCommand((() => { }));

        private RelayCommand<EventGroup> _weekNoSelect;

        public RelayCommand<EventGroup> WeekNoSelect =>
            _weekNoSelect ??= new RelayCommand<EventGroup>((group) =>
            {
                SelectEventGroup = group;
                _popupNavigationService.PushAsync(PopupPageNavigationConstants.WeekNoSelectPopupPage);
            });

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