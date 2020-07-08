using GalaSoft.MvvmLight;
using System.Collections.Generic;

namespace NeuToDo.Models.SettingsModels
{
    public class ServerPlatform : ObservableObject
    {
        private string _name;

        public string Name
        {
            get => _name;
            set => Set(nameof(Name), ref _name, value);
        }

        private string _userName;

        public string UserName
        {
            get => _userName;
            set => Set(nameof(UserName), ref _userName, value);
        }

        private string _lastUpdateTime;

        public string LastUpdateTime
        {
            get => _lastUpdateTime;
            set => Set(nameof(LastUpdateTime), ref _lastUpdateTime, value);
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

        public static List<ServerPlatform> ServerPlatforms { get; } = new List<ServerPlatform>
        {
            new ServerPlatform
            {
                Name = "东北大学教务处",
                UserName = string.Empty,
                LastUpdateTime = string.Empty,
                Button1Text = "关联",
                IsBound = false,
                ServerType = ServerType.Neu,
                PicUrl = "NeuPic.png"
            },
            new ServerPlatform
            {
                Name = "中国大学MOOC",
                UserName = string.Empty,
                LastUpdateTime = string.Empty,
                Button1Text = "关联",
                IsBound = false,
                ServerType = ServerType.Mooc,
                PicUrl = "MoocPic.png"
            },
            new ServerPlatform
            {
                Name = "东北大学Blackboard",
                UserName = string.Empty,
                LastUpdateTime = string.Empty,
                Button1Text = "关联",
                IsBound = false,
                ServerType = ServerType.Bb,
                PicUrl = "BbPic.png"
            }
        };
    }
}