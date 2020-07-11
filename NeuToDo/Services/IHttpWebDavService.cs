using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public interface IHttpWebDavService
    {
        void Initiate(Account account);

        Task<bool> TestConnection();
    }
}