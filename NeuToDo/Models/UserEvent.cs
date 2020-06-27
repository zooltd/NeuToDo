using SQLite;

namespace NeuToDo.Models
{
    [SQLite.Table("UserEvent")]
    public class UserEvent : EventModel
    {
        [SQLite.Column("is_repeat")]
        public bool IsRepeat { get; set; }
    }
}