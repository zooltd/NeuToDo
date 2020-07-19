using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeuToDo.Models;
using Plugin.Calendars;
using Plugin.Calendars.Abstractions;
using Xamarin.Essentials;

namespace NeuToDo.Services
{
    public class CalendarStorageProvider : ICalendarStorageProvider
    {
        public async Task<bool> CheckPermissionsAsync()
        {
            var writeStatus = await Permissions.CheckStatusAsync<Permissions.CalendarWrite>();
            if (writeStatus != PermissionStatus.Granted)
                writeStatus = await Permissions.RequestAsync<Permissions.CalendarWrite>();

            var readStatus = await Permissions.CheckStatusAsync<Permissions.CalendarRead>();
            if (readStatus != PermissionStatus.Granted)
                readStatus = await Permissions.RequestAsync<Permissions.CalendarRead>();

            return readStatus == PermissionStatus.Granted && writeStatus == PermissionStatus.Granted;
        }

        public async Task DeleteCalendarsAsync(string accountName, string calendarName)
        {
            var deviceCalendars = await CrossCalendars.Current.GetCalendarsAsync();

            foreach (var deviceCalendar in deviceCalendars)
                if (deviceCalendar.AccountName == accountName && deviceCalendar.Name == calendarName)
                    await CrossCalendars.Current.DeleteCalendarAsync(deviceCalendar);
        }

        public async Task AddCalendarAsync(string accountName, string calendarName)
        {
            await CrossCalendars.Current.AddOrUpdateCalendarAsync(new Calendar
                {AccountName = accountName, Color = "#BF4779", Name = calendarName});
        }

        public async Task<Calendar> GetCalendarsAsync(string calendarName)
        {
            var deviceCalendars = await CrossCalendars.Current.GetCalendarsAsync();

            var myCalendar = deviceCalendars.FirstOrDefault(x => x.Name == "NeuToDo");

            return myCalendar;
        }

        public IList<CalendarEvent> ToDoEventsToCalenderEvents(List<EventModel> eventModels)
            => eventModels.ConvertAll(x => new CalendarEvent
            {
                Name = x.Title, Description = x.Detail, Start = x.Time, End = x.Time,
                Reminders = new List<CalendarEventReminder>()
            });

        public async Task AddOrUpdateEventAsync(Calendar myCalendar, CalendarEvent calendarEvent) 
            => await CrossCalendars.Current.AddOrUpdateEventAsync(myCalendar, calendarEvent);
    }
}