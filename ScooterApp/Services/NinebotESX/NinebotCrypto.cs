using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using ScooterApp.Services.ScooterService;

namespace ScooterApp.Services.NinebotESX
{
    public class NinebotCrypto : ICryptoService
    {
        private static readonly byte[] firmawareData = { 0x97, 0xCF, 0xB8, 0x02, 0x84, 0x41, 0x43, 0xDE, 0x56, 0x00, 0x2B, 0x3B, 0x34, 0x78, 0x0A, 0x5D };

        private byte[] randomBleData = new byte[16];
        private byte[] randomAppData = new byte[16];
        private byte[] sha1Key = new byte[16];
        private UInt32 messageIteration = 0;
        private string name;

        public NinebotCrypto(string name)
        {
            this.name = name;
            Array.Clear(randomBleData, 0, randomBleData.Length);
            CalculateSha1Key(Encoding.ASCII.GetBytes(name), firmawareData);
        }

        public byte[] Decrypt(byte[] data)
        {
            byte[] decrypted = new byte[data.Length - 6];

            Buffer.BlockCopy(data, 0, decrypted, 0, 3);
            UInt32 newMessageIteration = messageIteration;
            if ((newMessageIteration & 0x0008000) > 0 && (data[data.Length - 2] >> 7) == 0)
            {
                newMessageIteration += 0x0010000;
            }
            newMessageIteration = (newMessageIteration & 0xFFFF0000) +
                         (UInt32)(data[data.Length - 2] << 8) +
                         data[data.Length - 1];


            int payloadLength = data.Length - 9;
            byte[] payload = new byte[payloadLength];

            Buffer.BlockCopy(data, 3, payload, 0, payloadLength);

            if (newMessageIteration == 0)
            {
                var payload_d = CryptoFirst(payload);
                var payload_e = CryptoFirst(payload_d);
                var eq = payload_e.SequenceEqual(payload);
                if (eq == false)
                {
                    Console.WriteLine("First Not eq");
                    //System.Diagnostics.Debug.WriteLine(String.Format("First Not eq \n\t{0}\n\t{1}\n\t{2}",
                    //    Windows.Security.Cryptography.CryptographicBuffer.EncodeToHexString(payload.AsBuffer()),
                    //    Windows.Security.Cryptography.CryptographicBuffer.EncodeToHexString(payload_d.AsBuffer()),
                    //    Windows.Security.Cryptography.CryptographicBuffer.EncodeToHexString(payload_e.AsBuffer())));

                }

                payload = CryptoFirst(payload);

                Buffer.BlockCopy(payload, 0, decrypted, 3, payload.Length);

                if (decrypted[0] == 0x5A &&
                    decrypted[1] == 0xA5 &&
                    decrypted[2] == 0x1E &&
                    decrypted[3] == 0x21 &&
                    decrypted[4] == 0x3E &&
                    decrypted[5] == 0x5B)
                {
                    Buffer.BlockCopy(decrypted, 7, randomBleData, 0, 16);
                    CalculateSha1Key(Encoding.ASCII.GetBytes(name), randomBleData);
                }
            }
            else if (newMessageIteration > 0 && newMessageIteration > messageIteration)
            {
                var payload_d = CryptoNext(payload, newMessageIteration);
                var payload_e = CryptoNext(payload_d, newMessageIteration);
                var eq = payload_e.SequenceEqual(payload);
                if (eq == false)
                {
                    Console.WriteLine("Next Not eq");
                    //System.Diagnostics.Debug.WriteLine(String.Format("Next Not eq \n\t{0}\n\t{1}\n\t{2}",
                    //    Windows.Security.Cryptography.CryptographicBuffer.EncodeToHexString(payload.AsBuffer()),
                    //    Windows.Security.Cryptography.CryptographicBuffer.EncodeToHexString(payload_d.AsBuffer()),
                    //    Windows.Security.Cryptography.CryptographicBuffer.EncodeToHexString(payload_e.AsBuffer())));
                }

                payload = CryptoNext(payload, newMessageIteration);

                Buffer.BlockCopy(payload, 0, decrypted, 3, payload.Length);

                if (decrypted[0] == 0x5A &&
                    decrypted[1] == 0xA5 &&
                    decrypted[2] == 0x00 &&
                    decrypted[3] == 0x21 &&
                    decrypted[4] == 0x3E &&
                    decrypted[5] == 0x5C &&
                    decrypted[6] == 0x01)
                {
                    CalculateSha1Key(randomAppData, randomBleData);
                }

                messageIteration = newMessageIteration;
            }

            //System.Diagnostics.Debug.WriteLine(String.Format("e string e = \"{0}\";", Windows.Security.Cryptography.CryptographicBuffer.EncodeToHexString(Data.AsBuffer())));
            //System.Diagnostics.Debug.WriteLine(String.Format("e string d = \"{0}\";", Windows.Security.Cryptography.CryptographicBuffer.EncodeToHexString(decrypted.AsBuffer())));

            return decrypted;
        }

