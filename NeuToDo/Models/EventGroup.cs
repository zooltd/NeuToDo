﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NeuToDo.Models
{
    public class EventGroup
    {
        public string Detail { get; set; }
        public int ClassIndex { get; set; }
        public DayOfWeek Day { get; set; }
        public ObservableCollection<int> WeekNo { get; set; }
    }

    public enum DayOfWeekCHN
    {
        周日,
        周一,
        周二,
        周三,
        周四,
        周五,
        周六
    }

    public enum ClassIndexCHN
    {
        第0节,
        第1节,
        第2节,
        第3节,
        第4节,
        第5节,
        第6节,
        第7节,
        第8节,
        第9节,
        第10节,
        第11节,
        第12节,
    }
}