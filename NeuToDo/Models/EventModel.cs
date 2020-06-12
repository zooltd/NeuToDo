using SQLite;
using System;

namespace NeuToDo.Models
{

    public class EventModel
    {
        [PrimaryKey, AutoIncrement]
        [SQLite.Column("id")]
        public int Id { get; set; }

        [SQLite.Column("title")]
        public string Title { get; set; }

        [SQLite.Column("detail")]
        public string Detail { get; set; }

        [SQLite.Column("starting")]
        public DateTimeOffset Starting { get; set; }

        /// <summary>
        /// 是否已完成
        /// </summary>
        [SQLite.Column("is_done")]
        public bool IsDone { get; set; }

        [SQLite.Column("code")]
        public string Code { get; set; }
    }
}