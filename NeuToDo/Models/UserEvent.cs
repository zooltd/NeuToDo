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
            StartDate = userEvent.StartDate;
            EndDate = userEvent.EndDate;
            TimeOfDay = userEvent.TimeOfDay;
            DaySpan = userEvent.DaySpan;
            Uuid = userEvent.Uuid;
            IsDeleted = userEvent.IsDeleted;
            LastModified = userEvent.LastModified;
        }

        public UserEvent(UserEvent userEvent, UserEventPeriod userEventPeriod)
        {
            Id = userEvent.Id;
            Code = userEvent.Code;
            Detail = userEvent.Detail;
            Title = userEvent.Title;
            Time = userEvent.Time;
            IsDone = userEvent.IsDone;
            IsRepeat = userEvent.IsRepeat;
            Uuid = userEvent.Uuid;
            IsDeleted = userEvent.IsDeleted;
            LastModified = userEvent.LastModified;
            StartDate = userEventPeriod.StartDate;
            EndDate = userEventPeriod.EndDate;
            TimeOfDay = userEventPeriod.TimeOfDay;
            DaySpan = userEventPeriod.DaySpan;
            Uuid = userEvent.Uuid;
            IsDeleted = userEvent.IsDeleted;
            LastModified = userEvent.LastModified;
        }

        public UserEvent()
        {
        }
    }
}