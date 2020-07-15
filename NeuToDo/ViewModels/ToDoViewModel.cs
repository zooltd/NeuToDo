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
using Xamarin.Plugin.Calendar.Models;

namespace NeuToDo.ViewModels
{
    public class ToDoViewModel : ViewModelBase
    {
        public ToDoViewModel(IDbStorageProvider dbStorageProvider,
            IContentPageNavigationService contentPageNavigationService,
            IDialogService dialogService,
            IAcademicCalendarService academicCalendarService,
            IPopupNavigationService popupNavigationService, ISyncService syncService)
        {
            _dbStorageProvider = dbStorageProvider;
            _neuStorage = dbStorageProvider.GetEventModelStorage<NeuEvent>();
            _moocStorage = dbStorageProvider.GetEventModelStorage<MoocEvent>();
            _userStorage = dbStorageProvider.GetEventModelStorage<UserEvent>();
            _academicCalendarService = academicCalendarService;
            _contentPageNavigationService = contentPageNavigationService;
            _dialogService = dialogService;
            _popupNavigationService = popupNavigationService;
            _syncService = syncService;
            dbStorageProvider.UpdateData += OnGetData;
            _today = DateTime.Today;
            ThisSunday = _today.AddDays(-(int) _today.DayOfWeek); //本周日
            ThisSaturday = ThisSunday.AddDays(6);
        }

        #region 私有变量

        private readonly IContentPageNavigationService _contentPageNavigationService;
        private readonly IDialogService _dialogService;
        private readonly IDbStorageProvider _dbStorageProvider;
        private readonly IEventModelStorage<NeuEvent> _neuStorage;
        private readonly IEventModelStorage<MoocEvent> _moocStorage;
        private readonly IEventModelStorage<UserEvent> _userStorage;
        private readonly IAcademicCalendarService _academicCalendarService;
        private readonly IPopupNavigationService _popupNavigationService;
        private readonly ISyncService _syncService;

        private Dictionary<DateTime, List<EventModel>> EventDict { get; set; } =
            new Dictionary<DateTime, List<EventModel>>();

        private bool _isLoaded;

        private readonly DateTime _today;

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
            var totalEventList = new List<EventModel>();
            totalEventList.AddRange(await _neuStorage.GetAllAsync(x => !x.IsDeleted));
            totalEventList.AddRange(await _moocStorage.GetAllAsync(x => !x.IsDeleted));
            totalEventList.AddRange(await _userStorage.GetAllAsync(x => !x.IsDeleted));
            EventDict = totalEventList.GroupBy(e => e.Time.Date).ToDictionary(g => g.Key, g => g.ToList());

            UpdateListData();
            UpdateCalendarData();
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
        private async Task UpdateSemester()
        {
            _academicCalendarService.Reset();
            ThisSunday = _today.AddDays(-(int) _today.DayOfWeek); //本周日
            ThisSaturday = ThisSunday.AddDays(6);
            (Semester, WeekNo) = await _academicCalendarService.GetCurrentSemester(); //TODO
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

            WeeklySummary = $"本周有{cnt}个ToDo事项, 所谓债多不压身";
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
            async e => await _contentPageNavigationService.PushAsync(e));

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
            await _dbStorageProvider.CheckInitialization(); //TODO
            await _syncService.SyncSyllabusAsync();
            await UpdateSemester();
            await LoadData();
            _isLoaded = true;
        }

        private RelayCommand<EventModel> _checkCommand;

        public RelayCommand<EventModel> CheckCommand =>
            _checkCommand ??= new RelayCommand<EventModel>(async (e) => await CheckCommandFunction(e));

