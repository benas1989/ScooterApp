namespace ScooterApp.Services.NinebotESX
{
    public enum ESCRegister
    {
        PHASE_A_CURRENT = 0x0D,
        PHASE_B_CURRENT = 0x0E,
        PHASE_C_CURRENT = 0x0F,
        SERIAL_NUMBER = 0x10, //Ok
        BLE_PASSWORD = 0x17, //Ok
        ESC_FIRMWARE_VERSION = 0x1A, //Ok
        ERROR_CODE = 0x1B, //OK
        ALARM_CODE = 0x1C, // Ok
        ESC_STATUS = 0X1D, // Ok
        CURRENT_OPERATION_SYSTEM = 0x1E, //Ok
        CURRENT_OPERATION_MODE = 0x1F, //Not working. Use 0x75.
        VOLUME_OF_BMS1 = 0x20, //Not working
        VOLUME_OF_BMS2 = 0x21,//Not working
        BMS_PERCENTAGE = 0x22,//Ok
        ACTUAL_REMAINING_MILEAGE = 0x24,//Ok
        PREDICTED_REMAINING_MILEAGE = 0x25,//Ok
        CURRENT_SPEED = 0x26,//Ok
        TOTAL_MILEAGE_L = 0x29,//Ok
        TOTOAL_MILEAGE_H = 0X2A,//Ok
        CURRENT_MILEAGE = 0x2F, //Ok
        TOTAL_RUNTIME_L = 0x32,//Ok
        TOTAL_RUNTIME_H = 0x33,//Ok
        TOTAL_RIDING_TIME_L = 0x34,//Ok
        TOTAL_RIDING_TIME_H = 0x35,//Ok
        CURRENT_RUNTIME = 0x3A,//Ok
        CURRENT_RIDING_TIME = 0x3B,//Ok
        SCOOTER_TEMPERATURE = 0x3E,//Ok
        BMS1_TEMPERATURE = 0x3F,//Not working
        BMS2_TEMPERATURE = 0x40,//Not Working
        MOS_PIPE_TEMPERATURE = 0x42,//Not working
        ESC_VOLTAGE = 0x47,//Ok
        BMS_VOLTAGE = 0x48,//Ok
        BMS_CURRENT = 0x49,//Ok
        REINITIALIZE_BLE = 0x4D,//??
        BMS2_TEMPERATURE_1C = 0x50,//Not working
        MOTOR_PHASE_CURRENT = 0x53,//Ok
        AVERAGE_SPEED = 0x65,//Ok
        BMS2_FIRMWARE_VERSION = 0x66,//Ok
        BMS1_FIRMWARE_VERSION = 0x67,//Ok
        BLE_FIRMWARE_VERSION = 0x68,//Ok
        LOCK_SCOOTER = 0x70,//Ok
        UNLOCK_SCOOTER = 0x71,//Ok
        SPEED_LIMIT_SPORT_MODE = 0x72,//Ok
        SPEED_LIMIT_NORMAL_MODE = 0x73,//Ok
        SPEED_LIMIT_ECO_MODE =0x74,//Ok
        OPERATING_MODE = 0x75,//Ok
        ENGINE_STATUS =0x76,//Ok
        RESTART = 0x78,//Ok
        SHUTDOWN = 0x79,//Ok
        KERS_LEVEL = 0x7B,//Ok
        CRUISE_CONTROL = 0x7C,//Ok
        FUNCTION_SETUP = 0x7D,//Ok
        RENTAL_LOOKING_FOR_SCOOTER = 0x7E,
        RENTAL_FUNCTION_SETUP1 = 0x80,
        RENTAL_FUNCTION_SETUP2 = 0x81,
        RENTAL_HEADLIGHT_CONTROL = 0x90,
        RENTAL_BEEP_ALARM = 0x91,
        RENTAL_BEEP_CONTROL_SWITCH = 0x92,


        ERROR_CODE_QUICK = 0xB0,//Ok
        ALARM_CODE_QUICK = 0xB1,//Ok
        ESC_STATUS_QUICK = 0xB3,//Ok
        VOLUME_OF_BMS_1_AND_2_QUICK = 0xB3,//Ok
        VOLUME_OF_BMS_QUICK = 0xB4,//Ok
        CURRENT_SPEED_QUICK = 0xB5,//Ok
        AVERAGE_SPEED_QUICK = 0xB6,//Ok
        TOTAL_MILEAGE2_L_QUICK = 0xB7,//Ok
        TOTAL_MILEAGE2_H_QUICK = 0xB8,//Ok
        SINGLE_MILEAGE_QUICK = 0xB9,//Ok
        SINGLE_OPERATION_TIME_QUICK = 0xBA,//Ok
        SCOOTER_TEMPERATURE_QUICK = 0xBB,//Ok
        OPERATING_MODE_AND_RANGE_QUICK = 0xBC,//Ok
        SCOOTER_POWER_QUICK = 0xBD,//Not working
        PREVIOUS_ALARM_CODE_FOR_DELAY_RESET_QUICK = 0xBE,//Ok
        PREDICTED_REMAINING_MILEAGE_QUICK = 0xBF,//Ok


        DISPLAY_MODE_OF_CHASSIS_LAMP = 0xC6, //Ok
        COLOR_CASSIS_LAMP_STRIP1 = 0xC8,
        COLOR_CASSIS_LAMP_STRIP2 = 0xCA,
        COLOR_CASSIS_LAMP_STRIP3 = 0xCC,
        COLOR_CASSIS_LAMP_STRIP4 = 0xCE,
        CPU_ID_A = 0xDA,
        CPU_ID_B = 0xDB,
        CPU_ID_C = 0xDC,
        CPU_ID_D = 0xDD,
        CPU_ID_E = 0xDE,
        CPU_ID_F = 0xDF,
    }
}
