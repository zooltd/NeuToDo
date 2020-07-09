using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models;
using NeuToDo.Services;
using NeuToDo.Utils;

namespace NeuToDo.ViewModels
{
    public class NeuEventDetailViewModel : ViewModelBase
    {
        private readonly IEventModelStorage<NeuEvent> _neuStorage;
        private readonly ISemesterStorage _semesterStorage;
        private readonly IDbStorageProvider _dbStorageProvider;
        private readonly IPopupNavigationService _popupNavigationService;
        private readonly IDialogService _dialogService;
        private readonly IContentPageNavigationService _contentPageNavigationService;
        private readonly ICampusStorageService _campusStorageService;

        public NeuEventDetailViewModel(IDbStorageProvider dbStorageProvider,
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


        #region 绑定命令

        private RelayCommand _pageAppearingCommand;

        public RelayCommand PageAppearingCommand =>
            _pageAppearingCommand ??= new RelayCommand(async () =>
                await PageAppearingCommandFunction());

        private async Task PageAppearingCommandFunction()
        {
            NeuEventDetail = new NeuEventWrapper(SelectedEvent);
            NeuEventDetail.EventSemester = await _semesterStorage.GetAsync(NeuEventDetail.SemesterId);
            var courses = await _neuStorage.GetAllAsync(e => e.Code == SelectedEvent.Code);
            var courseGroupList = courses.GroupBy(c => new {c.Day, c.ClassNo, c.Detail})
                .OrderBy(p => p.Key.Day);
            foreach (var group in courseGroupList)
            {
                NeuEventDetail.EventGroupList.Add(
                    new EventGroup
                    {
                        Day = (DayOfWeek) group.Key.Day,
                        Detail = group.Key.Detail,
                        ClassIndex = group.Key.ClassNo,
                        WeekNo = group.ToList().ConvertAll(x => x.Week)
                    });
            }
        }

        private RelayCommand _addPeriod;

        public RelayCommand AddPeriod => _addPeriod ??= new RelayCommand((() =>
        {
            NeuEventDetail.EventGroupList.Add(new EventGroup {WeekNo = new List<int>()});
        }));

        private RelayCommand<EventGroup> _removePeriod;

        public RelayCommand<EventGroup> RemovePeriod =>
            _removePeriod ??= new RelayCommand<EventGroup>(g => { NeuEventDetail.EventGroupList.Remove(g); });

        private RelayCommand _deleteAll;

        public RelayCommand DeleteAll =>
            _deleteAll ??= new RelayCommand((async () => await DeleteAllFunction()));

        private async Task DeleteAllFunction()
        {
            var toDelete = await _dialogService.DisplayAlert("警告", "确定删除有关本课程的所有时间段？", "Yes", "No");
            if (!toDelete) return;
            await _neuStorage.DeleteAllAsync(e => e.Code == SelectedEvent.Code);
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

            if (NeuEventDetail.EventGroupList.ToList().Exists(x => x.WeekNo == null || !x.WeekNo.Any()))
            {
                _dialogService.DisplayAlert("操作失败", "存在未填写的周数", "OK");
                return;
            }

            if (NeuEventDetail.EventGroupList.ToList().Exists(x => x.ClassIndex < 1))
            {
                _dialogService.DisplayAlert("操作失败", "存在未填写的节次", "OK");
                return;
            }

            var campus = await _campusStorageService.GetCampus();
            var newList = new List<NeuEvent>();
            foreach (var eventGroup in NeuEventDetail.EventGroupList)
            {
                newList.AddRange(eventGroup.WeekNo.Select(weekNo => new NeuEvent
                {
                    Title = SelectedEvent.Title,
                    Code = SelectedEvent.Code,
                    Day = (int) eventGroup.Day,
                    IsDone = false,
                    Detail = eventGroup.Detail,
                    Time = Calculator.CalculateClassTime(eventGroup.Day, weekNo, eventGroup.ClassIndex, campus,
                        NeuEventDetail.EventSemester.BaseDate),
                    Week = weekNo,
                    ClassNo = eventGroup.ClassIndex,
                    SemesterId = NeuEventDetail.EventSemester.SemesterId,
                }));
            }

            await _neuStorage.DeleteAllAsync((e => e.Code == SelectedEvent.Code));
            await _neuStorage.InsertAllAsync(newList);
            _dbStorageProvider.OnUpdateData();
            await _contentPageNavigationService.PopToRootAsync();
        }

        private RelayCommand<EventGroup> _weekNoSelect;

        public RelayCommand<EventGroup> WeekNoSelect =>
            _weekNoSelect ??= new RelayCommand<EventGroup>((group) =>
            {
                SelectEventGroup = group;
                _popupNavigationService.PushAsync(PopupPageNavigationConstants.WeekNoSelectPopupPage);
            });

        #endregion

        #region 绑定属性

        /// <summary>
        /// ItemSource of DayPicker
        /// </summary>
        public Array DayItems => Enum.GetValues(typeof(DayOfWeek));

        /// <summary>
        /// ItemSource of ClassNoPicker
        /// </summary>
        public List<int> ClassIndexItems => Enumerable.Range(1, 12).ToList();

        /// <summary>
        /// Navigation Parameter
        /// </summary>
        private NeuEvent _selectedEvent;

        /// <summary>
        /// Navigation Parameter
        /// </summary>
        public NeuEvent SelectedEvent
        {
            get => _selectedEvent;
            set => Set(nameof(SelectedEvent), ref _selectedEvent, value);
        }

        private NeuEventWrapper _neuEventDetail;

        public NeuEventWrapper NeuEventDetail
        {
            get => _neuEventDetail;
            set => Set(nameof(NeuEventDetail), ref _neuEventDetail, value);
        }

        private ObservableCollection<int> _weekIndexInSelectionPage;

        public ObservableCollection<int> WeekIndexInSelectionPage
        {
            get => _weekIndexInSelectionPage;
            set => Set(nameof(WeekIndexInSelectionPage), ref _weekIndexInSelectionPage, value);
        }

        public EventGroup SelectEventGroup { get; set; }

        #endregion
    }

    public class NeuEventWrapper : NeuEvent
    {
        public ObservableCollection<EventGroup> EventGroupList { get; set; }

        public Semester EventSemester { get; set; }

        public NeuEventWrapper(NeuEvent neuEvent) : base(neuEvent)
        {
            EventGroupList = new ObservableCollection<EventGroup>();
        }
    }

    public class EventGroup
    {
        public string Detail { get; set; }
        public int ClassIndex { get; set; }
        public DayOfWeek Day { get; set; }
        public List<int> WeekNo { get; set; }
    }
}