using System;
using System.Collections.Generic;
using System.Text;

namespace Gabana.ShareSource.Interfaces
{
    public interface ISerialPort
    {
        bool Open(string name, int baudrate, int flags = 0);

        void Close();

        void Send(byte[] data);

        event EventHandler<byte[]> Received;
    }
}
