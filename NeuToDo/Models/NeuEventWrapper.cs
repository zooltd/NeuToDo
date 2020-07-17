using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using NeuToDo.Utils;

namespace NeuToDo.Models
{
    public class NeuEventWrapper : NeuEvent
    {
        public ObservableCollection<NeuEventPeriod> EventPeriods { get; set; }

        public Semester EventSemester { get; set; }
        public Campus Campus { get; set; }

        public NeuEventWrapper(NeuEvent neuEvent) : base(neuEvent)
        {
            EventPeriods = new ObservableCollection<NeuEventPeriod>();
        }

        public List<NeuEvent> GetNeuEvents()
        {
            var neuEvents = new List<NeuEvent>();

            foreach (var eventPeriod in EventPeriods)
            {
                foreach (var weekNo in eventPeriod.WeekNo)
                {
                    neuEvents.Add(new NeuEvent
                    {
                        SemesterId = SemesterId,
                        Title = Title,
                        Code = Code,
                        PeriodId = eventPeriod.PeriodId,
                        Detail = eventPeriod.Detail,
                        ClassNo = eventPeriod.ClassIndex,
                        Day = (int) eventPeriod.Day,
                        Time = Calculator.CalculateClassTime(eventPeriod.Day, weekNo, eventPeriod.ClassIndex, Campus.南湖,
                            EventSemester.BaseDate),
                        Week = weekNo,
                        LastModified = DateTime.Now,
                        Uuid = Guid.NewGuid().ToString()
                    });
                }
            }

            return neuEvents;
        }
    }

    public class NeuEventPeriod : ObservableObject
    {
        public int PeriodId { get; set; }
        public string Detail { get; set; }
        public int ClassIndex { get; set; }
        public DayOfWeek Day { get; set; }

        private List<int> _weekNo;

        public List<int> WeekNo
        {
            get => _weekNo;
            set => Set(nameof(WeekNo), ref _weekNo, value);
        }
    }
}