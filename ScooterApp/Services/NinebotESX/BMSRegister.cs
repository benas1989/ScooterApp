namespace ScooterApp.Services.NinebotESX
{
    public enum BMSRegister
    {
       SERIAL_NUMBER = 0x10, //Ok
       FIRMWARE_VERSION = 0x17,//Ok
       DESIGN_CAPACITY = 0x18,//Ok
       ACTUAL_CAPACITY = 0x19,//
       DESIGN_VOLTAGE = 0x1A,//
       CHARGE_FULL_CYCLES = 0x1B,//Ok
       CHARGE_COUNT = 0x1C,//Ok
       CHARGE_CAPACITY_L = 0x1D,//Not working
       CHARGE_CAPACITY_H = 0x1E,//Not working
       OVER_DISCHARGE_TIMES = 0x1F,//Not working
       MANUFACTURE_DATE = 0x20,//Ok
       STATUS = 0x30,//Ok
       REMAINING_CAPACITY_mAh = 0x31,//Ok
       REMAINING_CAPACITY_PERCENT = 0x32,//Ok
       CURRENT = 0x33,//Ok
       VOLTAGE = 0x34,//Ok
       TEMPERATURE = 0x35,//Ok
       BALANCE_STATUS = 0x36,//Ok
       UNDERVOLTAGE_STATUS = 0x37,//Ok
       OVERVOLTAGE_STATUS = 0x38,//Ok
       COULOMBMETER_CAPACITY = 0x39,//Ok
       CAPACITY_BY_VOLTAGE = 0x3A,//Ok
       HEALTH = 0x3B,//Ok
       CELL_1_VOLTAGE = 0x40,//Ok
       CELL_2_VOLTAGE = 0x41,//Ok
       CELL_3_VOLTAGE = 0x42,//Ok
       CELL_4_VOLTAGE = 0x43,//Ok
       CELL_5_VOLTAGE = 0x44,//Ok
       CELL_6_VOLTAGE = 0x45,//Ok
       CELL_7_VOLTAGE = 0x46,//Ok
       CELL_8_VOLTAGE = 0x47,//Ok
       CELL_9_VOLTAGE = 0x48,//Ok
       CELL_10_VOLTAGE = 0x49,//Ok
       CONFIG_STRAPS = 0x51,
       ACTIVATION_DATA = 0x70
    }
}
