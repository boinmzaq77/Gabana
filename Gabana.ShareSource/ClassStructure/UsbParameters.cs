using System;
using System.Collections.Generic;
using System.Text;

namespace Gabana.ShareSource.ClassStructure
{
    public class UsbParameters
    {
        public int BaudRate { get; set; }
        public int DataBits { get; set; }
        public byte StopBitsByte { get; set; }
        public byte ParityBitesByte { get; set; }

        public UsbParameters(int baudRate, int dataBits, StopBits stopBits, Parity parity)
        {
            BaudRate = baudRate;
            DataBits = dataBits;
            switch (stopBits)
            {
                case StopBits.One: StopBitsByte = 0; break;
                case StopBits.OnePointFive: StopBitsByte = 1; break;
                case StopBits.Two: StopBitsByte = 2; break;
                default: throw new Exception("Bad value for stopBits: " + stopBits);
            }
            switch (parity)
            {
                case Parity.None: ParityBitesByte = 0; break;
                case Parity.Odd: ParityBitesByte = 1; break;
                case Parity.Even: ParityBitesByte = 2; break;
                case Parity.Mark: ParityBitesByte = 3; break;
                case Parity.Space: ParityBitesByte = 4; break;
                default: throw new Exception("Bad value for parity: " + parity);
            }

            //baudRate = 9600;
            //dataBits = 8;
            //stopBitsByte = 0;// None
            //parityBitesByte = 0;// None
        }
    }
}
