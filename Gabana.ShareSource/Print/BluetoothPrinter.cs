using Gabana.ShareSource.Abstracts;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Gabana.ShareSource.Print
{
    public class BluetoothPrinter : Printer
    {
        Plugin.BLE.Abstractions.Contracts.IAdapter adapter;
        Plugin.BLE.Abstractions.Contracts.IDevice device;
        ICharacteristic characteristic;
        Guid _guid;
        List<IDevice> deviceList = new List<IDevice>();
        public BluetoothPrinter(Guid guid)
        {
            _guid = guid;
        }

        public override async Task Open()
        {
            Plugin.BLE.Abstractions.ConnectParameters connectParameters = new Plugin.BLE.Abstractions.ConnectParameters(false, true);

            adapter = CrossBluetoothLE.Current.Adapter;
            adapter.ScanMode = ScanMode.Balanced;
            try
            {
                device = await adapter.ConnectToKnownDeviceAsync(_guid, connectParameters);
                var Services = await device.GetServicesAsync();
                var selectedServiceGuid2 = Services[Services.Count - 1].Id;
                var Service = await device.GetServiceAsync(selectedServiceGuid2);
                var characteristics2 = await Service.GetCharacteristicAsync(_guid);
                var characteristics = await Service.GetCharacteristicsAsync();
                characteristic = characteristics2;
            }
            catch (Exception)
            {
                throw new Exception("ไม่สามารถเชื่อมต่อกับเครื่องปริ้นได้");
            }
        }

#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public override async Task Close()
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {

        }

        public override async Task Write(byte[] bytesData, int timeOut)
        {
            //byte[] bytes = Encoding.ASCII.GetBytes("111111111111111111111111111111111111111111111111");
            await characteristic.WriteAsync(bytesData);
            await characteristic.WriteAsync(Encoding.ASCII.GetBytes("\n\n"));
        }
        public override async Task Writenofeed(byte[] bytesData, int timeOut)
        {
            //throw new NotImplementedException();
            await characteristic.WriteAsync(bytesData);
        }
    }

}
