using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public interface ILoginService
    {
        Task<bool> LoginAndFetchDataAsync(string userId, string password);

    }
}