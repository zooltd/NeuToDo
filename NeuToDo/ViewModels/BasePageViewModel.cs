using System.ComponentModel;
using System.Runtime.CompilerServices;
using NeuToDo.Annotations;
using Xamarin.Plugin.Calendar.Models;

namespace NeuToDo.ViewModels
{
    public class BasePageViewModel : INotifyPropertyChanged
    {
        public EventCollection Events { get; set; }

        private string _title = string.Empty;

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void SetProperty<TData>(ref TData storage, TData value, [CallerMemberName] string propertyName = "")
        {
            if (storage.Equals(value))
                return;

            storage = value;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}