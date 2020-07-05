using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace NeuToDo.Models
{
    public class EventGroup : ObservableObject
    {
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