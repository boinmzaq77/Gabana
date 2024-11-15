using Gabana.ShareSource.Abstracts;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Gabana.ShareSource.Print
{
    public class WifiPrinter : Printer
    {
        Socket pSocket;
        public static string settingtext;

        private SettingPrinter setting;

        public WifiPrinter(SettingPrinter setting)
        {
            this.setting = setting;
        }

        public override async Task Open()
        {

            try
            {
                pSocket = new Socket(SocketType.Stream, ProtocolType.IP);
                pSocket.ReceiveTimeout = 1000;
                pSocket.SendTimeout = 1000;
                IAsyncResult result = pSocket.BeginConnect(setting.IPADDRESS, Int32.Parse(setting.PORTNUMBER), null, null);

                bool success = result.AsyncWaitHandle.WaitOne(5000, true);

                if (pSocket.Connected)
                {
                    pSocket.EndConnect(result);
                }
                else
                {
                    // NOTE, MUST CLOSE THE SOCKET

                    pSocket.Close();
                    throw new ApplicationException("ไม่สามารถเชื่อมต่อกับเครื่องปริ้นได้");
                }
                //pSocket.time
                //pSocket.ReceiveTimeout = 1000;
                //pSocket.Connect(setting.IPADDRESS, Convert.ToInt32(setting.PORTNUMBER));

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public override async Task Close()
        {
            pSocket.Close();
        }

        public override async Task Write(byte[] bytesData, int timeOut)
        {
            byte[] b = new byte[]
               {
                    (byte)27,
                    (byte)64
               };
            //pSocket.Send(b);
            //pSocket.Send(Encoding.ASCII.GetBytes("Test Print\r\n\n\n\n\n\n\n"));
            //pSocket.Send(Encoding.ASCII.GetBytes("Test Print\r\n\n\n\n\n\n\n"));
            //pSocket.Send(Encoding.ASCII.GetBytes("Test Print\r\n\n\n\n\n\n\n"));
            //pSocket.Send(Encoding.ASCII.GetBytes("Test Print\r\n\n\n\n\n\n\n"));
            //pSocket.Send(Encoding.ASCII.GetBytes("Test Print\r\n\n\n\n\n\n\n"));
            pSocket.Send(bytesData);
            pSocket.Send(Encoding.ASCII.GetBytes("\n\n"));
        }

        public override async Task Writenofeed(byte[] bytesData, int timeOut)
        {
            pSocket.Send(bytesData);
        }
    }
}
