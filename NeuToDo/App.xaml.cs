﻿using NeuToDo.Views;
using Xamarin.Forms;

namespace NeuToDo
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }
    }
}