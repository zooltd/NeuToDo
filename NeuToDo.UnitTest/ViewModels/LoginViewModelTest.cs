using System.Threading.Tasks;
using Moq;
using NeuToDo.Models;
using NeuToDo.Services;
using NeuToDo.ViewModels;
using NUnit.Framework;

namespace NeuToDo.UnitTest.ViewModels
{
    public class LoginViewModelTest
    {
        [Test,Ignore("")]
        public async Task TestPageAppearingCommand()
        {
            var popupNavigationService = new Mock<IPopupNavigationService>();
            var loginAndFetchDataService = new Mock<ILoginAndFetchDataService>();
            var secureStorageProvider = new Mock<ISecureStorageProvider>();
            var mockPopupNavigationService = popupNavigationService.Object;
            var mockLoginAndFetchDataService = loginAndFetchDataService.Object;
            var mockSecureStorageProvider = secureStorageProvider.Object;
            var loginViewModel = new LoginViewModel(mockPopupNavigationService, mockLoginAndFetchDataService,
                mockSecureStorageProvider) {SettingItem = {ServerType = ServerType.Neu}};
            await loginViewModel.PageAppearingCommandFunction();
            secureStorageProvider.Verify(s=>s.GetAsync(loginViewModel.SettingItem.ServerType+"id"),Times.Once);
        }
    }
}