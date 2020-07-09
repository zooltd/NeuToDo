using SQLite;

namespace NeuToDo.Models
{
    [Table(nameof(UserEvent))]
    public class UserEvent : EventModel
    {
        [Column(nameof(IsRepeat))] public bool IsRepeat { get; set; }
    }
}