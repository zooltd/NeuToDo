using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using NeuToDo.Components;
using NeuToDo.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NeuToDo.Views.Popup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WeekNoSelectPopupPage : PopupPage
    {
        public WeekNoSelectPopupPage()
        {
            InitializeComponent();
            CollectionView.ItemsSource = Enumerable.Range(1, 24).ToList();
        }

        private bool _isSelectAll, _isSelectOdd, _isSelectEven;

        private void SelectAll(object sender, EventArgs e)
        {
            _isSelectAll = !_isSelectAll;

            foreach (var element in CollectionView.LogicalChildren)
            {
                var customButton = (CustomButton) element;
                customButton.IsClicked = _isSelectAll;
            }
        }

        private void SelectOdd(object sender, EventArgs e)
        {
            _isSelectOdd = !_isSelectOdd;

            for (var i = 0; i < CollectionView.LogicalChildren.Count; i += 2)
            {
                var customButton = (CustomButton) CollectionView.LogicalChildren[i];
                customButton.IsClicked = _isSelectOdd;
            }
        }

        private void SelectEven(object sender, EventArgs e)
        {
            _isSelectEven = !_isSelectEven;

            for (var i = 1; i < CollectionView.LogicalChildren.Count; i += 2)
            {
                var customButton = (CustomButton) CollectionView.LogicalChildren[i];
                customButton.IsClicked = _isSelectEven;
            }
        }


        protected override void OnAppearing()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                //TODO 阻塞
                var cnt = 0;
                while (CollectionView.LogicalChildren.Count < 24 && cnt < 20)
                {
                    Task.Delay(100);
                    cnt++;
                }

                if (CollectionView.LogicalChildren.Count < 24) return;
                if (!(BindingContext is EventDetailViewModel bindingContext)) return;
                foreach (var index in bindingContext.SelectEventGroup.WeekNo)
                {
                    if (CollectionView.LogicalChildren[index - 1] is CustomButton button) button.IsClicked = true;
                }
            });
        }
    }
}