using System;
using System.Collections.Generic;
using ScooterApp.ViewModels;
using Xamarin.Forms;

namespace ScooterApp.Views
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPageViewModel VM { get { return BindingContext as SettingsPageViewModel; } set { BindingContext = value; } }

        public SettingsPage()
        {
            InitializeComponent();
            VM = new SettingsPageViewModel();
        }

        protected override void OnAppearing()
        {
            Console.WriteLine("SettingsPage page OnAppearing");
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            Console.WriteLine("SettingsPage page OnDisappearing");
            base.OnDisappearing();
        }
    }
}
