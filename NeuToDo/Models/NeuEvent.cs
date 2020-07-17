using SQLite;

namespace NeuToDo.Models
{
    [Table(nameof(NeuEvent))]
    public class NeuEvent : EventModel
    {
        public NeuEvent(NeuEvent neuEvent)
        {
            Id = neuEvent.Id;
            Code = neuEvent.Code;
            Title = neuEvent.Title;
            Detail = neuEvent.Detail;
            Time = neuEvent.Time;
            IsDone = neuEvent.IsDone;
            PeriodId = neuEvent.PeriodId;
            Day = neuEvent.Day;
            Week = neuEvent.Week;
            SemesterId = neuEvent.SemesterId;
            ClassNo = neuEvent.ClassNo;
            IsUserGenerated = neuEvent.IsUserGenerated;
            Uuid = neuEvent.Uuid;
            IsDeleted = neuEvent.IsDeleted;
            LastModified = neuEvent.LastModified;
        }

        public NeuEvent()
        {
        }

        [Column(nameof(PeriodId))] public int PeriodId { get; set; }

        [Column(nameof(Day))] public int Day { get; set; }

        [Column(nameof(Week))] public int Week { get; set; }

        [Column(nameof(SemesterId))] public int SemesterId { get; set; }

        [Column(nameof(ClassNo))] public int ClassNo { get; set; }

        [Column(nameof(IsUserGenerated))] public bool IsUserGenerated { get; set; }
    }
}