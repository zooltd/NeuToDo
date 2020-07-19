using System.Collections.Generic;
using System.Threading.Tasks;
using NeuToDo.Models;
using Plugin.Calendars.Abstractions;

namespace NeuToDo.Services
{
    public interface ICalendarStorageProvider
    {
        /// <summary>
        /// 检查日历权限。
        /// </summary>
        /// <returns></returns>
        Task<bool> CheckPermissionsAsync();

        /// <summary>
        /// 删除所有该名称的日历
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="calendarName"></param>
        /// <returns></returns>
        Task DeleteCalendarsAsync(string accountName, string calendarName);

        Task AddCalendarAsync(string accountName, string calendarName);

        Task<Calendar> GetCalendarsAsync(string calendarName);

        IList<CalendarEvent> ToDoEventsToCalenderEvents(List<EventModel> eventModels);

        Task AddOrUpdateEventAsync(Calendar myCalendar, CalendarEvent calendarEvent);
    }
}