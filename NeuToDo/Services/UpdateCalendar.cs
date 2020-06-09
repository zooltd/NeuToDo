using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NeuToDo.Models;
using Xamarin.Plugin.Calendar.Models;

namespace NeuToDo.Services
{
    // public class UpdateCalendar : IUpdateCalendar
    // {
    //     public event EventHandler GotData;
    //
    //     private readonly CourseEventStorage _courseEventStorage = new CourseEventStorage();
    //
    //     public virtual void OnGotData()
    //     {
    //         GotData?.Invoke(this, EventArgs.Empty);
    //     }
    //
    //     /// <summary>
    //     /// GroupBy OR LINQ
    //     /// </summary>
    //     /// <returns></returns>
    //     public async Task<EventCollection> GetData()
    //     {
    //         if (!_courseEventStorage.DbExist())
    //         {
    //             return null;
    //         }
    //
    //         try
    //         {
    //             var events = new EventCollection();
    //
    //             var courseEventList = await _courseEventStorage.GetAll();
    //
    //             foreach (var e in courseEventList)
    //             {
    //                 var dateOfEvent = e.Starting.Date;
    //                 if (events.ContainsKey(dateOfEvent))
    //                 {
    //                     events[dateOfEvent] = new ArrayList(events[dateOfEvent]) {e};
    //                 }
    //                 else
    //                 {
    //                     events.Add(dateOfEvent, new List<EventModel> {e});
    //                 }
    //             }
    //
    //             return events;
    //         }
    //         catch (Exception e)
    //         {
    //             return null;
    //         }
    //     }
    // }
}