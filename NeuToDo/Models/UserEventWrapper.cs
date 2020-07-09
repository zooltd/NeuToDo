﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NeuToDo.Models
{
    public class UserEventWrapper : UserEvent
    {
        public DateTime EventDate { get; set; }

        public TimeSpan EventTime { get; set; }

        public ObservableCollection<Period> EventPeriods { get; set; }

        public UserEventWrapper(UserEvent userEvent) : base(userEvent)
        {
            EventDate = userEvent.Time.Date;
            EventTime = userEvent.Time.TimeOfDay;
            EventPeriods = new ObservableCollection<Period>();
        }

        public IEnumerable<UserEvent> GetUserEvents()
        {
            var userEvents = new List<UserEvent>();

            //保存单个
            if (!IsRepeat || EventPeriods.Count < 1)
            {
                Time = EventDate + EventTime;
                userEvents.Add(new UserEvent(this));
            }
            else
            {
                foreach (var period in EventPeriods)
                {
                    for (var time = period.StartDate + period.TimeOfDay;
                        time < period.EndDate;
                        time = time.AddDays(period.DaySpan))
                    {
                        Time = time;
                        userEvents.Add(new UserEvent(this, period));
                    }
                }
            }

            return userEvents;
        }
    }
}