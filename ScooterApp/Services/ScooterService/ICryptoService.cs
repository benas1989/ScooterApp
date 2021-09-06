namespace ScooterApp.Services.ScooterService
{
    public interface ICryptoService
    {
        byte[] Decrypt(byte[] data);
        byte[] Encrypt(byte[] data);
        byte[] CalculateCRC(byte[] data);
    }
}
