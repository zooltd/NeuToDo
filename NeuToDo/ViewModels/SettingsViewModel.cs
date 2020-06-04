using System.Collections.Generic;
using System.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models.SettingsModels;

namespace NeuToDo.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel()
        {
            Title = "Neu To Do";
            Settings = new List<SettingItemGroup>
            {
                new SettingItemGroup("关联平台", new List<SettingItem>
                {
                    new SettingItem
                    {
                        Name = "东北大学教务处", Detail = "未绑定", Button1Text = "关联", Button2Text = "解除", Button1Status = true,
                        Button2Status = false
                    },
                    new SettingItem
                    {
                        Name = "中国大学MOOC", Detail = "未绑定", Button1Text = "关联", Button2Text = "解除",
                        Button1Status = false, Button2Status = false
                    },
                    new SettingItem
                    {
                        Name = "东北大学Blackboard", Detail = "未绑定", Button1Text = "关联", Button2Text = "解除",
                        Button1Status = false, Button2Status = false
                    },
                }),
                new SettingItemGroup("同步设置", new List<SettingItem>
                {
                    new SettingItem
                    {
                        Name = "WebDAV", Detail = "未绑定", Button1Text = "关联", Button2Text = "解除", Button1Status = false,
                        Button2Status = false
                    },
                    new SettingItem
                    {
                        Name = "Github", Detail = "未绑定", Button1Text = "关联", Button2Text = "解除", Button1Status = false,
                        Button2Status = false
                    },
                })
            };
        }

        private RelayCommand _testCommand;

        public RelayCommand TestCommand =>
            _testCommand ?? (_testCommand = new RelayCommand((() =>
            {
                Settings[0][0].Name = "NEU";
                Settings = Settings;
            })));


        #region 绑定属性

        private List<SettingItemGroup> _settings;

        public List<SettingItemGroup> Settings
        {
            get => _settings;
            set => Set(nameof(Settings), ref _settings, value);
        }

        private string _title = string.Empty;

        public string Title
        {
            get => _title;
            set => Set(nameof(Title), ref _title, value);
        }

        #endregion
    }
}