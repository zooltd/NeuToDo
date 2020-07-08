using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models;
using NeuToDo.Services;
using NeuToDo.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace NeuToDo.ViewModels
{
    public class EventDetailViewModel : ViewModelBase
    {
        private readonly IEventModelStorage<NeuEvent> _neuStorage;
        private readonly ISemesterStorage _semesterStorage;
        private readonly IDbStorageProvider _dbStorageProvider;
        private readonly IPopupNavigationService _popupNavigationService;
        private readonly IDialogService _dialogService;
        private readonly IContentPageNavigationService _contentPageNavigationService;
        private readonly ICampusStorageService _campusStorageService;

        public EventDetailViewModel(IDbStorageProvider dbStorageProvider,
            IPopupNavigationService popupNavigationService,
            IDialogService dialogService,
            IContentPageNavigationService contentPageNavigationService,
            ICampusStorageService campusStorageService)
        {
            _neuStorage = dbStorageProvider.GetEventModelStorage<NeuEvent>();
            _semesterStorage = dbStorageProvider.GetSemesterStorage();
            _dbStorageProvider = dbStorageProvider;
            _popupNavigationService = popupNavigationService;
            _dialogService = dialogService;
            _contentPageNavigationService = contentPageNavigationService;
            _campusStorageService = campusStorageService;
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
            EventGroupList.Add(new EventGroup { WeekNo = new List<int>() });
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
            var res = await _dialogService.DisplayAlert("警告", "确定删除有关本课程的所有时间段？", "Yes", "No");
            if (!res) return;
            await _neuStorage.DeleteAllAsync((e => e.Code == SelectedEvent.Code));
            _dbStorageProvider.OnUpdateData();
            await _contentPageNavigationService.PopToRootAsync();
        }


        private RelayCommand _editDone;

        public RelayCommand EditDone => _editDone ??= new RelayCommand((async () => await EditDoneFunction()));

        public async Task EditDoneFunction()
        {
            if (string.IsNullOrWhiteSpace(SelectedEvent.Title))
            {
                _dialogService.DisplayAlert("操作失败", "课程名称格式错误", "OK");
                return;
            }

            if (EventGroupList.ToList().Exists(x => x.WeekNo == null || !x.WeekNo.Any()))
            {
                _dialogService.DisplayAlert("操作失败", "存在未填写的周数", "OK");
                return;
            }

            if (EventGroupList.ToList().Exists(x => x.ClassIndex < 1))
            {
                _dialogService.DisplayAlert("操作失败", "存在未填写的节次", "OK");
                return;
            }

            var campus = await _campusStorageService.GetCampus();
            var newList = new List<NeuEvent>();
            foreach (var eventGroup in EventGroupList)
            {
                newList.AddRange(eventGroup.WeekNo.Select(weekNo => new NeuEvent
                {
                    Title = SelectedEvent.Title,
                    Code = SelectedEvent.Code,
                    Day = (int)eventGroup.Day,
                    IsDone = false,
                    Detail = eventGroup.Detail,
                    Time = Calculator.CalculateClassTime(eventGroup.Day, weekNo, eventGroup.ClassIndex, campus,
                        EventSemester.BaseDate),
                    Week = weekNo,
                    ClassNo = eventGroup.ClassIndex,
                    SemesterId = EventSemester.SemesterId,
                    IsUserGenerated = true
                }));
            }

            await _neuStorage.DeleteAllAsync((e => e.Code == SelectedEvent.Code));
            await _neuStorage.InsertAllAsync(newList);
            _dbStorageProvider.OnUpdateData();
            await _contentPageNavigationService.PopToRootAsync();
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

        #endregion

        public async Task PageAppearingCommandFunction()
        {
            EventGroupList.Clear();
            if (SelectedEvent is NeuEvent neuEvent)
            {
 
                EventSemester = await _semesterStorage.GetAsync(neuEvent.SemesterId);

                var courses =  await _neuStorage.GetAllAsync(e => e.Code == SelectedEvent.Code);
                var courseGroupList = courses.GroupBy(c => new { c.Day, c.ClassNo, c.Detail })
                    .OrderBy(p => p.Key.Day);
                foreach (var group in courseGroupList)
                {
                    EventGroupList.Add(
                        new EventGroup
                        {
                            Day = (DayOfWeek)group.Key.Day,
                            Detail = group.Key.Detail,
                            ClassIndex = group.Key.ClassNo,
                            WeekNo = group.ToList().ConvertAll(x => x.Week)
                        });
                }
            }
        }
    }
}