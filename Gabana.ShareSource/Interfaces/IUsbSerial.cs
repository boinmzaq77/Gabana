using Gabana.ShareSource.ClassStructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gabana.ShareSource.Interfaces
{
    public interface IUsbSerial
    {
        List<UsbDevices> GetDevices();

        void Open(int productId, int vendorId, UsbParameters parameters);

        void Close();

        void Write(byte[] byteData, int timeout);

        //Bitmap ConvertImageToBitmap(string imageName);

        //byte[] ReadImageAllBytes(string imageName);

        //byte[] GenBitmapCode(Stream stream, bool doubleWidth, bool doubleHeight);
    }
}
