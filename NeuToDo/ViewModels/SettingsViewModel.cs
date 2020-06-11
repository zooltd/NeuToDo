using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models.SettingsModels;
using NeuToDo.Services;
using System.Collections.Generic;
using NeuToDo.Models;

namespace NeuToDo.ViewModels {
    public class SettingsViewModel : ViewModelBase {
        private readonly IPopupNavigationService _popupNavigationService;

        public SettingsViewModel(
            IPopupNavigationService popupNavigationService) {
            _popupNavigationService = popupNavigationService;

            Settings = new List<SettingItemGroup> {
                new SettingItemGroup("关联平台",
                    new List<SettingItem> {
                        new SettingItem {
                            Name = "东北大学教务处",
                            Detail = "未绑定",
                            Button1Text = "关联",
                            Button2Text = "解除",
                            Button1Status = true,
                            Button2Status = false,
                            ServerType = ServerType.Neu,
                            PicUrl =
                                "https://timgsa.baidu.com/timg?image&quality=80&size=b9999_10000&sec=1591467076898&di=177e5dbce6200cba888dff03fcf38b55&imgtype=0&src=http%3A%2F%2Fimg.ccutu.com%2Fupload%2Fimages%2F2017-6%2Fp00027076.png"
                        },
                        new SettingItem {
                            Name = "中国大学MOOC",
                            Detail = "未绑定",
                            Button1Text = "关联",
                            Button2Text = "解除",
                            Button1Status = false,
                            Button2Status = false,
                            ServerType = ServerType.Mooc,
                            PicUrl =
                                "https://timgsa.baidu.com/timg?image&quality=80&size=b9999_10000&sec=1591468918403&di=6959d42be7b1926277d777f76a06ec96&imgtype=0&src=http%3A%2F%2Fa0.att.hudong.com%2F73%2F52%2F20300542724313140971526245341.jpg"
                        },
                        new SettingItem {
                            Name = "东北大学Blackboard",
                            Detail = "未绑定",
                            Button1Text = "关联",
                            Button2Text = "解除",
                            Button1Status = false,
                            Button2Status = false,
                            ServerType = ServerType.Blackboard,
                            PicUrl =
                                "https://timgsa.baidu.com/timg?image&quality=80&size=b9999_10000&sec=1591469006153&di=ec4d28cba4e68adc24286c1ae3eea087&imgtype=0&src=http%3A%2F%2Fwww.jq960.com%2Fuploads%2Fallimg%2Fc170412%2F149200649360020-105162.jpg"
                        },
                    }),
                new SettingItemGroup("同步设置",
                    new List<SettingItem> {
                        new SettingItem {
                            Name = "WebDAV",
                            Detail = "未绑定",
                            Button1Text = "关联",
                            Button2Text = "解除",
                            Button1Status = false,
                            Button2Status = false,
                            ServerType = ServerType.WebDav
                        },
                        new SettingItem {
                            Name = "Github",
                            Detail = "未绑定",
                            Button1Text = "关联",
                            Button2Text = "解除",
                            Button1Status = false,
                            Button2Status = false,
                            ServerType = ServerType.Github
                        },
                    })
            };
        }


        #region 绑定方法

        private RelayCommand _testCommand;

        public RelayCommand TestCommand =>
            _testCommand ?? (_testCommand = new RelayCommand((() => { })));

        // private RelayCommand<ServerType> _command1;
        //
        // public RelayCommand<ServerType> Command1 =>
        //     _command1 ?? (_command1 = new RelayCommand<ServerType>((async s =>
        //     {
        //         await _navigationService.NavigateToPopupPageAsync(PopupPageNavigationConstants.LoginPopupPage, s);
        //     })));

        private RelayCommand<SettingItem> _command1;

        public RelayCommand<SettingItem> Command1 =>
            _command1 ?? (_command1 = new RelayCommand<SettingItem>((item) => {
                _popupNavigationService.PushAsync(
                    PopupPageNavigationConstants.LoginPopupPage, item);
            }));

        private RelayCommand _command2;

        public RelayCommand Command2 =>
            _command2 ?? (_command2 = new RelayCommand(() => { }));

        #endregion

        #region 绑定属性

        private List<SettingItemGroup> _settings;

        public List<SettingItemGroup> Settings {
            get => _settings;
            set => Set(nameof(Settings), ref _settings, value);
        }

        private string _title = string.Empty;

        public string Title {
            get => _title;
            set => Set(nameof(Title), ref _title, value);
        }

        #endregion
    }
}