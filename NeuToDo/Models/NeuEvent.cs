using System;

namespace NeuToDo.Models
{
    [SQLite.Table("NeuEvent")]
    public class NeuEvent : EventModel
    {
        [SQLite.Column("day")]
        public int Day { get; set; }

        [SQLite.Column("week")]
        public int Week { get; set; }
    }
}