using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models;
using NeuToDo.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Plugin.Calendar.Models;

namespace NeuToDo.ViewModels
{
    public class ToDoCalendarViewModel : ViewModelBase
    {
        /// <remarks>
        /// 注意有坑 Events无法添加多个属于一天的DateTime
        /// 赋值操作无法触发Notify
        /// </remarks>
        public EventCollection Events { get; private set; } = new EventCollection();

        private readonly IEventModelStorage<NeuEvent> _eventModelStorage;

        public ToDoCalendarViewModel(IEventModelStorageProvider eventModelStorageProvider)
        {
            var task = eventModelStorageProvider.GetEventModelStorage<NeuEvent>();
            task.Wait();
            _eventModelStorage = task.Result;
        }

        #region 绑定命令

        private RelayCommand _pageAppearingCommand;

        public RelayCommand PageAppearingCommand
            => _pageAppearingCommand ??= new RelayCommand(async () =>
                await PageAppearingCommandFunction());

        //TODO 需要检查是否已有DateTime
        private async Task PageAppearingCommandFunction()
        {
            try
            {
                var eventList = await _eventModelStorage.GetAllAsync();
                var eventDict = eventList.GroupBy(e => e.Starting.Date).ToDictionary(g => g.Key, g => g.ToList());
                foreach (var pair in eventDict)
                {
                    Events.Add(pair.Key, pair.Value);
                }
            }
            catch (Exception e)
            {
                // ignored
            }
        }


        /// <summary>
        /// test
        /// </summary>
        private RelayCommand<object> _eventSelectedCommand;

        public RelayCommand<object> EventSelectedCommand => _eventSelectedCommand ??=
            new RelayCommand<object>(async (item) => await ExecuteEventSelectedCommand(item));

        private async Task ExecuteEventSelectedCommand(object item)
        {
            if (item is EventModel eventModel)
            {
                var title = $"Selected: {eventModel.Title}";
                var message = $"Starts: {eventModel.Starting:HH:mm}{Environment.NewLine}Details: {eventModel.Starting}";
                await Application.Current.MainPage.DisplayAlert(title, message, "Ok");
                Console.WriteLine(message);
            }
        }

        private RelayCommand _todayCommand;

        public RelayCommand TodayCommand =>
            _todayCommand ??= new RelayCommand((() => { SelectedDate = DateTime.Today; }));

        private RelayCommand _swipeLeftCommand;

        public RelayCommand SwipeLeftCommand =>
            _swipeLeftCommand ??= new RelayCommand((() => { MonthYear = MonthYear.AddMonths(2); }));

        private RelayCommand _swipeRightCommand;

        public RelayCommand SwipeRightCommand =>
            _swipeRightCommand ??= new RelayCommand((() => { MonthYear = MonthYear.AddMonths(-2); }));

        private RelayCommand _swipeUpCommand;

        public RelayCommand SwipeUpCommand =>
            _swipeUpCommand ??= new RelayCommand((() => { MonthYear = DateTime.Today; }));

        #endregion

        #region 绑定属性

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
    }
}