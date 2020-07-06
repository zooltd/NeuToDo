using System;
using NeuToDo.Models;
using Xamarin.Forms;

namespace NeuToDo.Services
{
    public interface IContentPageActivationService
    {
        ContentPage Activate(string typeName);
    }
}