        public byte[] Encrypt(byte[] data)
        {
            byte[] encrypted = new byte[152];

            Buffer.BlockCopy(data, 0, encrypted, 0, 3);

            int payload_len = data.Length - 3;

            byte[] payload = new byte[payload_len];
            Buffer.BlockCopy(data, 3, payload, 0, payload_len);

            if (messageIteration == 0)
            {
                byte[] crc = CalculateCrcFirstMessage(payload);
                payload = CryptoFirst(payload);

                Buffer.BlockCopy(payload, 0, encrypted, 3, payload.Length);

                encrypted[payload_len + 3] = 0;
                encrypted[payload_len + 4] = 0;
                encrypted[payload_len + 5] = crc[0];
                encrypted[payload_len + 6] = crc[1];
                encrypted[payload_len + 7] = 0;
                encrypted[payload_len + 8] = 0;
                encrypted = encrypted.Take(payload_len + 9).ToArray();
                messageIteration++;
            }
            else
            {
                messageIteration++;
                byte[] crc = CalculateCrcNextMessage(data, messageIteration);
                payload = CryptoNext(payload, messageIteration);

                Buffer.BlockCopy(payload, 0, encrypted, 3, payload.Length);

                encrypted[payload_len + 3] = crc[0];
                encrypted[payload_len + 4] = crc[1];
                encrypted[payload_len + 5] = crc[2];
                encrypted[payload_len + 6] = crc[3];
                encrypted[payload_len + 7] = (byte)((messageIteration & 0x0000FF00) >> 8);
                encrypted[payload_len + 8] = (byte)((messageIteration & 0x000000FF) >> 0);
                encrypted = encrypted.Take(payload_len + 9).ToArray();

                if (data[0] == 0x5A &&
                    data[1] == 0xA5 &&
                    data[2] == 0x10 &&
                    data[3] == 0x3E &&
                    data[4] == 0x21 &&
                    data[5] == 0x5C &&
                    data[6] == 0x00)
                {
                    Buffer.BlockCopy(data, 7, randomAppData, 0, 16);
                }
            }
            //System.Diagnostics.Debug.WriteLine(String.Format("d string d = \"{0}\";", Windows.Security.Cryptography.CryptographicBuffer.EncodeToHexString(Data.AsBuffer())));
            //System.Diagnostics.Debug.WriteLine(String.Format("d string e = \"{0}\";", Windows.Security.Cryptography.CryptographicBuffer.EncodeToHexString(encrypted.AsBuffer())));

            return encrypted;
        }

        private byte[] CryptoFirst(byte[] data)
        {
            byte[] result = new byte[data.Length];

            int payload_len = data.Length;
            int byte_idx = 0;

            byte[] xor_data_1 = new byte[16];
            byte[] xor_data_2 = new byte[16];

            while (payload_len > 0)
            {
                int tmp_len = (payload_len <= 16) ? payload_len : 16;

                Array.Clear(xor_data_1, 0, xor_data_1.Length);
                Buffer.BlockCopy(data, byte_idx, xor_data_1, 0, tmp_len);

                byte[] aes_key = AesEcbEncrypt(firmawareData, sha1Key);
                Buffer.BlockCopy(aes_key, 0, xor_data_2, 0, 16);
                byte[] xor_data = XOR(xor_data_1, xor_data_2, 16);

                Buffer.BlockCopy(xor_data, 0, result, byte_idx, tmp_len);
                payload_len -= tmp_len;
                byte_idx += tmp_len;
            }

            return result;
        }

        private byte[] CryptoNext(byte[] data, UInt32 messageIteration)
        {
            byte[] result = new byte[data.Length];

            byte[] aes_enc_data = new byte[16];
            Array.Clear(aes_enc_data, 0, 16);
            aes_enc_data[0] = 1;
            aes_enc_data[1] = (byte)((messageIteration & 0xFF000000) >> 24);
            aes_enc_data[2] = (byte)((messageIteration & 0x00FF0000) >> 16);
            aes_enc_data[3] = (byte)((messageIteration & 0x0000FF00) >> 8);
            aes_enc_data[4] = (byte)((messageIteration & 0x000000FF) >> 0);
            Buffer.BlockCopy(randomBleData, 0, aes_enc_data, 5, 8);
            aes_enc_data[15] = 0;

            int payload_len = data.Length;
            int byte_idx = 0;

            byte[] xor_data_1 = new byte[16];
            byte[] xor_data_2 = new byte[16];

            while (payload_len > 0)
            {
                ++aes_enc_data[15];

                int tmp_len = (payload_len <= 16) ? payload_len : 16;

                Array.Clear(xor_data_1, 0, 16);
                Buffer.BlockCopy(data, byte_idx, xor_data_1, 0, tmp_len);

                byte[] aes_key = AesEcbEncrypt(aes_enc_data, sha1Key);
                Buffer.BlockCopy(aes_key, 0, xor_data_2, 0, 16);
                byte[] xor_data = XOR(xor_data_1, xor_data_2, 16);

                Buffer.BlockCopy(xor_data, 0, result, byte_idx, tmp_len);
                payload_len -= tmp_len;
                byte_idx += tmp_len;
            }

            return result;
        }

