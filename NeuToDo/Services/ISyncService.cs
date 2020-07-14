using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public interface ISyncService
    {
        Task SyncAsync(string remoteFilePath);
    }
}