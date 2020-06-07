using System;
using SQLite;

namespace NeuToDo.Models
{
    /// <summary>
    /// TODO 继承？  : EventModel
    /// </summary>
    [SQLite.Table("courses")]
    public class CourseEventModel : EventModel
    {
        [PrimaryKey, AutoIncrement]
        [SQLite.Column("id")]
        public new int Id { get; set; }

        [SQLite.Column("title")]
        public new string Title { get; set; }

        [SQLite.Column("detail")]
        public new string Detail { get; set; }

        [SQLite.Column("starting")]
        public new DateTime Starting { get; set; }

        /// <summary>
        /// 是否已完成
        /// </summary>
        [SQLite.Column("is_done")]
        public new bool IsDone { get; set; }

        [SQLite.Column("course_id")]
        public string CourseId { get; set; }
    }
}