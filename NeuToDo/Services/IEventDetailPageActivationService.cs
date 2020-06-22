using System;
using NeuToDo.Models;
using Xamarin.Forms;

namespace NeuToDo.Services
{
    public interface IEventDetailPageActivationService
    {
        ContentPage Activate(string typeName);
    }
}