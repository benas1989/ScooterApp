namespace ScooterApp.Services.NinebotESX
{
    public enum ESXFirmwareUpdateStatus
    {
        SUCCESSFUL = 0x00,
        FIRMWARE_IS_OVER_SIZED = 0x01,
        ERASE_FLASH_FAILED = 0x02,
        WRITE_FLASH_FAILED = 0x03,
        SCOOTER_UNLOCKED = 0x04,
        DATA_INDEX_ERROR = 0x05,
        BUSY = 0x06,
        DATA_FORMAT_ERROR = 0x07,
        DATA_VERIFICATION_FAILURE = 0x08,
        OTHER_ERROR = 0x09
    }
}
