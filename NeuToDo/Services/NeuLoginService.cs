using System;
using System.Linq;
using System.Threading.Tasks;
using NeuToDo.Models;

namespace NeuToDo.Services
{
    public class NeuLoginService : ILoginService
    {
        public IEventStorage StorageService { get; private set; } = new CourseEventStorage();

        public event EventHandler UpdateData;

        public async Task<bool> LoginTask(string userId, string password)
        {
            //ToDO do something
            var getter = new NeuSyllabusGetter(userId, password);
            try
            {
                await getter.WebCrawler();
                await StorageService.CreateDatabase();
                await StorageService.ClearDatabase();
                await StorageService.InsertAll(NeuSyllabusGetter.EventList.ConvertAll(x => (EventModel)x));


                return true;
            }
            catch (Exception e)
            {
                return false;
                // ignored
            }
        }

        protected virtual void OnUpdateData()
        {
            UpdateData?.Invoke(this, EventArgs.Empty);
        }
    }
}