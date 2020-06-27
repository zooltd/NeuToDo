using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using NeuToDo.Components;
using NeuToDo.Models;
using NeuToDo.Services;
using NeuToDo.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NeuToDo.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EventDetailPage : ContentPage
    {
        private readonly EventDescription _eventDescription = new EventDescription();
        private readonly EventTime _eventTime = new EventTime();
        private readonly EventPeriod _eventPeriod = new EventPeriod();

        public EventDetailPage(string typeName)
        {
            InitializeComponent();
            var pageContent = typeName switch
            {
                nameof(NeuEvent) => new StackLayout {Children = {_eventDescription, _eventTime, _eventPeriod}},
                nameof(MoocEvent) => new StackLayout {Children = {_eventDescription, _eventTime}},
                nameof(UserEvent) => new StackLayout {Children = {_eventDescription, _eventTime, _eventPeriod}},
                _ => new StackLayout()
            };
            var testLabel = new Label();
            testLabel.SetBinding(Label.TextProperty, new Binding("SelectedEvent.Title"));
            pageContent.Children.Add(testLabel);
            Content = pageContent;
        }
    }
}