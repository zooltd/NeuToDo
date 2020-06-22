using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models;
using NeuToDo.Services;

namespace NeuToDo.ViewModels
{
    public class EventDetailViewModel : ViewModelBase
    {
        private IEventModelStorageProvider _eventStorage;

        public EventDetailViewModel(IEventModelStorageProvider eventModelStorageProvider)
        {
            _eventStorage = eventModelStorageProvider;
        }

        private EventModel _selectedEvent;

        public EventModel SelectedEvent
        {
            get => _selectedEvent;
            set => Set(nameof(SelectedEvent), ref _selectedEvent, value);
        }

        private EventDetail _eventDetail;

        public EventDetail EventDetail
        {
            get => _eventDetail;
            set => Set(nameof(EventDetail), ref _eventDetail, value);
        }


        private RelayCommand _pageAppearingCommand;
        public RelayCommand PageAppearingCommand => _pageAppearingCommand ??= new RelayCommand(PageAppearingCommandFunction);

        private void PageAppearingCommandFunction()
        {
            EventDetail = new EventDetail(SelectedEvent);

        }
    }
}