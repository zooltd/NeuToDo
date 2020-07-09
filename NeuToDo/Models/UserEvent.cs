using SQLite;

namespace NeuToDo.Models
{
    [Table(nameof(UserEvent))]
    public class UserEvent : EventModel
    {
        [Column(nameof(IsRepeat))] public bool IsRepeat { get; set; }

        public UserEvent()
        {
        }

        protected UserEvent(UserEvent userEvent)
        {
            Id = userEvent.Id;
            Code = userEvent.Code;
            Detail = userEvent.Detail;
            Title = userEvent.Title;
            Time = userEvent.Time;
            IsDone = userEvent.IsDone;
            IsRepeat = userEvent.IsRepeat;
        }
    }
}