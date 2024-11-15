
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Xamarin.Forms;

namespace Gabana.iOS
{
    public class EscPosCommand
    {
       // string formtmu1;
        string spacebar80 = "                                            "; //42
        string spacebar48 = "                                ";
        int printsize = 1 ;
        

        public EscPosCommand(int v)
        {
            this.printsize = v;
        }

        public void PaperCut(ref List<byte> outputList, int m, int n)
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

            outputList.Add((byte)29);
            outputList.Add((byte)86);
            outputList.Add((byte)m);
            outputList.Add((byte)n);
        }

        public void OpentCashdraWersFullCut(ref List<byte> outputList, int p)
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

            //27,112,0,50,250

            outputList.Add((byte)27);
            outputList.Add((byte)112);
            outputList.Add((byte)p);
            outputList.Add((byte)50);
            outputList.Add((byte)250);
        }





        public void WakeupPrinter(Socket pSocket)
        {
            try
            {
                // byte[] b  new byte[3];
                byte[] b = new byte[0];
                //uf_write(b);
                //Thread.Sleep(100);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public void InitPrinter(ref List<byte> outputList)
        {
            try
            {
                //byte[] b = new byte[]
                //{
                //    (byte)27,
                //    (byte)64
                //};

                outputList.Add((byte)27);
                outputList.Add((byte)64);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public void SetCenter(ref List<byte> outputList)
        {
            try
            {
                outputList.Add((byte)27);
                outputList.Add((byte)97);
                outputList.Add((byte)49);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }
        public void SetLeft(ref List<byte> outputList)
        {
            try
            {
                outputList.Add((byte)27);
                outputList.Add((byte)97);
                outputList.Add((byte)0);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public void LF(ref List<byte> outputList)
        {
            try
            {
                //byte[] b = new byte[]
                //{
                //    (byte)10
                //};
                outputList.Add((byte)10);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public void CR(ref List<byte> outputList)
        {
            try
            {
                //byte[] b = new byte[]
                //{
                //    (byte)13
                //};
                outputList.Add((byte)13);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex.InnerException);
            }
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

        //public BMPData ConverToDotScale(Bitmap bmp)
        //{
        //    try
        //    {
        //        int threshold = 180; //127  //tawat Set ความเค็มภาพ
        //        int index = 0;
        //        double multiplier = 512;
        //        double scale = (double)(multiplier / (double)bmp.Width);
        //        int xHeight = (int)(bmp.Height * scale);
        //        int xWidth = (int)(bmp.Width * scale);
        //        var dimen = xWidth * xHeight;
        //        BitArray dots = new BitArray(dimen);

        //        for (var y = 0; y < xHeight; y++)
        //        {
        //            for (var x = 0; x < xWidth; x++)
        //            {
        //                var _x = (int)(x / scale);
        //                var _y = (int)(y / scale);

        //                //var color = new Color();
        //                //color = bmp.GetPixel(_x, _y);
        //                //var luminance = (int)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);

        //                var pixel = new Color(bmp.GetPixel(x, y));
        //                int li_r = (int)(pixel.R * 0.3);
        //                int li_g = (int)(pixel.G * 0.59);
        //                int li_b = (int)(pixel.B * 0.11);
        //                var luminance = Color.Rgb(li_r, li_g, li_b);

        //                dots[index] = (luminance < threshold);
        //                index++;
        //            }
        //        }

        //        //MessageBox.Show("xHeight:"+ xHeight.ToString()+ "/nxWidth:"+ xWidth.ToString(), "Test");
        //        return new BMPData()
        //        {
        //            Dots = dots,
        //            Height = (int)(bmp.Height * scale),
        //            Width = (int)(bmp.Width * scale)
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message, ex.InnerException);
        //    }
        //}

        //public void uf_getByteFromBMP(BMPData bmp, ref List<byte> outputList)
        //{
        //    BMPData data = bmp;
        //    BitArray dots = data.Dots;
        //    byte[] width = BitConverter.GetBytes(data.Width);

        //    int offset = 0;
        //    //MemoryStream stream = new MemoryStream();
        //    //BinaryWriter bw = new BinaryWriter(stream);

        //    // uf_initPrinter(); //ของเก่า
        //    outputList.Add((byte)27);
        //    outputList.Add((byte)64);

        //    //  uf_setLineSpace(24); //ของเก่า
        //    //Set line Space 24
        //    outputList.Add((byte)27);
        //    outputList.Add((byte)51);
        //    outputList.Add((byte)24);

        //    while (offset < data.Height)
        //    {
        //        uf_setImage_24dot_DoubleWidth(width[0], width[1], ref outputList);

        //        for (int x = 0; x < data.Width; ++x)
        //        {
        //            for (int k = 0; k < 3; ++k)
        //            {
        //                byte slice = 0;
        //                for (int b = 0; b < 8; ++b)
        //                {
        //                    int y = (((offset / 8) + k) * 8) + b;
        //                    // Calculate the location of the pixel we want in the bit array.
        //                    // It'll be at (y * width) + x.
        //                    int i = (y * data.Width) + x;

        //                    // If the image is shorter than 24 dots, pad with zero.
        //                    bool v = false;
        //                    if (i < dots.Length)
        //                    {
        //                        v = dots[i];
        //                    }
        //                    slice |= (byte)((v ? 1 : 0) << (7 - b));
        //                }
        //                // bw.Write(slice);
        //                outputList.Add(slice);
        //            }
        //        }
        //        offset += 24;
        //        // bw.Write(10);   //Set new line 
        //        outputList.Add((byte)10);
        //    }
        //    //// Restore the line spacing to the default of 30 dots.
        //    //Set line Space 30
        //    outputList.Add((byte)27);
        //    outputList.Add((byte)51);
        //    outputList.Add((byte)30);

        //    //bw.Flush();
        //    //byte[] bytes = stream.ToArray();

        //}

        public void setImage_24dot_DoubleWidth(int widthLow, int widthHeight, ref List<byte> outputList)
        {
            ///// Set bit-Image mode
            //byte[] b = new byte[]
            //{
            //    (byte)27,               //ESC
            //    (byte)42,               // bit-image mode
            //    (byte)33,               // 24-dot double-density 33
            //    (byte)widthLow,         // width low byte
            //    (byte)widthHeight      // width high byte
            //};

            //writer.Write(b);

            outputList.Add((byte)27);
            outputList.Add((byte)42);
            outputList.Add((byte)33);
            outputList.Add((byte)widthLow);
            outputList.Add((byte)widthHeight);
        }

        //public void LoadFormTMU(ref List<string> outputList, TranMyOrder _TranInfo)
        //{
        //    string ls_formdetail = getformtmu(0);
        //    string Ls_text = "";
        //    int at, start, endcommand, stcommand, count;

        //    string str;
        //    for (int i = 0; i < 6; i++)
        //    {

        //        switch (i)
        //        {
        //            case 0:
        //                start = 0;
        //                Ls_text = "HEADFIX";
        //                at = ls_formdetail.IndexOf("<" + Ls_text + ">", start, ls_formdetail.Length);
        //                if (at > -1)
        //                {
        //                    stcommand = at;
        //                    count = ls_formdetail.Length - stcommand;
        //                    at = ls_formdetail.IndexOf("</" + Ls_text + ">", stcommand, count);
        //                    if (at > -1)
        //                    {
        //                        endcommand = at;
        //                        count = endcommand - stcommand - (Ls_text.Length + 2);
        //                        str = Mid(ls_formdetail, stcommand + Ls_text.Length + 2, count);
        //                        GetDefault(Ls_text, str, ref outputList,_TranInfo);

        //                    }
        //                }
        //                break;

        //            case 1:
        //                start = 0;
        //                Ls_text = "TRANHEAD";
        //                at = ls_formdetail.IndexOf("<" + Ls_text + ">", start, ls_formdetail.Length);
        //                if (at > -1)
        //                {
        //                    stcommand = at;
        //                    count = ls_formdetail.Length - stcommand;
        //                    at = ls_formdetail.IndexOf("</" + Ls_text + ">", stcommand, count);
        //                    if (at > -1)
        //                    {
        //                        endcommand = at;
        //                        count = endcommand - stcommand - (Ls_text.Length + 2);
        //                        str = Mid(ls_formdetail, stcommand + Ls_text.Length + 2, count);
        //                        GetDefault(Ls_text, str, ref outputList,_TranInfo);

        //                    }
        //                }
        //                break;

        //            case 2:
        //                start = 0;
        //                Ls_text = "ITEMS";
        //                at = ls_formdetail.IndexOf("<" + Ls_text + ">", start, ls_formdetail.Length);
        //                if (at > -1)
        //                {
        //                    stcommand = at;
        //                    count = ls_formdetail.Length - stcommand;
        //                    at = ls_formdetail.IndexOf("</" + Ls_text + ">", stcommand, count);
        //                    if (at > -1)
        //                    {
        //                        endcommand = at;
        //                        count = endcommand - stcommand - (Ls_text.Length + 2);
        //                        str = Mid(ls_formdetail, stcommand + Ls_text.Length + 2, count);
        //                        GetDefault(Ls_text, str, ref outputList,_TranInfo);

        //                    }
        //                }
        //                break;

        //            case 3:
        //                start = 0;
        //                Ls_text = "TRANSUM";
        //                at = ls_formdetail.IndexOf("<" + Ls_text + ">", start, ls_formdetail.Length);
        //                if (at > -1)
        //                {
        //                    stcommand = at;
        //                    count = ls_formdetail.Length - stcommand;
        //                    at = ls_formdetail.IndexOf("</" + Ls_text + ">", stcommand, count);
        //                    if (at > -1)
        //                    {
        //                        endcommand = at;
        //                        count = endcommand - stcommand - (Ls_text.Length + 2);
        //                        str = Mid(ls_formdetail, stcommand + Ls_text.Length + 2, count);
        //                        GetDefault(Ls_text, str, ref outputList , _TranInfo);

        //                    }
        //                }
        //                break;

        //            case 4:
        //                start = 0;
        //                Ls_text = "FIXTAIL";
        //                at = ls_formdetail.IndexOf("<" + Ls_text + ">", start, ls_formdetail.Length);
        //                if (at > -1)
        //                {
        //                    stcommand = at;
        //                    count = ls_formdetail.Length - stcommand;
        //                    at = ls_formdetail.IndexOf("</" + Ls_text + ">", stcommand, count);
        //                    if (at > -1)
        //                    {
        //                        endcommand = at;
        //                        count = endcommand - stcommand - (Ls_text.Length + 2);
        //                        str = Mid(ls_formdetail, stcommand + Ls_text.Length + 2, count);
        //                        GetDefault(Ls_text, str, ref outputList, _TranInfo);

        //                    }
        //                }
        //                break;
        //        }

        //    }


        //}


        //public void GetDefault(string dwcase, string dwtext, ref List<string> outputList , TranMyOrder _TranInfo)
        //{

        //    //String st_data, ls_parameter;
        //    //int L_data;
        //    //int L_dataItems;

        //    int start, stcommand, endcommand;
        //    int at;
        //    int end;
        //    int count;

        //   // L_dataItems = -1;
        //    int loopcount = 0;

        //    switch (dwcase.ToLower())
        //    {
        //        case "headfix":
        //            loopcount = 8; //20
        //            break;

        //        case "tranhead":
        //            loopcount = 34;
        //            break;
        //        case "items":
        //            loopcount = 2;
        //            break;
        //        case "transum":
        //            loopcount = 33;
        //            break;
        //        case "fixtail":
        //            loopcount = 5; //20
        //            break;
        //    }
        //    string Ls_text;
        //    start = 0;
        //    string str;
        //    count = dwtext.Length;
        //    stcommand = 0;
        //    for (int k = 0; k < loopcount; k++)
        //    {
        //        Ls_text = k.ToString() ;
             
        //        at = dwtext.IndexOf("<L"+ Ls_text +">", stcommand, count);
        //        if (at > -1)
        //        {
        //            stcommand = at;
        //            count = dwtext.Length - stcommand;
        //            at = dwtext.IndexOf("</L" + k.ToString() + ">", at, count);
                    

        //            if (at > -1)
        //            {
        //                endcommand = at;
                      
        //                count = endcommand - stcommand - (Ls_text.Length + 3);
        //                str = Mid(dwtext, stcommand + Ls_text.Length + 3 , count);

        //                switch (dwcase.ToLower())
        //                {
        //                    case "headfix":
        //                        GetHeadfix(str, ref outputList);
        //                        break;

        //                    case "tranhead":
        //                        GetTranhead(str, ref outputList,_TranInfo);
        //                        break;
        //                    case "items":
        //                       GetItems(str, ref outputList);
        //                        break;
        //                    case "transum":
        //                       GetTranSum(str, ref outputList, _TranInfo);
        //                        break;
        //                    case "fixtail":
        //                        GetHeadfix(str, ref outputList);
        //                        break;
        //                }
                        
   
        //                stcommand = at;
        //                count = dwtext.Length - stcommand;
        //            }
        //        }
        //        else { }

        //    }

        //}


        public void GetHeadfix(string dwtext, ref List<string> outputList)
        {
            int numcol = 9;

            string Ls_text;
            string str;
            int count = dwtext.Length;
            int stcommand = 0;
            int endcommand;
            int at;

            for (int k = 0; k < numcol; k++)
            {

                Ls_text = k.ToString();
                at = dwtext.IndexOf("<" + Ls_text + ">", stcommand, count);
                if (at > -1)
                {
                    stcommand = at;
                    count = dwtext.Length - stcommand;
                    at = dwtext.IndexOf("</" + k.ToString() + ">", at, count);


                    if (at > -1)
                    {
                        endcommand = at;

                        count = endcommand - stcommand - (Ls_text.Length + 2);
                        str = Mid(dwtext, stcommand + Ls_text.Length + 2, count);

                        switch (k)
                        {
                            case 0: //newline ตามจำนวนที่ป้อน , ใช้ร่วมกับ FIXTAIL
                                int j = Convert.ToInt32(str);
                                if (j == 0)
                                { outputList.Add("-----------------------------------------"); }
                                else
                                {
                                    for (int i = 0; i < j; i++)
                                    {
                                        outputList.Add("newline");
                                    }
                                }
                                k = numcol;
                                break;
                            case 1: //ข้อความ(ภาษาไทย) ดึงจาก file
                            case 2: //ข้อความ(อังกฤษ) ดึงจาก file , ใช้ร่วมกับ FIXTAIL
                            case 3: //ชื่อบริษัท(ไทย) ดึงจาก file
                            case 4: //ชื่อบริษัท(อังกฤษ) ดึงจาก file
                            case 5: //select ที่อยู่
                            case 6: //select Tel
                            case 7: //select เลขประจำตัวผู้เสียภาษี
                            case 8: //select PMI
                                outputList.Add(str);
                                k = numcol;
                                break;
                        }
                        stcommand = at;
                        count = dwtext.Length - stcommand;
                    }
                }
               
            
            }
        }

        //public void GetTranhead(string dwtext, ref List<string> outputList, TranMyOrder _TranInfo)
        //{
        //    int numcol = 35;

        //    string Ls_text;
        //    string str;
        //    int count = dwtext.Length;
        //    int stcommand = 0;
        //    int endcommand;
        //    int at;

        //    for (int k = 0; k < numcol; k++)
        //    {

        //        Ls_text = k.ToString();
        //        at = dwtext.IndexOf("<" + Ls_text + ">", stcommand, count);
        //        if (at > -1)
        //        {
        //            stcommand = at;
        //            count = dwtext.Length - stcommand;
        //            at = dwtext.IndexOf("</" + k.ToString() + ">", at, count);


        //            if (at > -1)
        //            {
        //                endcommand = at;

        //                count = endcommand - stcommand - (Ls_text.Length + 2);
        //                str = Mid(dwtext, stcommand + Ls_text.Length + 2, count);

        //                switch (k)
        //                {
        //                    case 0: //newline ตามจำนวนที่ป้อน
        //                        int j = Convert.ToInt32(str);
        //                        if (j == 0)
        //                        { outputList.Add("-----------------------------------------"); }
        //                        else
        //                        {
        //                            for (int i = 0; i < j; i++)
        //                            {
        //                                outputList.Add("newline");
        //                            }
        //                        }
        //                        k = numcol;
        //                        break;
        //                    case 1: //ชื่อใบสำคัญ(ภาษาไทย)
        //                        outputList.Add(ReplaceSpacebar(_TranInfo.VOUCHERIDNAMETH));
        //                        k = numcol;
        //                        break;
        //                    case 2: //ชื่อใบสำคัญ(ภาษาอังกฤษ)
        //                        outputList.Add(ReplaceSpacebar(_TranInfo.VOUCHERIDNAMEEN));
        //                        k = numcol;
        //                        break;
        //                    case 3: //เลขที่เอกสาร(ชิดซ้าย)
        //                        outputList.Add(ReplaceSpacebar(_TranInfo.TRANNO));
        //                        k = numcol;
        //                        break;
        //                    case 4: //เลขที่เอกสาร(กึ่งกลาง)
        //                        outputList.Add(ReplaceSpacebar(_TranInfo.TRANNO,1));
        //                        k = numcol;
        //                        break;
        //                    case 5: //เลขที่เอกสาร(ชิดขวา)
        //                        outputList.Add(ReplaceSpacebar(_TranInfo.TRANNO, 2));
        //                        k = numcol;
        //                        break;
        //                    case 6: //cashier(ชิดซ้าย)
        //                        outputList.Add(ReplaceSpacebar(_TranInfo.USERIDSALE));
        //                        k = numcol;
        //                        break;
        //                    case 7: //cashier(กึ่งกลาง)
        //                        outputList.Add(ReplaceSpacebar(_TranInfo.USERIDSALE, 1));
        //                        k = numcol;
        //                        break;
        //                    case 8: //cashier(ชิดขวา)
        //                        outputList.Add(ReplaceSpacebar(_TranInfo.USERIDSALE, 2));
        //                        k = numcol;
        //                        break;
        //                    case 9: //PMI
        //                        //outputList.Add(ReplaceSpacebar2("PMI  XXXXXXXXXXXX","25/10/2562"));
        //                        k = numcol;
        //                        break;
        //                    case 10: //เลขที่เอกสาร
        //                        outputList.Add(ReplaceSpacebar(_TranInfo.TRANNO));
        //                        k = numcol;
        //                        break;
        //                    case 11: //Date(ชิดซ้าย)
        //                        outputList.Add(ReplaceSpacebar(_TranInfo.STRANDATE));
        //                        k = numcol;
        //                        break;
        //                    case 12: //Date(กึ่งกลาง)
        //                        outputList.Add(ReplaceSpacebar(_TranInfo.STRANDATE,1));
        //                        k = numcol;
        //                        break;
        //                    case 13: //Date(ชิดขวา)
        //                        outputList.Add(ReplaceSpacebar(_TranInfo.STRANDATE,2));
        //                        k = numcol;
        //                        break;
                           
        //                    case 17: //Time(ชิดซ้าย)
        //                        outputList.Add(ReplaceSpacebar(_TranInfo.STIME));
        //                        k = numcol;
        //                        break;
        //                    case 18: //Time(กึ่งกลาง)
        //                        outputList.Add(ReplaceSpacebar(_TranInfo.STIME,1));
        //                        k = numcol;
        //                        break;
        //                    case 19: //Time(ชิดขวา)
        //                        outputList.Add(ReplaceSpacebar(_TranInfo.STIME,2));
        //                        k = numcol;
        //                        break;

        //                    case 14: //เลขที่เอกสาร
        //                    case 21: //เลขที่เอกสาร
        //                        outputList.Add(ReplaceSpacebar(_TranInfo.TRANNO, 0));
        //                        k = numcol;
        //                        break;
        //                    case 15: //TAXID
        //                    case 22: //TAXID
        //                        outputList.Add(ReplaceSpacebar("TAXID:"+_TranInfo.TAXID));
        //                        k = numcol;
        //                        break;
        //                    case 23: //รหัสลูกค้า 
        //                        outputList.Add(ReplaceSpacebar(_TranInfo.PERSONID));
        //                        k = numcol;
        //                        break;
        //                    case 24: //ชื่อ-สกุลลูกค้า
        //                        outputList.Add(ReplaceSpacebar(_TranInfo.FULLNAME));
        //                         k = numcol;
        //                        break;
        //                    case 25: //ข้อความ(ไทย)
        //                        outputList.Add(str);
        //                        k = numcol;
        //                        break;
        //                    case 26: //ข้อความ(อังกฤษ)
        //                        outputList.Add(str);
        //                        k = numcol;
        //                        break;
        //                    case 16: //Cashier
        //                    case 20: //Cashier
        //                    case 27: //Sale(ชิดซ้าย)
        //                    case 30: //Sale
        //                        outputList.Add(ReplaceSpacebar(_TranInfo.USERIDSALE));
        //                        k = numcol;
        //                        break;
        //                    case 28: //Sale(กึ่งกลาง)
        //                        outputList.Add(ReplaceSpacebar(_TranInfo.USERIDSALE,1));
        //                        k = numcol;
        //                        break;
        //                    case 29: //Sale(ชิดขวา)
        //                        outputList.Add(ReplaceSpacebar(_TranInfo.USERIDSALE, 2));
        //                        k = numcol;
        //                        break;
        //                    case 31: //ที่อยู่ลูกค้า
        //                        outputList.Add(ReplaceSpacebar(_TranInfo.PERSONADDRESS1));
        //                        k = numcol;
        //                        break;
        //                    case 32: //POS ID(ชิดซ้าย)
        //                        outputList.Add(ReplaceSpacebar(_TranInfo.POSID));
        //                         k = numcol;
        //                        break;
        //                    case 33: //POS ID(กึ่งกลาง)
        //                        outputList.Add(ReplaceSpacebar(_TranInfo.POSID,1));
        //                        k = numcol;
        //                        break;
        //                    case 34: //POS ID(ชิดขวา)
        //                        outputList.Add(ReplaceSpacebar(_TranInfo.POSID, 2));
        //                        k = numcol;
        //                        break;
        //                }
        //                stcommand = at;
        //                count = dwtext.Length - stcommand;
        //            }
        //        }
        //    }
        //}

        public void GetItems(string dwtext, ref List<string> outputList)
        {
            int numcol = 4;

            string Ls_text;
            string str;
            int count = dwtext.Length;
            int stcommand = 0;
            int endcommand;
            int at;

            for (int k = 0; k < numcol; k++)
            {

                Ls_text = k.ToString();
                at = dwtext.IndexOf("<" + Ls_text + ">", stcommand, count);
                if (at > -1)
                {
                    stcommand = at;
                    count = dwtext.Length - stcommand;
                    at = dwtext.IndexOf("</" + k.ToString() + ">", at, count);


                    if (at > -1)
                    {
                        endcommand = at;

                        count = endcommand - stcommand - (Ls_text.Length + 2);
                        str = Mid(dwtext, stcommand + Ls_text.Length + 2, count);

                        switch (k)
                        {
                            case 0: //newline ตามจำนวนที่ป้อน 
                                int j = Convert.ToInt32(str);
                                if (j == 0)
                                { outputList.Add("-----------------------------------------"); }
                                else
                                {
                                    for (int i = 0; i < j; i++)
                                    {
                                        outputList.Add("newline");
                                    }
                                }
                                k = numcol;
                                break;
                            case 1: //แบบที่1

                            case 2: //แบบที่2

                            case 3: //แบบที่3
                               
                                outputList.Add("กาแฟ                       Psc.     700.00");
                                outputList.Add("    7.00 X @100.00                        ");
                                outputList.Add("นมสด                       Pcs.   1,200.00");
                                outputList.Add("    12.00 X @100.00                       ");
                                outputList.Add("ชานม                       Pcs.     800.00");
                                outputList.Add("    8.00 X @100.00                        ");
                                outputList.Add("ลิปตัน                       Pcs.      50.00");
                                outputList.Add("ชาเขียว                      Pcs.      10.00");
                                outputList.Add("ชานม                       Pcs.     200.00");
                                outputList.Add("    2.00 X @100.00                        ");
                                k = numcol;
                                break;

                          

                        }
                        stcommand = at;
                        count = dwtext.Length - stcommand;
                    }
                }


            }
        }

        //public void GetTranSum(string dwtext, ref List<string> outputList, TranMyOrder _TranInfo)
        //{
        //    int numcol = 34;

        //    string Ls_text;
        //    string str;
        //    int count = dwtext.Length;
        //    int stcommand = 0;
        //    int endcommand;
        //    int at;

        //    for (int k = 0; k < numcol; k++)
        //    {

        //        Ls_text = k.ToString();
        //        at = dwtext.IndexOf("<" + Ls_text + ">", stcommand, count);
        //        if (at > -1)
        //        {
        //            stcommand = at;
        //            count = dwtext.Length - stcommand;
        //            at = dwtext.IndexOf("</" + k.ToString() + ">", at, count);


        //            if (at > -1)
        //            {
        //                endcommand = at;

        //                count = endcommand - stcommand - (Ls_text.Length + 2);
        //                str = Mid(dwtext, stcommand + Ls_text.Length + 2, count);

        //                switch (k)
        //                {
        //                    case 0: //newline ตามจำนวนที่ป้อน
        //                        int j = Convert.ToInt32(str);
        //                        if (j == 0)
        //                        { outputList.Add("-----------------------------------------"); }
        //                        else
        //                        {
        //                            for (int i = 0; i < j; i++)
        //                            {
        //                                outputList.Add("newline");
        //                            }
        //                        }
        //                        k = numcol;
        //                        break;
        //                    case 1: //Total
        //                        outputList.Add(ReplaceSpacebar2("TOTAL", _TranInfo.total));
        //                        k = numcol;
        //                        break;
        //                    case 2: //Discount
        //                    case 3: //TXBL
        //                    case 4: //VAT     
        //                        outputList.Add(ReplaceSpacebar2("TOTALVAT",_TranInfo.totalVat));
        //                        k = numcol;
        //                        break;
        //                    case 5: //Grand Total
        //                        outputList.Add(ReplaceSpacebar2("GRAND TOTAL",_TranInfo.grandTotal));
        //                        k = numcol;
        //                        break;
        //                    case 6: //Cash
        //                        outputList.Add(ReplaceSpacebar2("CASH", _TranInfo.CASH));
        //                        k = numcol;
        //                        break;
        //                    case 7: //Change
        //                        outputList.Add(ReplaceSpacebar2("CHANGE", _TranInfo.CHANGE));
        //                        k = numcol;
        //                        break;
        //                    case 8: //Cash:#,###.00 Change : #,###.00
        //                    case 9: //Item sale
        //                    case 10: //Credit Card ######## Cash:#,###.00
        //                    case 11: //Coupon
        //                    case 12: //รวมเงิน
        //                    case 13: //ส่วนลด
        //                    case 14: //มูลค่าสินค้าคิด Vat
        //                    case 15: //ภาษีมูลค่าเพิ่ม
        //                    case 16: //ยอดสุทธิ
        //                    case 17: //จ่าย
        //                    case 18: //ทอน
        //                    case 19: //
        //                    case 20: //รวมรายการ  รวมจำนวนสินค้า
        //                    case 21: //เงินสด 0.00 บาท ทอน 0.00 บาท
        //                    case 22: //Credit Card Detail
        //                    case 23: //Sub total
        //                    case 24: //Score Point
        //                    case 25: //Point of Bill
        //                    case 26: //Discount Promotion
        //                    case 27: //ส่วนลดโปรโมชั่น
        //                    case 28: //Service Charge
        //                    case 29: //ส่วนลด(ไม่รวมโปรโมชั่น)
        //                    case 30: //มูลค่าสินค้าทั้งสิ้น
        //                    case 31: //ส่วนลดรวม
        //                    case 32: //Discount All
        //                    case 33: //มูลค่าสินค้า ไม่คิด Vat
                              
        //                        k = numcol;
        //                        break;
        //                }
        //                stcommand = at;
        //                count = dwtext.Length - stcommand;
        //            }
        //        }


        //    }
        //}

        private String Mid(String Param, int StartIndex, int Length)
        {
            String Result = Param.Substring(StartIndex, Length);
            return Result;
        }
        

        private string ReplaceSpacebar(string data)
        {
            string spacebar = "";
            if (printsize == 1)
            {
                spacebar = spacebar48;
            }
            else
            {
                spacebar = spacebar80;
            }
            if (spacebar.Length > data.Length)
            {
                int l = spacebar.Length - data.Length;
                data = data + Mid(spacebar, 1, l);
            }
            return data;
        }

        private string ReplaceSpacebar(string data,int opt)
        {
            string spacebar = "" ;
            if (printsize == 1)
            {
                spacebar = spacebar48;
            }
            else
            {
                spacebar = spacebar80;
            }
            if (spacebar.Length > data.Length)
            {
                int l = spacebar.Length - data.Length;

                switch (opt)
                {
                    case 1: //center
                        break;
                    case 2: //Rieght
                        data = Mid(spacebar, 1, l) + data;
                        break;
                    default: //Left
                        data = data + Mid(spacebar, 1, l);
                        break;
                }

            }

            return data;
        }

        public string ReplaceSpacebar2(string data1,string data2)
        {
            string spacebar = "";
            if (printsize == 1)
            {
                spacebar = spacebar48;
            }
            else
            {
                spacebar = spacebar80;
            }
            if (spacebar.Length > (ThaiLength(data1) + ThaiLength(data2)))
            {
                int l = spacebar.Length - ThaiLength(data1) - ThaiLength(data2);
                data1 = data1 + Mid(spacebar, 1, l) + data2;
            }
            else
            {
                data1 = data1 + " " + data2;
            }
            return data1;
        }
        public int ThaiLength(string stringthai)
        {
            int len = 0;
            int l = stringthai.Length;
            for (int i = 0; i < l; ++i)
            {
                if (char.GetUnicodeCategory(stringthai[i]) != System.Globalization.UnicodeCategory.NonSpacingMark)
                    ++len;
            }
            return len;
        }
        public string getformtmu(int index)
        {
            string form = "";
            switch (index)
            {
                case 0:
                    form = "<Vertion>9</Vertion> " +
                            "<HEADFIX>" +
                            "<L2><3>SeniorSoft Development .co.,ltd.</3></L2>" +
                            "<L3><5>... Payless, Higher Quality ...</5></L3>" +
                            "<L4><1>Tel. 0 - 2999 - 9999, 0 - 2999 - 9990</1></L4>" +
                            "<L5><0>0</0></L5>" +
                            "</HEADFIX>" +
                            "<TRANHEAD>" +
                            "<L2><5>none</5></L2>" +
                            "<L4><9>none</9></L4>" +
                            "<L5><23>none</23></L5>" +
                            "<L6><24>none</24></L6>" +
                            "<L9><19>TIME Rieght</19></L9>" +
                            "<L10><0>0</0></L10>" +
                            "</TRANHEAD>" +
                            "<ITEMS>" +
                            "<L1><1>Items list</1></L1>" +
                            "</ITEMS>" +
                            "<TRANSUM>" +
                            "<L1><0>0</0></L1>" +
                            "<L2><1>none</1></L2>" +
                            "<L3><4>none</4></L3>" +
                            "<L4><5>none</5></L4>" +
                            "<L5><6>none</6></L5>" +
                            "<L6><7>none</7></L6>" +
                            "<L7><0>0</0></L7>" +
                            "</TRANSUM>" +
                            "<FIXTAIL>" +
                            "<L2><2>VAT INCLUDE</2></L2>" +
                            "<L3><2>THANK YOU FOR YOU SHOPPING</2></L3>" +
                            "<L4><0>3</0></L4>" +
                            "</FIXTAIL>" +
                            "<group>" +
                            "<langth>Slip Port</langth>" +
                            "<langen>ENSlip Port</langen>" +
                            "<langch>CHSlip Port</langch>" +
                            "</group>" +
                            "<name>" +
                            "<langth>TMU_Temp</langth>" +
                            "<langen>ENTMU_Temp</langen>" +
                            "<langch>CHTMU_Temp</langch>" +
                            "</name>";
                    break;
            }

            return form;
        }

    }
}
