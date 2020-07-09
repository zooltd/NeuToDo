using NeuToDo.Models;
using System;
using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public interface IDbStorageProvider
    {
        bool IsInitialized { get; set; }

        Task CheckInitialization();

        IEventModelStorage<T> GetEventModelStorage<T>() where T : EventModel, new();

        ISemesterStorage GetSemesterStorage();

        Task CloseConnectionAsync();

        //TODO 抽离成新的Service?
        public event EventHandler UpdateData;

        public void OnUpdateData();
    }
}