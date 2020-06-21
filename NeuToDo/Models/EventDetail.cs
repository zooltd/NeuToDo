namespace NeuToDo.Models
{
    public class EventDetail
    {
        public EventModel Event;

        public string TypeName;

        public bool CanRepeat;

        public bool ShowSwitchCell;

        public bool IsRepeat;

        public EventDetail(EventModel e)
        {
            Event = e;
            TypeName = e.GetType().Name;
            switch (TypeName)
            {
                case nameof(NeuEvent):
                    CanRepeat = true;
                    ShowSwitchCell = false;
                    IsRepeat = true;
                    break;
                case nameof(MoocEvent):
                    CanRepeat = ShowSwitchCell = false;
                    IsRepeat = false;
                    break;
                case nameof(UserEvent):
                    CanRepeat = ShowSwitchCell = true;
                    IsRepeat = false;
                    break;
            }
        }
    }
}