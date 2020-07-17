using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NeuToDo.Models
{
    public class UserEventWrapper : UserEvent
    {
        public DateTime EventDate { get; set; }

        public TimeSpan EventTime { get; set; }

        public ObservableCollection<UserEventPeriod> EventPeriods { get; set; }

        public UserEventWrapper(UserEvent userEvent) : base(userEvent)
        {
            EventDate = userEvent.Time.Date;
            EventTime = userEvent.Time.TimeOfDay;
            EventPeriods = new ObservableCollection<UserEventPeriod>();
        }

        public List<UserEvent> GetUserEvents()
        {
            var userEvents = new List<UserEvent>();

            //保存单个
            if (!IsRepeat)
            {
                userEvents.Add(new UserEvent(this) {Time = EventDate + EventTime});
            }
            else
            {
                if (EventPeriods.Count == 0) return new List<UserEvent>();
                foreach (var period in EventPeriods)
                {
                    for (var time = period.StartDate + period.TimeOfDay;
                        time <= period.EndDate;
                        time = time.AddDays(period.DaySpan))
                    {
                        Time = time;
                        userEvents.Add(new UserEvent
                        {
                            Title = Title,
                            Code = Code,
                            Detail = Detail,
                            IsRepeat = IsRepeat,
                            Time = time,
                            PeriodId = period.PeriodId,
                            StartDate = period.StartDate,
                            EndDate = period.EndDate,
                            DaySpan = period.DaySpan,
                            TimeOfDay = period.TimeOfDay,
                            Uuid = Guid.NewGuid().ToString(),
                            LastModified = DateTime.Now
                        });
                    }
                }
            }

            return userEvents;
        }
    }
}