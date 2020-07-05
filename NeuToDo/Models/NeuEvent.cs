using System;

namespace NeuToDo.Models
{
    [SQLite.Table(nameof(NeuEvent))]
    public class NeuEvent : EventModel
    {
        [SQLite.Column(nameof(Day))] public int Day { get; set; }

        [SQLite.Column(nameof(Week))] public int Week { get; set; }

        [SQLite.Column(nameof(SemesterId))] public int SemesterId { get; set; }

        [SQLite.Column(nameof(ClassNo))] public int ClassNo { get; set; }
    }
}