﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NeuToDo.Views.SyncPage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LocalSyncPage : ContentPage
    {
        public LocalSyncPage()
        {
            InitializeComponent();
        }
    }
}