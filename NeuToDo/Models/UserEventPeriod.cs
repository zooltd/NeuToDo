using System;

namespace NeuToDo.Models
{
    public class UserEventPeriod
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan TimeOfDay { get; set; }
        public int DaySpan { get; set; }
    }
}