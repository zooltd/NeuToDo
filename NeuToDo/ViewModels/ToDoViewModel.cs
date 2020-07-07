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
using Xamarin.Plugin.Calendar.Models;

namespace NeuToDo.ViewModels
{
    public class ToDoViewModel : ViewModelBase
    {
        public ToDoViewModel(IStorageProvider storageProvider,
            IContentPageNavigationService contentPageNavigationService,
            IDialogService dialogService)
        {
            _storageProvider = storageProvider;
            _contentPageNavigationService = contentPageNavigationService;
            _dialogService = dialogService;
            storageProvider.UpdateData += OnGetData;
            _today = DateTime.Today;
            ThisSunday = _today.AddDays(-(int) _today.DayOfWeek); //本周日
            ThisSaturday = ThisSunday.AddDays(6);
        }

        #region 私有变量

        private readonly IStorageProvider _storageProvider;

        private readonly IContentPageNavigationService _contentPageNavigationService;

        private readonly IDialogService _dialogService;

        private Dictionary<DateTime, List<EventModel>> EventDict { get; set; } =
            new Dictionary<DateTime, List<EventModel>>();

        private bool _isLoaded;

        private readonly DateTime _today;

        private List<Semester> _semesters;

        private int _semesterIndex;

        private static readonly Semester EmptySemester = new Semester
            {SchoolYear = "未知的时间裂缝", Season = "请关联教务处", BaseDate = DateTime.MinValue, SemesterId = 0};

        #endregion

        #region 共用私有函数

        private async void OnGetData(object sender, EventArgs e)
        {
            await LoadData();
        }

        /// <summary>
        /// 从DB中读取数据到EventDict和_semesters, 并更新两个视图的绑定属性
        /// </summary>
        /// <returns></returns>
        private async Task LoadData()
        {
            var neuStorage = await _storageProvider.GetEventModelStorage<NeuEvent>();
            var moocStorage = await _storageProvider.GetEventModelStorage<MoocEvent>();
            var totalEventList = new List<EventModel>();
            totalEventList.AddRange(await neuStorage.GetAllAsync());
            totalEventList.AddRange(await moocStorage.GetAllAsync());
            EventDict = totalEventList.GroupBy(e => e.Time.Date).ToDictionary(g => g.Key, g => g.ToList());

            var semesterStorage = await _storageProvider.GetSemesterStorage();
            _semesters = await semesterStorage.GetAllOrderedByBaseDateAsync();

            UpdateCalendarData();
            UpdateSemester();
            UpdateListData();
        }

        /// <summary>
        /// 更新ToDoCalendar界面绑定属性EventCollection
        /// </summary>
        private void UpdateCalendarData()
        {
            EventCollection.Clear();
            foreach (var pair in EventDict)
                EventCollection.Add(pair.Key, pair.Value);
        }

        /// <summary>
        /// 更新ToDoList界面绑定属性ThisSunday, ThisSaturday, Semester, WeekNo
        /// </summary>
        private void UpdateSemester()
        {
            ThisSunday = _today.AddDays(-(int) _today.DayOfWeek); //本周日
            ThisSaturday = ThisSunday.AddDays(6);

            _semesterIndex = 0;
            if (_semesters.Count <= 0)
            {
                Semester = EmptySemester;
                WeekNo = 0;
            }
            else
            {
                Semester = _semesters[_semesterIndex];
                WeekNo = Calculator.CalculateWeekNo(Semester.BaseDate, DateTime.Today);
            }
        }

        /// <summary>
        /// 更新ToDoList界面绑定属性WeeklyAgenda, WeeklySummary
        /// </summary>
        private void UpdateListData()
        {
            var cnt = 0;
            WeeklyAgenda.Clear();
            for (var i = 0; i < 7; i++)
            {
                var currDay = ThisSunday.AddDays(i);
                WeeklyAgenda.Add(EventDict.ContainsKey(currDay)
                    ? new DailyAgenda(currDay, EventDict[currDay])
                    : new DailyAgenda(currDay, new List<EventModel>()));
                cnt += WeeklyAgenda[i].EventList.Count;
            }

            WeeklySummary = $"你本周有{cnt}个ToDo事项, 所谓债多不压身";
        }

        #endregion

        #region 共有绑定属性

        #endregion

        #region 共有绑定命令

        /// <summary>
        /// ToDoList, ToDoCalendar视图中Event点击命令，触发导航
        /// </summary>
        private RelayCommand<EventModel> _eventTappedCommand;

        /// <summary>
        /// ToDoList, ToDoCalendar视图中Event点击命令，触发导航
        /// </summary>
        public RelayCommand<EventModel> EventTappedCommand => _eventTappedCommand ??= new RelayCommand<EventModel>(
            ((e) => { _contentPageNavigationService.PushAsync(e); }));

        /// <summary>
        /// 页面显示命令
        /// </summary>
        private RelayCommand _pageAppearingCommand;

        /// <summary>
        /// 页面显示命令
        /// </summary>
        public RelayCommand PageAppearingCommand => _pageAppearingCommand ??=
            new RelayCommand(async () => await PageAppearingCommandFunction());

        private async Task PageAppearingCommandFunction()
        {
            if (_isLoaded) return;
            await LoadData();
            _isLoaded = true;
        }

        private RelayCommand<EventModel> _checkCommand;

        public RelayCommand<EventModel> CheckCommand =>
            _checkCommand ??= new RelayCommand<EventModel>(async (e) => await CheckCommandFunction(e));

