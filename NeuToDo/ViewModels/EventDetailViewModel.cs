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
using NeuToDo.Utils;
using Xamarin.Forms;

namespace NeuToDo.ViewModels
{
    public class EventDetailViewModel : ViewModelBase
    {
        private readonly IStorageProvider _storageProvider;

        private readonly IPopupNavigationService _popupNavigationService;

        private readonly IAlertService _alertService;

        private readonly IEventDetailNavigationService _eventDetailNavigationService;

        public EventDetailViewModel(IStorageProvider storageProvider,
            IPopupNavigationService popupNavigationService,
            IAlertService alertService,
            IEventDetailNavigationService eventDetailNavigationService)
        {
            _storageProvider = storageProvider;
            _popupNavigationService = popupNavigationService;
            _alertService = alertService;
            _eventDetailNavigationService = eventDetailNavigationService;
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

        private Semester _eventSemester;

        public Semester EventSemester
        {
            get => _eventSemester;
            set => Set(nameof(EventSemester), ref _eventSemester, value);
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
            EventGroupList.Add(new EventGroup {WeekNo = new List<int>()});
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
            _deleteCourse ??= new RelayCommand((async () => await DeleteCourseFunction()));

        public async Task DeleteCourseFunction()
        {
            var res = await _alertService.DisplayAlert("警告", "确定删除有关本课程的所有时间段？", "Yes", "No");
            if (!res) return;
            var neuStorage = await _storageProvider.GetEventModelStorage<NeuEvent>();
            await neuStorage.DeleteAllAsync((e => e.Code == SelectedEvent.Code));
            _storageProvider.OnUpdateData();
            await _eventDetailNavigationService.PopToRootAsync();
        }


        private RelayCommand _editDone;

        public RelayCommand EditDone => _editDone ??= new RelayCommand((async () => await EditDoneFunction()));

        public async Task EditDoneFunction()
        {
            if (string.IsNullOrWhiteSpace(SelectedEvent.Title))
            {
                _alertService.DisplayAlert("操作失败", "课程名称格式错误", "OK");
                return;
            }

            if (EventGroupList.ToList().Exists(x => x.WeekNo == null || !x.WeekNo.Any()))
            {
                _alertService.DisplayAlert("操作失败", "存在未填写的周数", "OK");
                return;
            }

            if (EventGroupList.ToList().Exists(x => x.ClassIndex < 1))
            {
                _alertService.DisplayAlert("操作失败", "存在未填写的节次", "OK");
                return;
            }


            var neuStorage = await _storageProvider.GetEventModelStorage<NeuEvent>();
            var newList = new List<NeuEvent>();
            foreach (var eventGroup in EventGroupList)
            {
                newList.AddRange(eventGroup.WeekNo.Select(weekNo => new NeuEvent
                {
                    Title = SelectedEvent.Title,
                    Code = SelectedEvent.Code,
                    Day = (int) eventGroup.Day,
                    IsDone = false,
                    Detail = eventGroup.Detail,
                    Time = Calculator.CalculateClassTime(eventGroup.Day, weekNo, eventGroup.ClassIndex, Campus.Hunnan,
                        EventSemester.BaseDate), //TODO Campus全局设置
                    Week = weekNo,
                    ClassNo = eventGroup.ClassIndex,
                    SemesterId = EventSemester.SemesterId
                }));
            }

            await neuStorage.DeleteAllAsync((e => e.Code == SelectedEvent.Code));
            await neuStorage.InsertAllAsync(newList);
            _storageProvider.OnUpdateData();
            await _eventDetailNavigationService.PopToRootAsync();
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

        // /// <summary>
        // /// SeekNoSelectPopupPage
        // /// </summary>
        // private RelayCommand _selectWeekNoCancel;
        //
        // public RelayCommand SelectWeekNoCancel =>
        //     _selectWeekNoCancel ??= new RelayCommand((() => { _popupNavigationService.PopAllAsync(); }));
        //
        // private RelayCommand<Grid> _selectWeekNoDone;
        //
        // public RelayCommand<Grid> SelectWeekNoDone =>
        //     _selectWeekNoDone ??= new RelayCommand<Grid>((SelectWeekNoDoneFunction));
        //
        // public void SelectWeekNoDoneFunction(Grid grid)
        // {
        //     // var temp = new List<int>();
        //     // var buttons = grid.LogicalChildren.ToList().ConvertAll(x => (CustomButton) x);
        //     // for (var i = 0; i < buttons.Count; i++)
        //     // {
        //     //     if (buttons[i].IsClicked) temp.Add(i + 1);
        //     // }
        //     //
        //     // SelectEventGroup.WeekNo = new List<int>(temp);
        //     //
        //     // _popupNavigationService.PopAllAsync();
        // }

        #endregion

        public async Task PageAppearingCommandFunction()
        {
            EventGroupList.Clear();
            if (SelectedEvent is NeuEvent neuEvent)
            {
                var neuStorage = await _storageProvider.GetEventModelStorage<NeuEvent>();
                var semesterStorage = await _storageProvider.GetSemesterStorage();
                try
                {
                    EventSemester = await semesterStorage.GetAsync(neuEvent.SemesterId);
                }
                catch (Exception e)
                {
                    EventSemester = new Semester();
                }
                var courses = await neuStorage.GetAllAsync(e => e.Code == SelectedEvent.Code);
                var courseGroupList = courses.GroupBy(c => new {c.Day, c.ClassNo, c.Detail})
                    .OrderBy(p => p.Key.Day);
                foreach (var group in courseGroupList)
                {
                    EventGroupList.Add(
                        new EventGroup
                        {
                            Day = (DayOfWeek) group.Key.Day,
                            Detail = group.Key.Detail,
                            ClassIndex = group.Key.ClassNo,
                            WeekNo = group.ToList().ConvertAll(x => x.Week)
                        });
                }
            }
        }
    }
}