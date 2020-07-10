using NeuToDo.Components;
using NeuToDo.ViewModels;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms.Xaml;

namespace NeuToDo.Views.Popup
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WeekNoSelectPopupPage : PopupPage
    {
        public WeekNoSelectPopupPage()
        {
            InitializeComponent();
            _buttons = new List<CustomCheckButton>
            {
                Button1, Button2, Button3, Button4, Button5, Button6, Button7, Button8, Button9, Button10, Button11,
                Button12, Button13, Button14, Button15, Button16, Button17, Button18, Button19, Button20, Button21,
                Button22, Button23, Button24
            };
        }

        private readonly List<CustomCheckButton> _buttons;

        private bool _isSelectAll, _isSelectOdd, _isSelectEven;

        private void SelectAll(object sender, EventArgs e)
        {
            _isSelectAll = !_isSelectAll;

            foreach (var button in _buttons)
                button.IsClicked = _isSelectAll;
        }

        private void SelectOdd(object sender, EventArgs e)
        {
            _isSelectOdd = !_isSelectOdd;

            for (var i = 0; i < 24; i += 2)
                _buttons[i].IsClicked = _isSelectOdd;
        }

        private void SelectEven(object sender, EventArgs e)
        {
            _isSelectEven = !_isSelectEven;

            for (var i = 1; i < 24; i += 2)
                _buttons[i].IsClicked = _isSelectEven;
        }

        private void WeekNoSelectPopupPage_OnAppearing(object sender, EventArgs e)
        {
            if (!(BindingContext is NeuEventDetailViewModel bindingContext)) return;
            _buttons.ForEach(x => x.IsClicked = false);
            foreach (var index in bindingContext.SelectEventGroup.WeekNo)
                _buttons[index - 1].IsClicked = true;
        }

        private void SelectWeekNoCancel(object sender, EventArgs e)
        {
            PopupNavigation.Instance.RemovePageAsync(this);
        }

        private void SelectWeekNoDone(object sender, EventArgs e)
        {
            if (!(BindingContext is NeuEventDetailViewModel bindingContext)) return;
            bindingContext.SelectEventGroup.WeekNo =
                new List<int>(_buttons.Where(x => x.IsClicked).ToList().ConvertAll(x => int.Parse(x.Text)));
            PopupNavigation.Instance.RemovePageAsync(this);
        }
    }
}