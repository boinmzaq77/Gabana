using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Gabana.ShareSource.Interfaces
{
    public interface IBluetooth
    {
        IList<string> GetDeviceList();
        // Task Print(string deviceName, string text);
        Task Open(string deviceName, byte[] linesToPrint);
    }
}
