using System;
using System.Linq;
using System.Threading.Tasks;
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

        public RelayCommand PageAppearingCommand =>
            _pageAppearingCommand ??= new RelayCommand(async () =>
                await PageAppearingCommandFunction()
            );

        private async Task PageAppearingCommandFunction()
        {
            EventDetail = new EventDetail(SelectedEvent);
            if (EventDetail.TypeName == nameof(NeuEvent))
            {
                var neuStorage = await _eventStorage.GetEventModelStorage<NeuEvent>();
                var neuEvents = await neuStorage.GetAllAsync(EventDetail.Code);
                var groups = neuEvents.Select(e => new {e.Day, e.Week}).GroupBy(e => e.Day, e => e.Week).ToDictionary(g=>g.Key,g=>g.ToList());
               
                foreach (var group in groups)
                {
                    var day = (DayOfWeek) group.Key;
                }
            }

            
        }
    }
}