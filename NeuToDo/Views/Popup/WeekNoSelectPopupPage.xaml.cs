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
        private readonly EventDetailViewModel _bindingContext = SimpleIoc.Default.GetInstance<EventDetailViewModel>();

        private readonly List<int> _selectedIndex = new List<int>();

        public WeekNoSelectPopupPage()
        {
            InitializeComponent();
            BindingContext = _bindingContext;
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

        private async void SelectCancel(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.RemovePageAsync(this);
        }

        private async void SelectDone(object sender, EventArgs e)
        {
            _selectedIndex.Clear();
            var customButtons = CollectionView.LogicalChildren.ToList().ConvertAll(x => (CustomButton) x);

            for (var i = 0; i < customButtons.Count; i++)
                if (customButtons[i].IsClicked)
                    _selectedIndex.Add(i + 1);
            _bindingContext.SelectEventGroup.WeekNo = new List<int>(_selectedIndex);
            await PopupNavigation.Instance.RemovePageAsync(this);
        }


        protected override void OnAppearing()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                //TODO 阻塞
                while (CollectionView.LogicalChildren.Count < 24)
                {
                    Task.Delay(100);
                }

                if (CollectionView.LogicalChildren.Count < 24) return;
                foreach (var index in _bindingContext.SelectEventGroup.WeekNo)
                {
                    if (CollectionView.LogicalChildren[index - 1] is CustomButton button) button.IsClicked = true;
                }
            });
        }
    }
}