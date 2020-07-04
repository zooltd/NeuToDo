using System;

namespace NeuToDo.Models
{
    [SQLite.Table("NeuEvent")]
    public class NeuEvent : EventModel
    {
        [SQLite.Column("day")] public int Day { get; set; }

        [SQLite.Column("week")] public int Week { get; set; }

        [SQLite.Column(nameof(SemesterId))] public int SemesterId { get; set; }

        [SQLite.Column(nameof(ClassNo))] public int ClassNo { get; set; }
    }
}