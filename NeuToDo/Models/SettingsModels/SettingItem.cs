using GalaSoft.MvvmLight;
using Xamarin.Forms;

namespace NeuToDo.Models.SettingsModels
{
    public class SettingItem : ObservableObject
    {
        private string _name;

        public string Name
        {
            get => _name;
            set => Set(nameof(Name), ref _name, value);
        }

        private string _detail;

        public string Detail
        {
            get => _detail;
            set => Set(nameof(Detail), ref _detail, value);
        }

        private string _button1Text;

        public string Button1Text
        {
            get => _button1Text;
            set => Set(nameof(Button1Text), ref _button1Text, value);
        }

        private bool _isBound;

        public bool IsBound
        {
            get => _isBound;
            set => Set(nameof(IsBound), ref _isBound, value);
        }

        private ServerType _serverType;

        public ServerType ServerType
        {
            get => _serverType;
            set => Set(nameof(ServerType), ref _serverType, value);
        }

        private string _picUrl;

        public string PicUrl
        {
            get => _picUrl;
            set => Set(nameof(PicUrl), ref _picUrl, value);
        }
    }
}