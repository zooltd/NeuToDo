using System;
using NeuToDo.Models;
using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public interface IStorageProvider
    {
        Task<IEventModelStorage<T>> GetEventModelStorage<T>() where T : EventModel, new();

        Task<ISemesterStorage> GetSemesterStorage();

        Task CloseConnectionAsync();

        //TODO 抽离成新的Service
        public event EventHandler UpdateData;
        public void OnUpdateData();
    }
}