using System;
using System.Collections.Generic;

namespace NeuToDo.Models.SettingsModels
{
    public class SettingItemGroup : List<SettingItem>
    {
        public string Name { get; private set; }

        public SettingItemGroup(string name, List<SettingItem> items) : base(items)
        {
            Name = name;
        }

        public static List<SettingItemGroup> SettingGroup { get; } = new List<SettingItemGroup>
        {
            new SettingItemGroup("关联平台",
                new List<SettingItem>
                {
                    new SettingItem
                    {
                        Name = "东北大学教务处",
                        UserName = string.Empty,
                        LastUpdateTime = string.Empty,
                        Button1Text = "关联",
                        IsBound = false,
                        ServerType = ServerType.Neu,
                        PicUrl =
                            "https://timgsa.baidu.com/timg?image&quality=80&size=b9999_10000&sec=1591467076898&di=177e5dbce6200cba888dff03fcf38b55&imgtype=0&src=http%3A%2F%2Fimg.ccutu.com%2Fupload%2Fimages%2F2017-6%2Fp00027076.png"
                    },
                    new SettingItem
                    {
                        Name = "中国大学MOOC",
                        UserName = string.Empty,
                        LastUpdateTime = string.Empty,
                        Button1Text = "关联",
                        IsBound = false,
                        ServerType = ServerType.Mooc,
                        PicUrl =
                            "https://timgsa.baidu.com/timg?image&quality=80&size=b9999_10000&sec=1591468918403&di=6959d42be7b1926277d777f76a06ec96&imgtype=0&src=http%3A%2F%2Fa0.att.hudong.com%2F73%2F52%2F20300542724313140971526245341.jpg"
                    },
                    new SettingItem
                    {
                        Name = "东北大学Blackboard",
                        UserName = string.Empty,
                        LastUpdateTime = string.Empty,
                        Button1Text = "关联",
                        IsBound = false,
                        ServerType = ServerType.Bb,
                        PicUrl =
                            "http://5b0988e595225.cdn.sohucs.com/images/20171020/ce9240ffb60c40678b24489e366ead24.jpeg"
                    },
                }),
            new SettingItemGroup("同步设置",
                new List<SettingItem>
                {
                    new SettingItem
                    {
                        Name = "WebDAV",
                        UserName = string.Empty,
                        LastUpdateTime = string.Empty,
                        Button1Text = "关联",
                        IsBound = false,
                        ServerType = ServerType.WebDav
                    }
                })
        };
    }
}