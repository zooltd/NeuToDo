using System.Collections.ObjectModel;

namespace NeuToDo.Models
{
    public class NeuEventWrapper : NeuEvent
    {
        public ObservableCollection<NeuEventPeriod> EventPeriods { get; set; }

        public Semester EventSemester { get; set; }

        public NeuEventWrapper(NeuEvent neuEvent) : base(neuEvent)
        {
            EventPeriods = new ObservableCollection<NeuEventPeriod>();
        }
    }
}