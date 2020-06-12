using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NeuToDo.Services;
using NeuToDo.ViewModels;
using NUnit.Framework;

namespace NeuToDo.UnitTest.ViewModels {
    [TestFixture]
    public class SettingsViewModelTest {
        [Test]
        public async Task TestCommand1() {
            var popupNavigationMock = new Mock<IPopupNavigationService>();
            var mockPopupNavigation = popupNavigationMock.Object;

            var settingsViewModel = new SettingsViewModel(mockPopupNavigation);
        }
    }
}