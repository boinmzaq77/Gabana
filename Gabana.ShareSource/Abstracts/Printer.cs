using Gabana.ShareSource.ClassStructure;
using Gabana.ShareSource.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Gabana.ShareSource.Abstracts
{
    public abstract class Printer
    {
        public Printer()
        {

        }
        public abstract Task Open();
        public abstract Task Close();
        public abstract Task Write(byte[] bytesData, int timeOut);
        public abstract Task Writenofeed(byte[] bytesData, int timeOut);

        public async Task Print(string slipName, ParamSlip paramSlip)
        {



            //byte[] bytesData = DependencyService.Get<IGraphic>().DrawString();
            //byte[] bytesData = DependencyService.Get<IGraphic>().DrawString();

            byte[] bytesData = DependencyService.Get<IGraphic>().DrawImageFromXml(slipName, paramSlip);
            byte[] bytes = Encoding.ASCII.GetBytes("123456789");
            await Write(bytes, 1000);
            //PaperCut();

        }

        public void SetCodePange(int pangecode, ref List<byte> outputList)
        {
            try
            {
                outputList.Add((byte)27);
                outputList.Add((byte)116);
                outputList.Add((byte)pangecode);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public void SetPrintMod(int mode, ref List<byte> outputList)
        {
            try
            {
                outputList.Add((byte)27);
                outputList.Add((byte)33);
                outputList.Add((byte)mode);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }


        public async void PaperCut()
        {

            /*------------------------------/
            ESC:29
            t:86
            m Function
             65 Feeds paper to(cutting position + [n x basic calculated pitch]) and performs a full cut
             66 Feeds paper to(cutting position + [n x basic calculated pitch]) and performs a partial cut
            (one point uncut)
             67 Not Used
             68 Not Used
            n 0 >= n <= 255
            /-------------------------------*/


            /// Set bit-Image mode
            byte[] b = new byte[]
            {
                (byte)29,
                (byte)86,
                (byte)66,
                (byte)50
            };

            await Write(b, 100);

            //outputList.Add((byte)29);
            //outputList.Add((byte)86);
            //outputList.Add((byte)m);
            //outputList.Add((byte)n);
        }



        private void GenerateImageFromXml()
        { // method อ่านค่า xml และ

        }


    }

}
