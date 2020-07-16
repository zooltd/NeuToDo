using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace NeuToDo.Models
{
    public class ServerPlatform
    {
        public string Name { get; set; }


        public string UserName { get; set; }


        public string LastUpdateTime { get; set; }


        public string Button1Text { get; set; }


        public bool IsBound { get; set; }


        public ServerType ServerType { get; set; }


        public string PicUrl { get; set; }

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
            }
        };
    }
}