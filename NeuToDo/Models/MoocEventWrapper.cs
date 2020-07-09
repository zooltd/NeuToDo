using System;

namespace NeuToDo.Models
{
    public class MoocEventWrapper : MoocEvent
    {
        public DateTime EventDate { get; set; }
        public TimeSpan EventTime { get; set; }

        public MoocEventWrapper(MoocEvent moocEvent) : base(moocEvent)
        {
            EventDate = moocEvent.Time.Date;
            EventTime = moocEvent.Time.TimeOfDay;
        }
    }
}