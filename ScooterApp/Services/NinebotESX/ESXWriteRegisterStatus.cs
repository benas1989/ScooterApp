namespace ScooterApp.Services.NinebotESX
{
    public enum ESXWriteRegisterStatus
    {
        SUCCESSFUL = 0x00,
        NO_WRITE_PERMISSION = 0x01,
        REGISTER_BUSY = 0x02,
        REGISTER_OUT_OFF_SCOPE = 0x03,
        WRONG_FORMAT = 0x04
    }
}
