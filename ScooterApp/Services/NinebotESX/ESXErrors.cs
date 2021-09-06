using System.Collections.Generic;

namespace ScooterApp.Services.NinebotESX
{
    public class ESXErrors
    {
        public static readonly Dictionary<int, string> ErrorCodes = new Dictionary<int, string>()
        {
            { 0, "No errors." },
            { 10, "Abnormal communicaton between instrument panel and main control panel; please check related wiring." },
            { 11, "Abnormal A phase current sampling of the motor, please check the main control panel." },
            { 12, "Abnormal B phase current sampling of the motor, please check the main control panel." },
            { 13, "Abnormal C phase current sampling of the motor, please check the main control panel." },
            { 14, "Abnormal accelerator Hall, please check accelerator twist grip , instrument panel and related wiring." },
            { 15, "Abnormal brake Hall, please check brake twist grip , instrument panel and related wiring." },
            { 16, "Built-in batery abnormality in MOS switch, please check the main control panel." },
            { 17, "External batery abnormality in MOS switch, please check the main control panel." },
            { 18, "Abnormal motor Hall, please check motor, main control panel and related wiring." },
            { 19, "Built-in batery abnormality in voltage detecton, please check built-in batery, main control panel and related wiring" },
            { 20, "External batery abnormality in voltage detecton, please check external batery, main control panel and related wiring." },
            { 21, "Built-in batery abnormality in communicaton, please check built-in batery, main control panel and related wiring." },
            { 22, "Built-in batery password is wrong, please replace the batery." },
            { 23, "Built-in batery is in default serial number, please replace the batery." },
            { 24, "System voltage detecton abnormality, please check the main control panel." },
            { 25, "Undefined error 25." },
            { 26, "Flash save error, please check the main control panel." },
            { 27, "Master control password is wrong, please replace the main control panel." },
            { 28, "Short circuit in motor driven upper MOS bridge, please check the main control panel." },
            { 29, "Failure in motor driven botom MOS bridge, please check the main control panel." },
            { 30, "Undefined error 30." },
            { 31, "Program skip error, please check the main control panel." },
            { 32, "Undefined error 32." },
            { 33, "Undefined error 33." },
            { 34, "Undefined error 34." },
            { 35, "Vehicle is in default serial number, please replace the main control panel." },
            { 36, "2+4 connector failure or external batery charging wire failure." },
            { 37, "Charging base failure or built-in batery charging wire failure." },
            { 38, "Undefined." },
            { 39, "Built-in batery temperature sensor abnormality, please replace the batery." },
            { 40, "Controller temperature sensor abnormality, please check the main control panel." },
            { 41, "External batery temperature sensor abnormality, please replace the batery." },
            { 42, "External batery abnormality in communicaton, please check external batery, main control panel and related wiring." },
            { 43, "External batery password is wrong, please replace the batery" },
            { 44, "External batery is in default serial number, please replace the batery." },
            { 45, "Undefined error 45." },
            { 46, "Undefined error 46." },
            { 47, "Undefined error 47." },
            { 48, "Undefined error 48." },
            { 49, "Undefined error 49." },
            { 50, "Undefined error 50." },
            { 51, "Invalid ESC firmware." },
        };
    }
}
