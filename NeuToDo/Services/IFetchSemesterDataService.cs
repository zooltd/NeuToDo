using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public interface IFetchSemesterDataService
    {
        Task FetchSemesterAsync();
    }
}