using System;
using System.Collections.Generic;
using System.Text;

namespace Gabana.ShareSource.Interfaces
{
    public interface IDeviceInfo
    {
        List<string> Devices();
        void ConnectAndSend(byte[] bytesToPrint, int productId, int vendorId);
        void test();
        void testWrite(string data, int productId, int vendorId);
    }
}
