using System;
using System.Text;

namespace ScooterApp
{
    public class Utils
    {
        public static void PrintHex(string title, byte[] data)
        {
            if (data == null)
            {
                Console.WriteLine("ERROR: data array is NULL. Title: " + title);
                return;
            }
            StringBuilder message = new StringBuilder();
            message.Append(title + ": ");
            Console.Write(title + ": ");
            foreach (var item in data)
            {
                if (item <= 0x0f)
                {
                    message.Append(string.Format("0x0{0:X},", item));
                    Console.Write("0x0{0:X},", item);
                }
                else
                {
                    message.Append(string.Format("0x{0:X},", item));
                    Console.Write("0x{0:X},", item);
                }
            }
            Console.WriteLine();
        }

        public static string ArrayToASCI(byte[] data)
        {
            StringBuilder asci = new StringBuilder();
            foreach (var item in data)
            {
                asci.Append((char)item);
            }
            return asci.ToString();
        }

        public static string ArrayToNinebotVersion(byte[] data)
        {
            string version = string.Format("{0}.{1}.{2}", data[1] & 0xF, data[0] >> 4, data[0] & 0xF);
            return version;
        }

        public static int BytesToInt(byte byte1, byte byte0)
        {
            int total = byte1;
            total = total << 8;
            total = (total | byte0);
            return total;
        }

        public static int BytesToInt(byte byte3, byte byte2, byte byte1, byte byte0)
        {
            int total = byte3;
            total = total << 8;
            total = total | byte2;
            total = total << 8;
            total = total | byte1;
            total = total << 8;
            total = total | byte0;
            return total;
        }
    }
}
