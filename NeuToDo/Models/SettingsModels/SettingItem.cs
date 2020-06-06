using GalaSoft.MvvmLight;

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

        private string _button2Text;

        public string Button2Text
        {
            get => _button2Text;
            set => Set(nameof(Button2Text), ref _button2Text, value);
        }

        private bool _button1Status;

        public bool Button1Status
        {
            get => _button1Status;
            set => Set(nameof(Button1Status), ref _button1Status, value);
        }

        private bool _button2Status;

        public bool Button2Status
        {
            get => _button2Status;
            set => Set(nameof(Button2Status), ref _button2Status, value);
        }

        private ServerType _serverType;

        public ServerType ServerType
        {
            get => _serverType;
            set => Set(nameof(ServerType), ref _serverType, value);
        }

    }

    public enum ServerType
    {
        Neu,
        Mooc,
        Blackboard,
        WebDav,
        Github
    }
}