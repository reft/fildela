using System;
using System.Globalization;

namespace Fildela.Business.Helpers
{
    public static class BusinessUnitConverterExtensions
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
    }
}
