﻿using SQLite;

namespace NeuToDo.Models
{
    [Table(nameof(NeuEvent))]
    public class NeuEvent : EventModel
    {
        public NeuEvent(NeuEvent neuEvent)
        {
            Id = neuEvent.Id;
            Code = neuEvent.Code;
            Title = neuEvent.Title;
            Detail = neuEvent.Detail;
            Time = neuEvent.Time;
            IsDone = neuEvent.IsDone;
            Day = neuEvent.Day;
            Week = neuEvent.Week;
            SemesterId = neuEvent.SemesterId;
            ClassNo = neuEvent.ClassNo;
            IsUserGenerated = neuEvent.IsUserGenerated;
        }

        public NeuEvent()
        {
        }

        [Column(nameof(Day))] public int Day { get; set; }

        [Column(nameof(Week))] public int Week { get; set; }

        [Column(nameof(SemesterId))] public int SemesterId { get; set; }

        [Column(nameof(ClassNo))] public int ClassNo { get; set; }

        [Column(nameof(IsUserGenerated))] public bool IsUserGenerated { get; set; }
    }
}