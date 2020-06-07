using System;
using NeuToDo.Models.SettingsModels;
using SQLite;

namespace NeuToDo.Models
{

    public class EventModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Detail { get; set; }

        public DateTime Starting { get; set; }

        /// <summary>
        /// 是否已完成
        /// </summary>
        public bool IsDone { get; set; }
    }
}