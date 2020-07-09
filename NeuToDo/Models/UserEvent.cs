﻿using System;
using NeuToDo.ViewModels;
using SQLite;

namespace NeuToDo.Models
{
    [Table(nameof(UserEvent))]
    public class UserEvent : EventModel
    {
        [Column(nameof(IsRepeat))] public bool IsRepeat { get; set; }

        #region Class Period //TODO

        [Column(nameof(StartDate))] public DateTime StartDate { get; set; }
        [Column(nameof(EndDate))] public DateTime EndDate { get; set; }
        [Column(nameof(TimeOfDay))] public TimeSpan TimeOfDay { get; set; }
        [Column(nameof(DaySpan))] public int DaySpan { get; set; }

        #endregion


        public UserEvent(UserEvent userEvent)
        {
            Id = userEvent.Id;
            Code = userEvent.Code;
            Detail = userEvent.Detail;
            Title = userEvent.Title;
            Time = userEvent.Time;
            IsDone = userEvent.IsDone;
            IsRepeat = userEvent.IsRepeat;
        }

        public UserEvent(UserEvent userEvent, Period period)
        {
            Id = userEvent.Id;
            Code = userEvent.Code;
            Detail = userEvent.Detail;
            Title = userEvent.Title;
            Time = userEvent.Time;
            IsDone = userEvent.IsDone;
            IsRepeat = userEvent.IsRepeat;
            StartDate = period.StartDate;
            EndDate = period.EndDate;
            TimeOfDay = period.TimeOfDay;
            DaySpan = period.DaySpan;
        }

        public UserEvent()
        {
        }
    }
}