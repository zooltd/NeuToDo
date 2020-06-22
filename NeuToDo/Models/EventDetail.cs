using System;
using System.Collections.Generic;

namespace NeuToDo.Models
{
    public class EventDetail : EventModel
    {
        public string TypeName;

        public bool CanRepeat;

        public bool ShowSwitchCell;

        public bool IsRepeat;

        public List<Dictionary<DayOfWeek, string>> RepeatList;

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
            RepeatList = new List<Dictionary<DayOfWeek, string>>();
        }
    }
}