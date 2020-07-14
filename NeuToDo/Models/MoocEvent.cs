using System.Net.Mail;

namespace NeuToDo.Models
{
    [SQLite.Table(nameof(MoocEvent))]
    public class MoocEvent : EventModel
    {
        public MoocEvent(MoocEvent moocEvent)
        {
            Id = moocEvent.Id;
            Code = moocEvent.Code;
            Title = moocEvent.Title;
            Detail = moocEvent.Detail;
            Time = moocEvent.Time;
            IsDone = moocEvent.IsDone;
            Uuid = moocEvent.Uuid;
            IsDeleted = moocEvent.IsDeleted;
            LastModified = moocEvent.LastModified;
        }

        public MoocEvent()
        {
        }
    }
}