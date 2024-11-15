using Gabana.ShareSource.Abstracts;
using Gabana.ShareSource.ClassStructure;
using Gabana.ShareSource.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Gabana.ShareSource.Print
{
    public class UsbPrinter : Printer
    {
        // UsbPrinter ตอนนี้มีแต่เฉพาะใน Android เท่านั้น
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public override async Task Open()
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
#pragma warning disable CS0219 // The variable 'productid' is assigned but its value is never used
            int productid = 4101;
#pragma warning restore CS0219 // The variable 'productid' is assigned but its value is never used
#pragma warning disable CS0219 // The variable 'vendorid' is assigned but its value is never used
            int vendorid = 33923;
#pragma warning restore CS0219 // The variable 'vendorid' is assigned but its value is never used
            try
            {
                UsbParameters usbParameters = new UsbParameters(9600, 8, StopBits.One, Parity.None);
                var x = DependencyService.Get<IUsbSerial>().GetDevices();
                DependencyService.Get<IUsbSerial>().Open(x[0].productId, x[0].vendorId, usbParameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public override async Task Close()
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            DependencyService.Get<IUsbSerial>().Close();
        }
#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public override async Task Write(byte[] bytesData, int timeOut)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            DependencyService.Get<IUsbSerial>().Write(bytesData, timeOut);
        }
        public override async Task Writenofeed(byte[] bytesData, int timeOut)
        {
            DependencyService.Get<IUsbSerial>().Write(bytesData, timeOut);
        }
    }
}
