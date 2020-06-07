using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models.SettingsModels;
using NeuToDo.Services;
using System.Collections.Generic;

namespace NeuToDo.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        public PopupPageNavigationService PopupService { get; } = new PopupPageNavigationService();
        
        public SettingsViewModel()
        {
            Title = "Neu To Do";
            Settings = new List<SettingItemGroup>
            {
                new SettingItemGroup("关联平台", new List<SettingItem>
                {
                    new SettingItem
                    {
                        Name = "东北大学教务处", Detail = "未绑定", Button1Text = "关联", Button2Text = "解除",
                        Button1Status = true, Button2Status = false, ServerType = ServerType.Neu
                    },
                    new SettingItem
                    {
                        Name = "中国大学MOOC", Detail = "未绑定", Button1Text = "关联", Button2Text = "解除",
                        Button1Status = false, Button2Status = false, ServerType = ServerType.Mooc
                    },
                    new SettingItem
                    {
                        Name = "东北大学Blackboard", Detail = "未绑定", Button1Text = "关联", Button2Text = "解除",
                        Button1Status = false, Button2Status = false, ServerType = ServerType.Blackboard
                    },
                }),
                new SettingItemGroup("同步设置", new List<SettingItem>
                {
                    new SettingItem
                    {
                        Name = "WebDAV", Detail = "未绑定", Button1Text = "关联", Button2Text = "解除",
                        Button1Status = false, Button2Status = false, ServerType = ServerType.WebDav
                    },
                    new SettingItem
                    {
                        Name = "Github", Detail = "未绑定", Button1Text = "关联", Button2Text = "解除",
                        Button1Status = false, Button2Status = false, ServerType = ServerType.Github
                    },
                })
            };
        }


        #region 绑定方法

        private RelayCommand _testCommand;

        public RelayCommand TestCommand =>
            _testCommand ?? (_testCommand = new RelayCommand((() => { })));

        private RelayCommand<ServerType> _command1;

        public RelayCommand<ServerType> Command1 =>
            _command1 ?? (_command1 = new RelayCommand<ServerType>((async s =>
            {
                await PopupService.Login(s);
            })));

        #endregion

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