using System.Threading.Tasks;
using NeuToDo.Models;

namespace NeuToDo.Services
{
    public interface ILoginServiceProvider
    {
        Task<ILoginService> GetLoginService(ServerType serverType);
    }
}