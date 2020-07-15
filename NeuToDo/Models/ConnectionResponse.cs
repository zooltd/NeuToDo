using GalaSoft.MvvmLight;

namespace NeuToDo.Models
{
    public class ConnectionResponse : ObservableObject
    {
        private bool _isConnected;

        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                _isConnected = value;
                PictureSource = IsConnected ? "success.png" : "failure.png";
            }
        }

        private string _reason;

        public string Reason
        {
            get => _reason;
            set => Set(nameof(Reason), ref _reason, value);
        }

        private string _pictureSource;

        public string PictureSource
        {
            get => _pictureSource;
            set => Set(nameof(PictureSource), ref _pictureSource, value);
        }
    }
}