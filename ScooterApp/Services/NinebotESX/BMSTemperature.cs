namespace ScooterApp.Services.NinebotESX
{
    public class BMSTemperature
    {
        public int Sensor1 { get; set; }
        public int Sensor2 { get; set; }

        public BMSTemperature(int temperature1, int temperature2)
        {
            Sensor1 = temperature1;
            Sensor2 = temperature2;
        }
    }
}
