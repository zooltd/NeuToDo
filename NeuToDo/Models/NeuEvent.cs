using SQLite;

namespace NeuToDo.Models
{
    [Table(nameof(NeuEvent))]
    public class NeuEvent : EventModel
    {
        [Column(nameof(Day))] public int Day { get; set; }

        [Column(nameof(Week))] public int Week { get; set; }

        [Column(nameof(SemesterId))] public int SemesterId { get; set; }

        [Column(nameof(ClassNo))] public int ClassNo { get; set; }

        [Column(nameof(IsUserGenerated))] public bool IsUserGenerated { get; set; }
    }
}