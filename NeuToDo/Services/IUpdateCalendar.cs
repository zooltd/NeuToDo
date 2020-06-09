using System;
using System.Threading.Tasks;
using Xamarin.Plugin.Calendar.Models;

namespace NeuToDo.Services
{
    public interface IUpdateCalendar
    {
        event EventHandler GotData;
        void OnGotData();
        Task<EventCollection> GetData();
    }
}