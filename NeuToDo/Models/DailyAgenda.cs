using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NeuToDo.Models
{
    public class DailyAgenda
    {
        public string Topic { get; set; }
        public string Duration { get; set; }
        public DateTime Date { get; set; }
        public ObservableCollection<EventModel> EventList { get; set; }
        public string Color { get; set; }

        public DailyAgenda(DateTime date, List<EventModel> eventList)
        {
            Topic = eventList.Count == 0 ? "本日无事" : "本日计划";
            Duration = "00:00 - 24:00"; //TODO
            Date = date;
            EventList = new ObservableCollection<EventModel>(eventList);
            Color = DailyColor.DayColor[date.DayOfWeek];
        }
    }

    public static class DailyColor
    {
        public static readonly Dictionary<DayOfWeek, string> DayColor = new Dictionary<DayOfWeek, string>
        {
            {DayOfWeek.Sunday, "#B96CBD"},
            {DayOfWeek.Monday, "#49A24D"},
            {DayOfWeek.Tuesday, "#FDA838"},
            {DayOfWeek.Wednesday, "#F75355"},
            {DayOfWeek.Thursday, "#00C6AE"},
            {DayOfWeek.Friday, "#455399"},
            {DayOfWeek.Saturday, "#FFD700"},
        };
    }
}