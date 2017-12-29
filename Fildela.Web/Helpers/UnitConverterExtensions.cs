using System;
using System.Globalization;

namespace Fildela.Web.Helpers
{
    public class UnitConverterExtensions
    {
        public static string ConvertByteToASuitableUnit(long bytes)
        {
            double bytesConverted = bytes;
            string bytesConvertedResult = String.Empty;

            //Byte
            bytesConvertedResult = bytesConverted.ToString("0") + " Bytes";

            //Byte - Kilobyte
            if (bytesConverted > 99.99)
            {
                bytesConverted = bytesConverted / 1024;
                bytesConvertedResult = bytesConverted.ToString("0.00", new CultureInfo("en-US")) + " KB";

                //Kilobyte - Megabyte
                if (bytesConverted > 99.99)
                {
                    bytesConverted = bytesConverted / 1024;
                    if (bytesConverted < 1)
                        bytesConvertedResult = bytesConverted.ToString("0.00", new CultureInfo("en-US")) + " MB";
                    else
                        bytesConvertedResult = bytesConverted.ToString("0.00", new CultureInfo("en-US")) + " MB";
                }
            }

            return bytesConvertedResult;
        }

        public static string ConvertByteToASuitableUnitAndReturnUnitName(long bytes)
        {
            double bytesConverted = bytes;

            if (bytesConverted > 99.99)
            {
                bytesConverted = bytesConverted / 1024;

                if (bytesConverted > 99.99)
                    return "MB";
                else
                    return "KB";
            }
            else
                return "Byte";
        }

        public static decimal ConvertBytesToMegabytes(long bytes)
        {
            var a1 = (bytes / 1024M) / 1024M;
            var a2 = ((decimal)bytes / 1024M) / 1024M;

            return (bytes / 1024M) / 1024M;
        }

        public static string ConvertToPercentageString(double currentVal, double maxVal)
        {
            string percentageString = string.Empty;

            if (currentVal != 0 && maxVal != 0)
            {
                double percentageLong = currentVal / maxVal;

                if (percentageLong < 0.01)
                    percentageLong = 0.01;

                percentageString = percentageLong.ToString("0%");
            }
            else
                percentageString = "0%";

            return percentageString;
        }

        public static string SubtractAndConvertByteToASuitableUnit(long allowedStoredBytes, long storedBytes)
        {
            double bytesConverted = allowedStoredBytes - storedBytes;
            string bytesConvertedResult = String.Empty;

            //Byte
            bytesConvertedResult = bytesConverted.ToString("0") + " Bytes";

            //Byte - Kilobyte
            if (bytesConverted > 99.99)
            {
                bytesConverted = bytesConverted / 1024;
                bytesConvertedResult = bytesConverted.ToString("0.00", new CultureInfo("en-US")) + " KB";

                //Kilobyte - Megabyte
                if (bytesConverted > 99.99)
                {
                    bytesConverted = Math.Floor(bytesConverted = bytesConverted / 1024);
                    bytesConvertedResult = bytesConverted.ToString("0") + " MB";
                }
            }

            return bytesConvertedResult;
        }
    }
}