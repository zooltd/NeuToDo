using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using NeuToDo.Models;
using NeuToDo.Models.SettingsModels;
using NeuToDo.Services;
using System.Collections.Generic;

namespace NeuToDo.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly IPopupNavigationService _popupNavigationService;

        public SettingsViewModel(IPopupNavigationService popupNavigationService)
        {
            _popupNavigationService = popupNavigationService;

            Settings = SettingItemGroup.SettingGroup;
        }


        #region 绑定方法

        private RelayCommand<SettingItem> _command1;

        public RelayCommand<SettingItem> Command1 =>
            _command1 ??= new RelayCommand<SettingItem>(Command1Function);

        public void Command1Function(SettingItem item)
        {
            _popupNavigationService.PushAsync(PopupPageNavigationConstants.LoginPopupPage, item);
        }

        private RelayCommand _command2;

        public RelayCommand Command2 =>
            _command2 ??= new RelayCommand(() => { });

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