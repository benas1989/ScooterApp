using System;
using System.Collections.Generic;
using ScooterApp.ViewModels;
using Xamarin.Forms;

namespace ScooterApp.Views
{
    public partial class ConnectPage : ContentPage
    {
        private bool firstTime = true;
        public ConnectPageViewModel VM { get { return BindingContext as ConnectPageViewModel; } set { BindingContext = value; } }

        public ConnectPage()
        {
            InitializeComponent();
            VM = new ConnectPageViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (firstTime)
            {
                VM.ScanBLECommand.Execute(null);
                firstTime = false;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
    }
}
