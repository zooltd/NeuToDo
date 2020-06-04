using System;

namespace NeuToDo.Models
{
    public class EventModel
    {
        public string Name { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// 是否已完成
        /// </summary>
        public bool IsDone { get; set; }

        /// <summary>
        /// 规定格式为 D/M/Y 12:00:00 AM
        /// </summary>
        public DateTime Date { get; set; }

        public DateTime Starting { get; set; }
    }
}