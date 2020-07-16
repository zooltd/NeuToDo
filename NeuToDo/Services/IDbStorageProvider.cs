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

        public event EventHandler UpdateData;

        public void OnUpdateData();
    }
}