        private async Task CheckCommandFunction(EventModel eventModel)
        {
            // switch (eventModel)
            // {
            //     case NeuEvent neuEvent:
            //     {
            //         var storage = await _storageProvider.GetEventModelStorage<NeuEvent>();
            //         await storage.UpdateAsync(neuEvent);
            //         break;
            //     }
            //     case MoocEvent moocEvent:
            //     {
            //         var storage = await _storageProvider.GetEventModelStorage<MoocEvent>();
            //         await storage.UpdateAsync(moocEvent);
            //         break;
            //     }
            //     case UserEvent userEvent:
            //     {
            //         var storage = await _storageProvider.GetEventModelStorage<UserEvent>();
            //         await storage.UpdateAsync(userEvent);
            //         break;
            //     }
            // }
        }

        #endregion

        #region Calendar绑定命令

        /// <summary>
        /// 点击CalendarHeader月份时跳转到当天
        /// </summary>
        private RelayCommand _monthYearTappedCommand;

        /// <summary>
        /// 点击CalendarHeader月份时跳转到当天
        /// </summary>
        public RelayCommand MonthYearTappedCommand =>
            _monthYearTappedCommand ??= new RelayCommand((() =>
            {
                MonthYear = DateTime.Today;
                SelectedDate = DateTime.Today;
            }));

        #endregion

        #region List绑定命令

        /// <summary>
        /// 查看上周Event
        /// </summary>
        private RelayCommand _toLastWeek;

        /// <summary>
        /// 查看上周Event
        /// </summary>
        public RelayCommand ToLastWeek => _toLastWeek ??= new RelayCommand(ToLastWeekFunction);

        private void ToLastWeekFunction()
        {
            ThisSunday = ThisSunday.AddDays(-7);
            ThisSaturday = ThisSaturday.AddDays(-7);

            if (WeekNo > 0)
            {
                WeekNo--;
                UpdateListData();
                return;
            }

            ++_semesterIndex;

            if (_semesters.Count > _semesterIndex)
            {
                Semester = _semesters[_semesterIndex];
                WeekNo = Calculator.CalculateWeekNo(Semester.BaseDate, ThisSunday);
            }
            else
            {
                Semester = EmptySemester;
            }

            UpdateListData();
        }

        /// <summary>
        /// 查看下周Event
        /// </summary>
        private RelayCommand _toNextWeek;

        /// <summary>
        /// 查看下周Event
        /// </summary>
        public RelayCommand ToNextWeek => _toNextWeek ??= new RelayCommand(ToNextWeekFunction);

        private void ToNextWeekFunction()
        {
            ThisSunday = ThisSunday.AddDays(7);
            ThisSaturday = ThisSaturday.AddDays(7);

            if (_semesterIndex > _semesters.Count)
            {
                _semesterIndex--;
                UpdateListData();
                return;
            }

            var maxThisSemesterSunday = _semesterIndex >= 1
                ? _semesters[_semesterIndex - 1].BaseDate.AddDays(-7)
                : DateTime.MaxValue;
            if (ThisSunday > maxThisSemesterSunday)
            {
                Semester = _semesters[--_semesterIndex];
                WeekNo = 0;
            }
            else
            {
                WeekNo++;
            }

            UpdateListData();
        }

        /// <summary>
        /// 导航到新自定义事件编辑页面
        /// </summary>
        private RelayCommand _navigateToNewUserEventPage;

        /// <summary>
        /// 导航到新自定义事件编辑页面
        /// </summary>
        public RelayCommand NavigateToNewUserEventPage =>
            _navigateToNewUserEventPage ??= new RelayCommand((() =>
            {
                _contentPageNavigationService.PushAsync(new UserEvent());
            }));

        /// <summary>
        /// 导航到新自定义事件编辑页面
        /// </summary>
        private RelayCommand _navigateToNewNeuEventPage;

        /// <summary>
        /// 导航到新课程编辑页面
        /// </summary>
        public RelayCommand NavigateToNewNeuEventPage =>
            _navigateToNewNeuEventPage ??= new RelayCommand(NavigateToNewNeuEventPageFunction);

        private void NavigateToNewNeuEventPageFunction()
        {
            if (Semester.SemesterId == 0)
            {
                _dialogService.DisplayAlert("提示", "当前学期不明，请关联东北大学(设置=>关联)", "OK");
                return;
            }

            _contentPageNavigationService.PushAsync(new NeuEvent
                {SemesterId = Semester.SemesterId, Code = Calculator.CalculateUniqueNeuEventCode(), IsDone = false});
        }

        #endregion


        #region Calendar绑定属性

        public EventCollection EventCollection { get; } = new EventCollection();

        private DateTime _selectedDate = DateTime.Today;

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set => Set(nameof(SelectedDate), ref _selectedDate, value);
        }

        private DateTime _monthYear = DateTime.Today;

        public DateTime MonthYear
        {
            get => _monthYear;
            set => Set(nameof(MonthYear), ref _monthYear, value);
        }

        #endregion

        #region List绑定属性

        public ObservableCollection<DailyAgenda> WeeklyAgenda { get; } = new ObservableCollection<DailyAgenda>();

        private DateTime _thisSunday;

        public DateTime ThisSunday
        {
            get => _thisSunday;
            set => Set(nameof(ThisSunday), ref _thisSunday, value);
        }

        private DateTime _thisSaturday;

        public DateTime ThisSaturday
        {
            get => _thisSaturday;
            set => Set(nameof(ThisSaturday), ref _thisSaturday, value);
        }

        private int _weekNo;

        public int WeekNo
        {
            get => _weekNo;
            set => Set(nameof(WeekNo), ref _weekNo, value);
        }

        private Semester _semester;

        public Semester Semester
        {
            get => _semester;
            set => Set(nameof(Semester), ref _semester, value);
        }

        private string _weeklySummary;

        public string WeeklySummary
        {
            get => _weeklySummary;
            set => Set(nameof(WeeklySummary), ref _weeklySummary, value);
        }

        #endregion
    }
}