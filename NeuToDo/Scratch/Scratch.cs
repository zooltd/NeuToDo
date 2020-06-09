using System.Threading.Tasks;
using SQLite;

namespace NeuToDo.Scratch
{
    /// <summary>
    /// Just Scratch
    /// </summary>
    public class NewEventModel
    {
        [PrimaryKey, AutoIncrement] public int Id { get; set; }
    }

    public class NewNeuEventModel : NewEventModel
    {
    }

    public interface INewEventModelStorage<T> where T : NewEventModel
    {
        Task InsertAsync(T t);
    }

    public class NewEventModelStorage<T> : INewEventModelStorage<T>
        where T : NewEventModel
    {
        private SQLiteAsyncConnection _connection;

        public async Task InsertAsync(T t)
        {
            await _connection.InsertAsync(t);
        }
    }

    public interface IEventStorageProvider
    {
        INewEventModelStorage<NewNeuEventModel> GetNeuEventModelStorage();
    }

    public class EventStorageProvider : IEventStorageProvider
    {
        public INewEventModelStorage<NewNeuEventModel> GetNeuEventModelStorage()
        {
            return new NewEventModelStorage<NewNeuEventModel>();
        }
    }

    public class NeuLogin
    {
        private INewEventModelStorage<NewNeuEventModel> _eventModelStorage;

        public NeuLogin(IEventStorageProvider eventStorageProvider)
        {
            _eventModelStorage = eventStorageProvider.GetNeuEventModelStorage();
        }

        public async Task DoSomehingAsync()
        {
            await _eventModelStorage.InsertAsync(new NewNeuEventModel());
        }
    }
}