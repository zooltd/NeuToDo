using GalaSoft.MvvmLight;
using NeuToDo.Models;

namespace NeuToDo.ViewModels
{
    public class EventDetailViewModel : ViewModelBase
    {
        private EventModel _event;

        public EventModel Event
        {
            get => _event;
            set => Set(nameof(Event), ref _event, value);
        }
    }
}