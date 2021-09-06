using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ScooterApp.Services
{
    public class NotificationService
    {
        private static NotificationService instance;
        public static NotificationService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NotificationService();
                }
                return instance;
            }
        }
        private NotificationService() { }

        public void DisplayAlert(string title, string message, string buttonText = "Ok")
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.DisplayAlert(title, message, buttonText);
            });
        }
    }
}
