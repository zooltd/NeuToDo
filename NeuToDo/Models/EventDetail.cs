using System;
using System.Collections.Generic;

namespace NeuToDo.Models
{
    public class EventDetail : EventModel
    {
        public string TypeName { get; set; }

        public bool CanRepeat { get; set; }

        public bool ShowSwitchCell { get; set; }

        public bool IsRepeat { get; set; }

        public List<TimeTable> RepeatList { get; set; }

        public EventDetail(EventModel e)
        {
            Id = e.Id;
            Title = e.Title;
            Detail = e.Detail;
            Time = e.Time;
            IsDone = e.IsDone;
            Code = e.Code;
            TypeName = e.GetType().Name;
            switch (TypeName)
            {
                case nameof(NeuEvent):
                    CanRepeat = true;
                    ShowSwitchCell = false;
                    IsRepeat = true;
                    break;
                case nameof(MoocEvent):
                    CanRepeat = ShowSwitchCell = false;
                    IsRepeat = false;
                    break;
                case nameof(UserEvent):
                    CanRepeat = ShowSwitchCell = true;
                    IsRepeat = false;
                    break;
            }

            RepeatList = new List<TimeTable>();
        }
    }

    public class TimeTable
    {
        public DayOfWeek Day { get; set; }
        public string WeekNo { get; set; }
    }
}