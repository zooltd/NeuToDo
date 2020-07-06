using System;
using System.Collections.Generic;

namespace NeuToDo.Models.SettingsModels
{
    public class SettingItemGroup : List<Platform>
    {
        public string Name { get; private set; }

        public SettingItemGroup(string name, List<Platform> items) : base(items)
        {
            Name = name;
        }

        public static List<SettingItemGroup> SettingGroup { get; } = new List<SettingItemGroup>
        {
            new SettingItemGroup("关联平台",
                new List<Platform>
                {
                    new Platform
                    {
                        Name = "东北大学教务处",
                        UserName = string.Empty,
                        LastUpdateTime = string.Empty,
                        Button1Text = "关联",
                        IsBound = false,
                        ServerType = ServerType.Neu,
                        PicUrl = "NeuPic.png"
                    },
                    new Platform
                    {
                        Name = "中国大学MOOC",
                        UserName = string.Empty,
                        LastUpdateTime = string.Empty,
                        Button1Text = "关联",
                        IsBound = false,
                        ServerType = ServerType.Mooc,
                        PicUrl = "MoocPic.png"
                    },
                    new Platform
                    {
                        Name = "东北大学Blackboard",
                        UserName = string.Empty,
                        LastUpdateTime = string.Empty,
                        Button1Text = "关联",
                        IsBound = false,
                        ServerType = ServerType.Bb,
                        PicUrl = "BbPic.png"
                    },
                }),
            new SettingItemGroup("同步设置",
                new List<Platform>
                {
                    new Platform
                    {
                        Name = "WebDAV",
                        UserName = string.Empty,
                        LastUpdateTime = string.Empty,
                        Button1Text = "关联",
                        IsBound = false,
                        ServerType = ServerType.WebDav,
                        PicUrl = "CloudStorage.png"
                    }
                })
        };
    }
}