        private async Task CheckCommandFunction(EventModel eventModel)
        {
            switch (eventModel)
            {
                case NeuEvent neuEvent:
                {
                    await _neuStorage.UpdateAsync(neuEvent);
                    break;
                }
                case MoocEvent moocEvent:
                {
                    await _moocStorage.UpdateAsync(moocEvent);
                    break;
                }
                case UserEvent userEvent:
                {
                    await _userStorage.UpdateAsync(userEvent);
                    break;
                }
            }
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

        /// <summary>
        /// 导航到新自定义事件编辑页面
        /// </summary>
        private RelayCommand _navigateToNewUserEventPage;

        /// <summary>
        /// 导航到新自定义事件编辑页面
        /// </summary>
        public RelayCommand NavigateToNewUserEventPage =>
            _navigateToNewUserEventPage ??= new RelayCommand(async () => await NavigateToNewUserEventPageFunction());

        private async Task NavigateToNewUserEventPageFunction()
        {
            await _contentPageNavigationService.PushAsync(new UserEvent
                {Code = Calculator.CalculateUniqueUserEventCode(), IsDone = false, Time = DateTime.Today});
        }


        /// <summary>
        /// 导航到新自定义事件编辑页面
        /// </summary>
        private RelayCommand _navigateToNewNeuEventPage;

        /// <summary>
        /// 导航到新课程编辑页面
        /// </summary>
        public RelayCommand NavigateToNewNeuEventPage =>
            _navigateToNewNeuEventPage ??= new RelayCommand(async () => await NavigateToNewNeuEventPageFunction());

        private async Task NavigateToNewNeuEventPageFunction()
        {
            await _contentPageNavigationService.PushAsync(new NeuEvent
            {
                SemesterId = Semester.SemesterId, Code = Calculator.CalculateUniqueNeuEventCode(), IsDone = false,
                Time = DateTime.Today
            });
        }

        #endregion

        #region List绑定命令

        /// <summary>
        /// 查看上周Event
        /// </summary>
        private RelayCommand _toLastWeek;

        /// <summary>
        /// 查看上周Event
        /// </summary>
        public RelayCommand ToLastWeek => _toLastWeek ??= new RelayCommand(async () => await ToLastWeekFunction());

        private async Task ToLastWeekFunction()
        {
            (Semester, WeekNo, ThisSunday) = await _academicCalendarService.ToLastWeekSemester();
            ThisSaturday = ThisSaturday.AddDays(-7);
            UpdateListData();
        }

        /// <summary>
        /// 查看下周Event
        /// </summary>
        private RelayCommand _toNextWeek;

        /// <summary>
        /// 查看下周Event
        /// </summary>
        public RelayCommand ToNextWeek => _toNextWeek ??= new RelayCommand(async () => await ToNextWeekFunction());

        private async Task ToNextWeekFunction()
        {
            (Semester, WeekNo, ThisSunday) = await _academicCalendarService.ToNextWeekSemester();
            ThisSaturday = ThisSaturday.AddDays(7);
            UpdateListData();
        }

        private RelayCommand _navigateToSearchPage;

        public RelayCommand NavigateToSearchPage =>
            _navigateToSearchPage ??= new RelayCommand(async () =>
                await _contentPageNavigationService.PushAsync(ContentNavigationConstants.SearchPage));

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

        #region SearchPage绑定属性

        private bool _searchStatus;

        public bool SearchStatus
        {
            get => _searchStatus;
            set => Set(nameof(SearchStatus), ref _searchStatus, value);
        }

        private string _queryString;

        public string QueryString
        {
            get => _queryString;
            set => Set(nameof(QueryString), ref _queryString, value);
        }

        private List<EventModel> _searchResult;

        public List<EventModel> SearchResult
        {
            get => _searchResult ??= new List<EventModel>();
            set => Set(nameof(SearchResult), ref _searchResult, value);
        }

        #endregion

        #region SearchPage绑定命令

        private RelayCommand _searchCommand;

        public RelayCommand SearchCommand =>
            _searchCommand ??= new RelayCommand(SearchCommandFunction);

        private void SearchCommandFunction()
        {
            SearchStatus = true;
            SearchResult.Clear();
            var eventList = EventDict.Values.ToList();

            if (eventList.Count == 0)
            {
                SearchResult = new List<EventModel>();
                SearchStatus = false;
                return;
            }

            foreach (var dailyEventList in eventList)
            foreach (var eventModel in dailyEventList)
                if (eventModel.Title.ToLower().Contains(QueryString.ToLower()) ||
                    eventModel.Detail.ToLower().Contains(QueryString.ToLower()))
                    SearchResult.Add(eventModel);

            SearchResult = SearchResult.OrderByDescending(x => x.Time).ToList();

            SearchStatus = false;
        }

        #endregion
    }
}