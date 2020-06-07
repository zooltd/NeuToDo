using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NeuToDo.Models;

namespace NeuToDo.Services
{
    public interface IEventStorage
    {
        Task CreateDatabase();

        Task ClearDatabase();

        Task Insert(EventModel e);

        Task InsertAll(IList<EventModel> eventList);

        bool IsInitialized();

    }
}