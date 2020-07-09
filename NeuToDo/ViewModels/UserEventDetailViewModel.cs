using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Components;
using NeuToDo.Models;

namespace NeuToDo.ViewModels
{
    public class UserEventDetailViewModel : ViewModelBase
    {
        private List<int> _daySpans;
        public List<int> DaySpans => Enumerable.Range(1, 31).ToList();

        private UserEvent _selectedEvent;

        public UserEvent SelectedEvent
        {
            get => _selectedEvent;
            set => Set(nameof(SelectedEvent), ref _selectedEvent, value);
        }


        private UserEventWrapper _userEventDetail;

        public UserEventWrapper UserEventDetail
        {
            get => _userEventDetail;
            set => Set(nameof(UserEventDetail), ref _userEventDetail, value);
        }

        private RelayCommand _pageAppearingCommand;

        public RelayCommand PageAppearingCommand =>
            _pageAppearingCommand ??= new RelayCommand(async () => await PageAppearingCommandFunction());

        private async Task PageAppearingCommandFunction()
        {
            UserEventDetail = new UserEventWrapper(SelectedEvent);
        }

        private RelayCommand _deleteAll;

        public RelayCommand DeleteAll =>
            _deleteAll ??= new RelayCommand(() => { });

        private RelayCommand _addPeriod;

        public RelayCommand AddPeriod =>
            _addPeriod ??= new RelayCommand(() =>
            {
                UserEventDetail.EventPeriods.Add(new Period
                {
                    StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(7),
                    EventTime = TimeSpan.Zero, DaySpan = 1
                });
            });

        private RelayCommand _editDone;

        public RelayCommand EditDone =>
            _editDone ??= new RelayCommand(async () => await EditDoneFunction());

        private async Task EditDoneFunction()
        {
        }

        private RelayCommand<Period> _removePeriod;

        public RelayCommand<Period> RemovePeriod =>
            _removePeriod ??= new RelayCommand<Period>((p) => { UserEventDetail.EventPeriods.Remove(p); });
    }

    public class UserEventWrapper
    {
        public UserEvent UserEvent { get; set; }

        public DateTime EventDate { get; set; }

        public TimeSpan EventTime { get; set; }

        public ObservableCollection<Period> EventPeriods { get; set; }

        public UserEventWrapper(UserEvent userEvent)
        {
            UserEvent = userEvent;
            EventDate = userEvent.Time.Date;
            EventTime = userEvent.Time.TimeOfDay;
            EventPeriods = new ObservableCollection<Period>();
        }
    }

    public class Period
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan EventTime { get; set; }
        public int DaySpan { get; set; }
    }
}