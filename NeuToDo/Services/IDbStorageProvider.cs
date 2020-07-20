using NeuToDo.Models;
using System;
using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public interface IDbStorageProvider
    {
        Task CheckInitialization();

        IEventModelStorage<T> GetEventModelStorage<T>() where T : EventModel, new();

        ISemesterStorage GetSemesterStorage();

        Task CloseConnectionAsync();

        event EventHandler UpdateData;

        void OnUpdateData();
    }
}