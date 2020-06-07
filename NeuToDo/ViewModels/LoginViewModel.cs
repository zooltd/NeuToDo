using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using NeuToDo.Models.SettingsModels;
using NeuToDo.Services;
using Rg.Plugins.Popup.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace NeuToDo.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private ILoginService _loginService;
        private readonly ServerType _type;

        [PreferredConstructor]
        public LoginViewModel()
        {
            FetchSecureStorage();
        }

        public LoginViewModel(ServerType type)
        {
            _type = type;
            FetchSecureStorage();
            SetProperties(type);
        }


        private void FetchSecureStorage()
        {
            var getUserName = TryGetUserName();
            getUserName.Wait();
            var getPassword = TryGetPassword();
            getPassword.Wait();
        }

        private void SetProperties(ServerType type)
        {
            switch (type)
            {
                case ServerType.Neu:
                    PicUrl =
                        "https://timgsa.baidu.com/timg?image&quality=80&size=b9999_10000&sec=1591467076898&di=177e5dbce6200cba888dff03fcf38b55&imgtype=0&src=http%3A%2F%2Fimg.ccutu.com%2Fupload%2Fimages%2F2017-6%2Fp00027076.png";
                    _loginService = new NeuLoginService();
                    break;
                case ServerType.Mooc:
                    PicUrl =
                        "https://timgsa.baidu.com/timg?image&quality=80&size=b9999_10000&sec=1591468918403&di=6959d42be7b1926277d777f76a06ec96&imgtype=0&src=http%3A%2F%2Fa0.att.hudong.com%2F73%2F52%2F20300542724313140971526245341.jpg";
                    _loginService = new MoocLoginService();
                    break;
                case ServerType.Blackboard:
                    PicUrl =
                        "https://timgsa.baidu.com/timg?image&quality=80&size=b9999_10000&sec=1591469006153&di=ec4d28cba4e68adc24286c1ae3eea087&imgtype=0&src=http%3A%2F%2Fwww.jq960.com%2Fuploads%2Fallimg%2Fc170412%2F149200649360020-105162.jpg";
                    _loginService = new BbLoginService();
                    break;
            }
        }


        private async Task UpdateSecureStorage()
        {
            try
            {
                await SecureStorage.SetAsync(_type + "Id", UserName);
                await SecureStorage.SetAsync(_type + "Pd", Password);
            }
            catch (Exception e)
            {
                //ignored
            }
        }


        #region 绑定方法

        private RelayCommand _onLogin;

        public RelayCommand OnLogin => _onLogin ?? (_onLogin = new RelayCommand((async () =>
        {
            await PopupPageNavigationService.Loading();
            // var getter = new NeuSyllabusGetter(UserName, Password);
            // await getter.WebCrawler();

            var loginTask = _loginService.LoginTask(UserName, Password);
            var res = await loginTask;

            //TODO FIXME
            // SimpleIoc.Default.GetInstance<ToDoCalendarViewModel>().UpdateEvents(); //TEST


            if (res)
            {
                await PopupPageNavigationService.SuccessMessage();
                await UpdateSecureStorage();
            }
            else
            {
                await PopupPageNavigationService.ErrorMessage();
            }

            await Task.Delay(1500);

            await PopupNavigation.Instance.PopAllAsync();
        })));

        #endregion

        #region 绑定属性

        private string _picUrl;

        public string PicUrl
        {
            get => _picUrl;
            set => Set(nameof(PicUrl), ref _picUrl, value);
        }

        private static string _userName;

        public string UserName
        {
            get => _userName;
            set => Set(nameof(UserName), ref _userName, value);
        }


        private string _password;

        public string Password
        {
            get => _password;
            set => Set(nameof(Password), ref _password, value);
        }

        #endregion

        private async Task TryGetUserName()
        {
            try
            {
                UserName = await SecureStorage.GetAsync(_type + "Id");
            }
            catch (Exception e)
            {
                // ignored
                UserName = string.Empty;
            }
        }

        private async Task TryGetPassword()
        {
            try
            {
                Password = await SecureStorage.GetAsync(_type + "Pd");
            }
            catch (Exception e)
            {
                // ignored
                Password = string.Empty;
            }
        }
    }
}