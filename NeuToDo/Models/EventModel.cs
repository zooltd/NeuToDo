using System;
using System.Threading.Tasks;
using NeuToDo.Models.SettingsModels;
using SQLite;

namespace NeuToDo.Models {
    public class NewEventModel {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
    }

    public class NewNeuEventModel : NewEventModel {
    }

    public interface INewEventModelStorage<T> where T : NewEventModel {
        Task InsertAsync(T t);
    }

    public class NewEventModelStorage<T> : INewEventModelStorage<T>
        where T : NewEventModel {
        private SQLiteAsyncConnection _connection;

        public async Task InsertAsync(T t) {
            await _connection.InsertAsync(t);
        }
    }

    public interface IEventStorageProvider {
        INewEventModelStorage<NewNeuEventModel> GetNeuEventModelStorage();
    }

    public class EventStorageProvider : IEventStorageProvider {
        public INewEventModelStorage<NewNeuEventModel> GetNeuEventModelStorage() {
           return new NewEventModelStorage<NewNeuEventModel>();
        }
    }

    public class NeuLogin {

        private INewEventModelStorage<NewNeuEventModel> _eventModelStorage;

        public NeuLogin(IEventStorageProvider eventStorageProvider) {
            _eventModelStorage = eventStorageProvider.GetNeuEventModelStorage();
        }

        public async Task DoSomehingAsync() {
            await _eventModelStorage.InsertAsync(new NewNeuEventModel());
        }
    }











    [SQLite.Table("courses")]
    public class EventModel
    {
        [PrimaryKey, AutoIncrement]
        [SQLite.Column("id")]
        public int Id { get; set; }

        [SQLite.Column("title")] 
        public string Title { get; set; }

        [SQLite.Column("detail")] 
        public string Detail { get; set; }

        [SQLite.Column("starting")] 
        public DateTime Starting { get; set; }

        /// <summary>
        /// 是否已完成
        /// </summary>
        [SQLite.Column("is_done")]
        public bool IsDone { get; set; }

        [SQLite.Column("code")] 
        public string Code { get; set; }
    }
}