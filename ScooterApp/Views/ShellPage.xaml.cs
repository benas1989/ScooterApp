using System;
using System.Collections.Generic;
using ScooterApp.ViewModels;
using Xamarin.Forms;

namespace ScooterApp.Views
{
    public partial class ShellPage : Shell
    {
        public ShellPageViewModel VM { get { return BindingContext as ShellPageViewModel; } set { BindingContext = value; } }
        public ShellPage()
        {
            InitializeComponent();
            VM = new ShellPageViewModel();
        }
    }
}