        private byte[] CalculateCrcFirstMessage(byte[] data)
        {
            UInt16 crc = 0;
            for (int i = 0; i < data.Length; ++i)
            {
                crc += data[i];
            }

            crc = (UInt16)(~crc);

            byte[] ret = new byte[2];
            ret[0] = (byte)(crc & 0x00ff);
            ret[1] = (byte)((crc >> 8) & 0x0ff);

            return ret;
        }

        private byte[] CalculateCrcNextMessage(byte[] data, UInt32 messageIteration)
        {
            byte[] aes_enc_data = new byte[16];
            Array.Clear(aes_enc_data, 0, 16);

            int payload_len = data.Length - 3;
            int byte_idx = 3;
            byte[] xor_data_1 = new byte[16];
            byte[] xor_data_2 = new byte[16];
            byte[] xor_data = null;
            byte[] aes_key = null;

            aes_enc_data[0] = 89;
            aes_enc_data[1] = (byte)((messageIteration & 0xFF000000) >> 24);
            aes_enc_data[2] = (byte)((messageIteration & 0x00FF0000) >> 16);
            aes_enc_data[3] = (byte)((messageIteration & 0x0000FF00) >> 8);
            aes_enc_data[4] = (byte)((messageIteration & 0x000000FF) >> 0);
            Buffer.BlockCopy(randomBleData, 0, aes_enc_data, 5, 8);
            aes_enc_data[15] = (byte)payload_len;

            aes_key = AesEcbEncrypt(aes_enc_data, sha1Key);
            Buffer.BlockCopy(aes_key, 0, xor_data_2, 0, 16);

            Array.Clear(xor_data_1, 0, 16);
            Buffer.BlockCopy(data, 0, xor_data_1, 0, 3);

            xor_data = XOR(xor_data_1, xor_data_2, 16);
            aes_key = AesEcbEncrypt(xor_data, sha1Key);
            Buffer.BlockCopy(aes_key, 0, xor_data_2, 0, 16);

            while (payload_len > 0)
            {
                int tmp_len = (payload_len <= 16) ? payload_len : 16;

                Array.Clear(xor_data_1, 0, 16);
                Buffer.BlockCopy(data, byte_idx, xor_data_1, 0, tmp_len);

                xor_data = XOR(xor_data_1, xor_data_2, 16);

                aes_key = AesEcbEncrypt(xor_data, sha1Key);
                Buffer.BlockCopy(aes_key, 0, xor_data_2, 0, 16);
                payload_len -= tmp_len;
                byte_idx += tmp_len;
            }

            aes_enc_data[0] = 1;
            aes_enc_data[15] = 0;

            aes_key = AesEcbEncrypt(aes_enc_data, sha1Key);
            Buffer.BlockCopy(aes_key, 0, xor_data_1, 0, 4);
            Buffer.BlockCopy(xor_data_2, 0, xor_data_2, 0, 4);

            return XOR(xor_data_1, xor_data_2, 4);
        }

        private void CalculateSha1Key(byte[] sha1Data1, byte[] sha1Data2)
        {
            byte[] sha_data = new byte[32];
            sha1Data1.CopyTo(sha_data, 0);
            sha1Data2.CopyTo(sha_data, 16);

            byte[] sha_hash = Sha1(sha_data);
            Buffer.BlockCopy(sha_hash, 0, sha1Key, 0, 16);
        }

        private byte[] AesEcbEncrypt(byte[] data, byte[] key)
        {
            using(Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.Padding = PaddingMode.None;
                aes.Mode = CipherMode.ECB;
                aes.GenerateIV();
                using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    using (var ms = new MemoryStream())
                    using (var cryptoStream = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(data, 0, data.Length);
                        cryptoStream.FlushFinalBlock();
                        return ms.ToArray();
                    }
                }
            }
        }

        private byte[] Sha1(byte[] data)
        {
            using (SHA1 sha1 = SHA1.Create())
            {
                return sha1.ComputeHash(data);
            }
        }

        private byte[] XOR(byte[] data1, byte[] data2, int size)
        {
            byte[] data = new byte[size];
            for (int i = 0; i < size; i++)
                data[i] = (byte)(data1[i] ^ data2[i]);
            return data;
        }

        public byte[] CalculateCRC(byte[] data)
        {
            byte[] crc = new byte[2];
            long result = 0;
            for (int i = 2; i < data.Length; i++)
            {
                result += data[i];
            }
            short xor = (short)(result ^ 0xFFFF);
            crc[1] = (byte)(xor >> 8);
            crc[0] = (byte)(xor & 0xFF);
            return crc;
        }
    }
}
