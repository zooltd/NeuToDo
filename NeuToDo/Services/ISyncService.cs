using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public interface ISyncService
    {
        Task SyncSyllabusAsync();
        Task SyncEventModelsAsync(string remoteFilePath);
    }
}