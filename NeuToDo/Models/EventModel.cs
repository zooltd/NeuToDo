using SQLite;
using System;

namespace NeuToDo.Models
{
    public class EventModel
    {
        [PrimaryKey, AutoIncrement]
        [Column(nameof(Id))]
        public int Id { get; set; }

        [Column(nameof(Title))] public string Title { get; set; }

        [Column(nameof(Detail))] public string Detail { get; set; }

        [Column(nameof(Time))] public DateTime Time { get; set; }

        /// <summary>
        /// 是否已完成
        /// </summary>
        [Column(nameof(IsDone))]
        public bool IsDone { get; set; }

        [Column(nameof(Code))] public string Code { get; set; }
    }
}