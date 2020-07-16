using System;
using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public interface ISyncService
    {
        Task SyncEventModelsAsync(string remoteFilePath);
    }
}