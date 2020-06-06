using System;
using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public interface IxxxService
    {
        Task GetDataAsync();

        event EventHandler GotData;

        int GetData();
    }
}