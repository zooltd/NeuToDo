﻿using System;
using System.Threading.Tasks;
using NeuToDo.Services;
using NeuToDo.ViewModels;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Plugin.Calendar.Models;

namespace NeuToDo.Views.Popup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPopupPage : PopupPage
    {
        public LoginPopupPage()
        {
            InitializeComponent();
            // var task = TryGet();
            // task.Wait();
        }

        //TODO 放在viewmodel属性的get里
        // private async Task TryGet()
        // {
        //     try
        //     {
        //         var userName = await SecureStorage.GetAsync("NeuId");
        //         var password = await SecureStorage.GetAsync("NeuPd");
        //         UsernameEntry.Text = userName;
        //         PasswordEntry.Text = password;
        //     }
        //     catch (Exception e)
        //     {
        //         // ignored
        //     }
        // }
        //
        // private async void OnLogin(object sender, EventArgs e)
        // {
        //     var loadingPage = new LoadingPopupPage();
        //     await PopupNavigation.Instance.PushAsync(loadingPage);
        //     var userName = UsernameEntry.Text;
        //     var password = PasswordEntry.Text;
        //     SecureStorage.RemoveAll();
        //     await SecureStorage.SetAsync("NeuId", userName);
        //     await SecureStorage.SetAsync("NeuPd", password);
        //
        //     await new ViewModelLocator().ToDoCalendarViewModel.UpdateEvents();
        //
        //     await PopupNavigation.Instance.RemovePageAsync(loadingPage);
        //     await PopupNavigation.Instance.RemovePageAsync(this);
        //     // await PopupNavigation.Instance.PushAsync(new LoginSuccessPopupPage(), true);
        // }

        protected override void OnAppearingAnimationBegin()
        {
            base.OnAppearingAnimationBegin();

            FrameContainer.HeightRequest = -1;

            if (!IsAnimationEnabled)
            {
                // CloseImage.Rotation = 0;
                // CloseImage.Scale = 1;
                // CloseImage.Opacity = 1;

                LoginButton.Scale = 1;
                LoginButton.Opacity = 1;

                UsernameEntry.TranslationX = PasswordEntry.TranslationX = 0;
                UsernameEntry.Opacity = PasswordEntry.Opacity = 1;

                return;
            }

            // CloseImage.Rotation = 30;
            // CloseImage.Scale = 0.3;
            // CloseImage.Opacity = 0;

            LoginButton.Scale = 0.3;
            LoginButton.Opacity = 0;

            UsernameEntry.TranslationX = PasswordEntry.TranslationX = -10;
            UsernameEntry.Opacity = PasswordEntry.Opacity = 0;
        }

        protected override async Task OnAppearingAnimationEndAsync()
        {
            if (!IsAnimationEnabled)
                return;

            var translateLength = 400u;

            await Task.WhenAll(
                UsernameEntry.TranslateTo(0, 0, easing: Easing.SpringOut, length: translateLength),
                UsernameEntry.FadeTo(1),
                (new Func<Task>(async () =>
                {
                    await Task.Delay(200);
                    await Task.WhenAll(
                        PasswordEntry.TranslateTo(0, 0, easing: Easing.SpringOut, length: translateLength),
                        PasswordEntry.FadeTo(1));
                }))());

            await Task.WhenAll(
                // CloseImage.FadeTo(1),
                // CloseImage.ScaleTo(1, easing: Easing.SpringOut),
                // CloseImage.RotateTo(0),
                LoginButton.ScaleTo(1),
                LoginButton.FadeTo(1));
        }

        protected override async Task OnDisappearingAnimationBeginAsync()
        {
            if (!IsAnimationEnabled)
                return;

            var taskSource = new TaskCompletionSource<bool>();

            var currentHeight = FrameContainer.Height;

            await Task.WhenAll(
                UsernameEntry.FadeTo(0),
                PasswordEntry.FadeTo(0),
                LoginButton.FadeTo(0));

            FrameContainer.Animate("HideAnimation", d => { FrameContainer.HeightRequest = d; },
                start: currentHeight,
                end: 170,
                finished: async (d, b) =>
                {
                    await Task.Delay(300);
                    taskSource.TrySetResult(true);
                });

            await taskSource.Task;
        }

        private void OnCloseButtonTapped(object sender, EventArgs e)
        {
            CloseAllPopup();
        }

        protected override bool OnBackgroundClicked()
        {
            CloseAllPopup();

            return false;
        }

        private async void CloseAllPopup()
        {
            await PopupNavigation.Instance.PopAllAsync();
        }
    }
}