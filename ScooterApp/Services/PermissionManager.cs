using System.Threading.Tasks;

namespace ScooterApp.Services
{
    public class PermissionManager
    {
        public static async Task<bool> IsLocationPermissionsGrantedAsync()
        {
            var locationPermissionStatus = await Xamarin.Essentials.Permissions.CheckStatusAsync<Xamarin.Essentials.Permissions.LocationWhenInUse>();

            if (locationPermissionStatus != Xamarin.Essentials.PermissionStatus.Granted)
            {
                var status = await Xamarin.Essentials.Permissions.RequestAsync<Xamarin.Essentials.Permissions.LocationWhenInUse>();
                return status == Xamarin.Essentials.PermissionStatus.Granted;
            }
            return true;
        }
    }
}
