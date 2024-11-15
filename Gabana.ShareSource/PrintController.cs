using Gabana.Model;
using Gabana.ShareSource.Abstracts;
using Gabana.ShareSource.ClassStructure;
using Gabana.ShareSource.Print;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Gabana.ShareSource
{
    public class PrintController
    {
        public static SettingPrinter settingPrinter = new SettingPrinter();
        Printer bluetooth;
        async public void Test(byte[] t)
        {
            await PrintWifi(t);
        }

        async public Task PrintBluetooth(byte[] t)
        {

            try
            {
                bluetooth = new BluetoothPrinter(Guid.Parse(DataCashingAll.setting.BLUETOOTH1));
                await bluetooth.Open();
                await bluetooth.Write(t, 1000); //Print("SlipTrans.xml", paramSlip);
                await bluetooth.Close();
            }
            catch (Exception ex)
            {
                await bluetooth.Close();
                throw new Exception(ex.Message);
            }
        }

        async public Task PrintWifi(byte[] t)
        {

            try
            {
                var Printer = new WifiPrinter(DataCashingAll.setting);
                await Printer.Open();
                await Printer.Write(t, 10000);
                byte[] b = new byte[]
                {
                    (byte)29,
                    (byte)86,
                    (byte)66,
                    (byte)50
                };

                await Printer.Write(b, 100);
                await Printer.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        async public Task PrintBluetoothAndroid(TranWithDetailsLocal tran)
        {

            try
            {
                List<List<KeyValuePair<string, string>>> items = new List<List<KeyValuePair<string, string>>>();
                for (int i = 0; i < tran.tranDetailItemWithToppings.Count; i++)
                {
                    items.Add(new List<KeyValuePair<string, string>>()
                    {
                    //new KeyValuePair<string, string>("@QuantityItem",(i+1).ToString()),
                    new KeyValuePair<string, string>("@ItemName",tran.tranDetailItemWithToppings[i].tranDetailItem.ItemName),
                    new KeyValuePair<string, string>("@Price",tran.tranDetailItemWithToppings[i].tranDetailItem.Price.ToString("#,##0.00"))
                    });
                }
                ParamSlip paramSlip = new ParamSlip()
                {
                    Header = new List<KeyValuePair<string, string>>()
                    {
                    //new KeyValuePair<string, string>("@Company_name", "ซี"),
                    new KeyValuePair<string, string>("@Billno", tran.tran.TranNo),
                    //new KeyValuePair<string, string>("@DeviceID", "ID000001"),
                    //new KeyValuePair<string, string>("@Datetime", tran),
                    new KeyValuePair<string, string>("@Total",tran.tran.GrandTotal.ToString("#,##0.00"))
                    },
                    Details = items
                };
                Printer bluetooth = new BluetoothPrinter(Guid.Parse(DataCashingAll.setting.BLUETOOTH1));
                await bluetooth.Open();
                await bluetooth.Print("SlipTrans.xml", paramSlip); //Print("SlipTrans.xml", paramSlip);
                await bluetooth.Close();
            }
            catch (Exception)
            {
                return;
            }
        }

        async public Task PrintWifiAndroid(TranWithDetailsLocal tran)
        {

            try
            {
                List<List<KeyValuePair<string, string>>> items = new List<List<KeyValuePair<string, string>>>();
                for (int i = 0; i < tran.tranDetailItemWithToppings.Count; i++)
                {
                    items.Add(new List<KeyValuePair<string, string>>()
                    {
                    //new KeyValuePair<string, string>("@QuantityItem",(i+1).ToString()),
                    new KeyValuePair<string, string>("@ItemName",tran.tranDetailItemWithToppings[i].tranDetailItem.ItemName),
                    new KeyValuePair<string, string>("@Price",tran.tranDetailItemWithToppings[i].tranDetailItem.Price.ToString("#,##0.00"))
                    });
                }
                ParamSlip paramSlip = new ParamSlip()
                {
                    Header = new List<KeyValuePair<string, string>>()
                    {
                    //new KeyValuePair<string, string>("@Company_name", "ซี"),
                    new KeyValuePair<string, string>("@Billno", tran.tran.TranNo),
                    //new KeyValuePair<string, string>("@DeviceID", "ID000001"),
                    //new KeyValuePair<string, string>("@Datetime", tran),
                    new KeyValuePair<string, string>("@Total",tran.tran.GrandTotal.ToString("#,##0.00"))
                    },
                    Details = items
                };

                var Printer = new WifiPrinter(DataCashingAll.setting);
                await Printer.Open();
                await Printer.Print("SlipTrans.xml", paramSlip);
                await Printer.Close();
            }
            catch (Exception)
            {
                return;
            }

        }
    }
}
