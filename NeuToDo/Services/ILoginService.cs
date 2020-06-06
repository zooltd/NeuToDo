using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public interface ILoginService
    {
        Task<bool> LoginTask(string userId, string password);

    }
}