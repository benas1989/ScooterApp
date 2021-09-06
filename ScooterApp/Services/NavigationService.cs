using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ScooterApp.Services
{
    public class NavigationService
    {
        private static NavigationService instance;
        public static NavigationService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new NavigationService();
                }
                return instance;
            }
        }
        private NavigationService(){}

        public void Goto(string route)
        {
            try
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Shell.Current.GoToAsync(route, true);

                });
            }
            catch (Exception ex)
            {
                NotificationService.Instance.DisplayAlert("Error", ex.Message);
            }
        }
    }
}
