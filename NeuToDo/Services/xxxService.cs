using System;
using System.Threading.Tasks;

namespace NeuToDo.Services {
    public class xxxService : IxxxService {
        public async Task GetDataAsync() {
            // NEUSy.....
            // Save Neu sya.....
            GotData?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler GotData;
        public int GetData()
        {
            throw new NotImplementedException();
        }
    }
}