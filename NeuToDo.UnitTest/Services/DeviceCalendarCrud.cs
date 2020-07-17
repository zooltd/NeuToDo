using System.Threading.Tasks;
using NUnit.Framework;
using Plugin.Calendars;
using Plugin.Calendars.Abstractions;
using Xamarin.Forms;

namespace NeuToDo.UnitTest.Services
{
    public class DeviceCalendarCrud
    {
        public async Task Test()
        {
            var calendars = await CrossCalendars.Current.GetCalendarsAsync();
            await CrossCalendars.Current.AddOrUpdateCalendarAsync(new Calendar
                {AccountName = "Device", Color = "#BF4779", Name = "NeuToDo"});
        }
    }
}