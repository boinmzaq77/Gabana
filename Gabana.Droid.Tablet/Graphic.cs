
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Gabana.ShareSource.ClassStructure;
using Java.Nio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Xamarin.Forms;
using Rect = Android.Graphics.Rect;

namespace Gabana.Droid.Tablet
{
    public class Graphic
    {
        string strParam = string.Empty;
        public byte[] DrawImageFromXml(string xmlName, ParamSlip paramSlip, AssetManager assets)
        {// method อ่าน xml เพื่อสร้าง image และ gen เป็น bytes
            // Create bitmap empty
            Bitmap bitmap = null;
            Canvas canvas = null;
            byte[] bytesData;
            try
            {

                XmlDocument doc = new XmlDocument();
                doc.Load(assets.Open(xmlName));
                int slipWidht = doc.DocumentElement.Attributes["Width"] != null ? Convert.ToInt32(doc.DocumentElement.Attributes["Width"].Value) : 0;
                bitmap = Bitmap.CreateBitmap(slipWidht, 50000, Bitmap.Config.Argb8888); // create ให้ height ยาวไว้ก่อน                  
                canvas = new Canvas(bitmap);
                canvas.DrawColor(Android.Graphics.Color.White);
                XmlNodeList xmlNodeList = doc.DocumentElement.ChildNodes;
                int posY = 0;
                int lineSpace = doc.DocumentElement.Attributes["LineSpace"] != null ? Convert.ToInt32(doc.DocumentElement.Attributes["LineSpace"].Value) : 0;
                foreach (XmlNode nodeMain in xmlNodeList) // Header , Details , Footer
                {
                    int loopDraw = nodeMain.Name.ToLower() == "details" ? paramSlip.Details.Count : 1;
                    for (int i = 0; i < loopDraw; i++)
                    {
                        foreach (XmlNode nodeLine in nodeMain.SelectNodes("Line")) // Line
                        {
                            int lineHeight = 0;
                            int lineTextSize = nodeLine.Attributes["TextSize"] != null ? Convert.ToInt32(nodeLine.Attributes["TextSize"].Value) : 0;
                            TypefaceStyle lineTypefaceStyle = TypefaceStyle.Normal;
                            if (nodeLine.Attributes["Bold"] != null)
                            {
                                if (nodeLine.Attributes["Bold"].Value.ToLower() == "true")
                                {
                                    lineTypefaceStyle = TypefaceStyle.Bold;
                                }
                            }
                            foreach (XmlNode nodeText in nodeLine.SelectNodes("Text")) // Text
                            {
                                int textSize;
                                if (lineTextSize == 0)
                                {
                                    textSize = nodeText.Attributes["TextSize"] != null ? Convert.ToInt32(nodeText.Attributes["TextSize"].Value) : 0;
                                }
                                else
                                {
                                    textSize = lineTextSize;
                                }

                                TypefaceStyle typefaceStyle = TypefaceStyle.Normal;
                                if (lineTypefaceStyle != TypefaceStyle.Normal)
                                {
                                    typefaceStyle = lineTypefaceStyle;
                                }
                                else
                                {
                                    if (nodeText.Attributes["Bold"] != null)
                                    {
                                        if (nodeText.Attributes["Bold"].Value.ToLower() == "true")
                                        {
                                            typefaceStyle = TypefaceStyle.Bold;
                                        }
                                    }
                                }
                                int xText = nodeText.Attributes["X"] != null ? Convert.ToInt32(nodeText.Attributes["X"].Value) : 0;
                                int textWidth = nodeText.Attributes["Width"] != null ? Convert.ToInt32(nodeText.Attributes["Width"].Value) : 0;
                                int xAlign = 0;

                                Paint.Align align = Paint.Align.Left;
                                string alignValue = nodeText.Attributes["Align"] != null ? nodeText.Attributes["Align"].Value : "Left";
                                switch (alignValue.ToLower())
                                {
                                    case "center":
                                        align = Paint.Align.Center;
                                        xAlign = (textWidth / 2);
                                        break;
                                    case "left":
                                        align = Paint.Align.Left;
                                        xAlign = 0;
                                        break;
                                    case "right":
                                        align = Paint.Align.Right;
                                        xAlign = textWidth;
                                        break;
                                    default:
                                        break;
                                }
                                using (Paint p = new Paint())
                                {
                                    using (Typeface tf = Typeface.Create(Xamarin.Forms.Font.Default.FontFamily, typefaceStyle))
                                    {
                                        Android.Graphics.Rect rect = new Android.Graphics.Rect();
                                        p.SetTypeface(tf);
                                        if (textSize != 0)
                                            p.TextSize = textSize;
                                        p.Color = Android.Graphics.Color.Black;
                                        p.TextAlign = align;
                                        string value = WrapText(ReplaceText(nodeText.LastChild.Value, nodeMain.Name, paramSlip, i), xText, textWidth, p);
                                        if (String.IsNullOrEmpty(value)) value = " ";
                                        p.GetTextBounds(value, 0, value.Length, rect);
                                        lineHeight = lineHeight < rect.Height() ? rect.Height() : lineHeight;

                                        if ((strParam == "@ToppingName" || strParam == "@ToppingPrice" || strParam == "@Comment"))
                                        {
                                            if (!string.IsNullOrEmpty(value) && value != " ")
                                            {
                                                lineHeight = -6;
                                                p.Color = Android.Graphics.Color.Black;
                                            }
                                            else
                                            {
                                                //lineHeight = -5;
                                                break;
                                            }
                                        }

                                        int yPrint = posY + lineHeight;
                                        //canvas.DrawLine(xText, yPrint, xText + textWidth, yPrint,new Paint() {StrokeWidth = 3 });
                                        canvas.DrawText(value, xAlign + xText, yPrint, p);
                                    }
                                }
                            }

                            posY += lineHeight + lineSpace;
                        }
                    }
                }
                bitmap.Height = posY + 0;// สามารถเปลี่ยน height ให้น้อยลงได้ แต่มากกว่า ตอน createไม่ได้
                bytesData = convertBitmapToByteArrayUncompressed(bitmap);
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                if (canvas != null)
                    canvas.Dispose();
                if (bitmap != null)
                    bitmap.Dispose();
            }
            return bytesData;

            //using (var reader = XmlReader.Create(Forms.Context.Assets.Open(xmlName)))
            //{

            //    --------------------------------------------read all line---------------------------------------
            //   int posY = 0;
            //    int LineHeight = 0;
            //    float textSize = 0;
            //    Paint.Align align = Paint.Align.Left;
            //    int slipWidth = 0;
            //    int textWidth = 0;
            //    int lineSpace = 0; // ระยะห่างของ text แต่ละ line
            //    int xText = 0; // x ของ <Text>
            //    int xAlign = 0; // x ของ align ที่กำหนด เช่น center ก็จะเอา slipWidth / 2 
            //    bool isUnderItems = false; //หากอยู่ใน tag items จะทำการ loop ข้อมูลจาก struct items
            //    while (reader.Read())
            //    {
            //        switch (reader.NodeType)
            //        {
            //            case XmlNodeType.None:
            //                break;
            //            case XmlNodeType.Element:
            //                if (reader.HasAttributes)
            //                {
            //                    string elementName = reader.LocalName.ToLower();
            //                    if (elementName == "items")
            //                        isUnderItems = true;
            //                    while (reader.MoveToNextAttribute())
            //                    {
            //                        switch (reader.Name.ToLower())
            //                        {
            //                            case "width":
            //                                if (elementName == "sliplayout")
            //                                {
            //                                    slipWidth = Convert.ToInt32(reader.Value);
            //                                }
            //                                else if (elementName == "text")
            //                                {
            //                                    textWidth = Convert.ToInt32(reader.Value);
            //                                }
            //                                break;
            //                            case "textsize":
            //                                textSize = Convert.ToInt32(reader.Value);
            //                                break;
            //                            case "align":
            //                                switch (reader.Value.ToLower())
            //                                {
            //                                    case "center":
            //                                        align = Paint.Align.Center;
            //                                        break;
            //                                    case "left":
            //                                        align = Paint.Align.Left;
            //                                        break;
            //                                    case "right":
            //                                        align = Paint.Align.Right;
            //                                        break;
            //                                    default:
            //                                        break;
            //                                }
            //                                break;
            //                            case "linespace":
            //                                lineSpace = Convert.ToInt32(reader.Value);
            //                                break;
            //                            case "x":
            //                                xText = Convert.ToInt32(reader.Value);
            //                                break;
            //                            default:
            //                                break;
            //                        }
            //                    }
            //                }

            //                break;
            //            case XmlNodeType.Attribute:
            //                break;
            //            case XmlNodeType.Text:

            //                using (Paint p = new Paint())
            //                {
            //                    using (Typeface tf = Typeface.Create(Xamarin.Forms.Font.Default.FontFamily, TypefaceStyle.Bold))
            //                    {
            //                        Rect rect = new Rect();
            //                        p.SetTypeface(tf);
            //                        if (textSize != 0)
            //                            p.TextSize = textSize;

            //                        p.Color = Android.Graphics.Color.Black;
            //                        p.TextAlign = align;
            //                        string value = WrapText(reader.Value, xText, textWidth, p);
            //                        p.GetTextBounds(value, 0, value.Length, rect);
            //                        LineHeight = LineHeight < rect.Height() ? rect.Height() : LineHeight;
            //                        //int height = rect.Height();
            //                        int yPrint = posY + LineHeight;
            //                        //posY = posY == 0 ? posY + rect.Height() : posY;
            //                        if (align == Paint.Align.Center)
            //                            xAlign = (textWidth / 2);
            //                        else if (align == Paint.Align.Left)
            //                            xAlign = 0;
            //                        else if (align == Paint.Align.Right)
            //                            xAlign = textWidth;
            //                        //canvas.DrawLine(xText, yPrint, xText + textWidth, yPrint,new Paint() {StrokeWidth = 3 });
            //                        canvas.DrawText(value, xAlign + xText, yPrint, p);

            //                    }
            //                }
            //                break;

            //            case XmlNodeType.EndElement:
            //                if (reader.Name.ToLower() == "line")
            //                {
            //                    posY += LineHeight + lineSpace;
            //                    textSize = 0;
            //                    align = Paint.Align.Left;
            //                    LineHeight = 0;
            //                    xText = 0;
            //                }
            //                if (reader.Name.ToLower() == "items") // ออกจาก loop items
            //                    isUnderItems = false;
            //                break;
            //            default:
            //                break;
            //        }
            //    }

            //    var byteArr = convertBitmapToByteArrayUncompressed(bitmap);
            //    return byteArr;

            //}
        }

        public Bitmap ImageFromXml(string xmlName, ParamSlip paramSlip, AssetManager assets)
        {// method อ่าน xml เพื่อสร้าง image และ gen เป็น bytes
            // Create bitmap empty
            Bitmap bitmap = null;
            Canvas canvas = null;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(assets.Open(xmlName));
                int slipWidht = doc.DocumentElement.Attributes["Width"] != null ? Convert.ToInt32(doc.DocumentElement.Attributes["Width"].Value) : 0;
                bitmap = Bitmap.CreateBitmap(slipWidht, 50000, Bitmap.Config.Argb8888); // create ให้ height ยาวไว้ก่อน
                canvas = new Canvas(bitmap);
                canvas.DrawColor(Android.Graphics.Color.White);
                XmlNodeList xmlNodeList = doc.DocumentElement.ChildNodes;
                int posY = 0;
                int lineSpace = doc.DocumentElement.Attributes["LineSpace"] != null ? Convert.ToInt32(doc.DocumentElement.Attributes["LineSpace"].Value) : 0;
                foreach (XmlNode nodeMain in xmlNodeList) // Header , Details , Footer
                {
                    int loopDraw = nodeMain.Name.ToLower() == "details" ? paramSlip.Details.Count : 1;
                    for (int i = 0; i < loopDraw; i++)
                    {
                        foreach (XmlNode nodeLine in nodeMain.SelectNodes("Line")) // Line
                        {
                            int lineHeight = 0;
                            int lineTextSize = nodeLine.Attributes["TextSize"] != null ? Convert.ToInt32(nodeLine.Attributes["TextSize"].Value) : 0;
                            TypefaceStyle lineTypefaceStyle = TypefaceStyle.Normal;
                            if (nodeLine.Attributes["Bold"] != null)
                            {
                                if (nodeLine.Attributes["Bold"].Value.ToLower() == "true")
                                {
                                    lineTypefaceStyle = TypefaceStyle.Bold;
                                }
                            }
                            foreach (XmlNode nodeText in nodeLine.SelectNodes("Text")) // Text
                            {
                                int textSize;
                                if (lineTextSize == 0)
                                {
                                    textSize = nodeText.Attributes["TextSize"] != null ? Convert.ToInt32(nodeText.Attributes["TextSize"].Value) : 0;
                                }
                                else
                                {
                                    textSize = lineTextSize;
                                }

                                TypefaceStyle typefaceStyle = TypefaceStyle.Normal;
                                if (lineTypefaceStyle != TypefaceStyle.Normal)
                                {
                                    typefaceStyle = lineTypefaceStyle;
                                }
                                else
                                {
                                    if (nodeText.Attributes["Bold"] != null)
                                    {
                                        if (nodeText.Attributes["Bold"].Value.ToLower() == "true")
                                        {
                                            typefaceStyle = TypefaceStyle.Bold;
                                        }
                                    }
                                }
                                int xText = nodeText.Attributes["X"] != null ? Convert.ToInt32(nodeText.Attributes["X"].Value) : 0;
                                int textWidth = nodeText.Attributes["Width"] != null ? Convert.ToInt32(nodeText.Attributes["Width"].Value) : 0;
                                int xAlign = 0;

                                Paint.Align align = Paint.Align.Left;
                                string alignValue = nodeText.Attributes["Align"] != null ? nodeText.Attributes["Align"].Value : "Left";
                                switch (alignValue.ToLower())
                                {
                                    case "center":
                                        align = Paint.Align.Center;
                                        xAlign = (textWidth / 2);
                                        break;
                                    case "left":
                                        align = Paint.Align.Left;
                                        xAlign = 0;
                                        break;
                                    case "right":
                                        align = Paint.Align.Right;
                                        xAlign = textWidth;
                                        break;
                                    default:
                                        break;
                                }
                                using (Paint p = new Paint())
                                {
                                    using (Typeface tf = Typeface.Create(Xamarin.Forms.Font.Default.FontFamily, typefaceStyle))
                                    {
                                        Android.Graphics.Rect rect = new Android.Graphics.Rect();
                                        p.SetTypeface(tf);
                                        if (textSize != 0)
                                            p.TextSize = textSize;
                                        p.Color = Android.Graphics.Color.Black;
                                        p.TextAlign = align;
                                        string value = WrapText(ReplaceText(nodeText.LastChild.Value, nodeMain.Name, paramSlip, i), xText, textWidth, p);
                                        if (String.IsNullOrEmpty(value)) value = " ";
                                        p.GetTextBounds(value, 0, value.Length, rect);
                                        lineHeight = lineHeight < rect.Height() ? rect.Height() : lineHeight;

                                        //กรณีไม่มีค่า
                                        //MerchantAddress ,TelNumber ,CustomerAddress , Discount member, Discount, Vat , Service
                                        if (strParam == "@merchantAddress1" || strParam == "@merchantAddress2" || strParam == "@merchantAddress3"
                                            || strParam == "@merchantTel" || strParam == "@Address1" || strParam == "@Address2")
                                        {
                                            if (string.IsNullOrWhiteSpace(value))
                                            {
                                                p.TextSize = 0;
                                                lineHeight = -5; //-12 ,-8
                                            }
                                            else
                                            {
                                                lineHeight = 20;
                                            }
                                        }

                                        //TranDetailTopping
                                        if (strParam == "@Quantity" || strParam == "@ItemName" || strParam == "@ItemPrice")
                                        {
                                            if (!string.IsNullOrWhiteSpace(value))
                                            {
                                                lineHeight = 22;
                                            }
                                            else
                                            {
                                                p.TextSize = 0;
                                                lineHeight = -2;
                                            }
                                        }

                                        if (strParam == "@ToppingName" || strParam == "@ToppingPrice" || strParam == "@Comment")
                                        {
                                            if (!string.IsNullOrWhiteSpace(value))
                                            {
                                                lineHeight = 10;
                                                p.Color = Android.Graphics.Color.Gray;
                                            }
                                            else
                                            {
                                                p.TextSize = 0;
                                                lineHeight = -12;
                                            }
                                        }

                                        if (strParam == "@CountDetail" || strParam == "@SumQuantity")
                                        {
                                            lineHeight = 20;
                                        }

                                        if (strParam == "@Member1" || strParam == "@Member2" || strParam == "@Member3"
                                        || strParam == "@Vat1" || strParam == "@Vat2" || strParam == "@Vat2"
                                        || strParam == "@Discount1" || strParam == "@Discount2" || strParam == "@Discount3"
                                        || strParam == "@Service1" || strParam == "@Service2" || strParam == "@Service3"
                                        || strParam == "@GrandTotal1" || strParam == "@GrandTotal2" || strParam == "@GrandTotal3"
                                        || strParam == "@Total1" || strParam == "@Total2" || strParam == "@Total3")
                                        {
                                            if (string.IsNullOrWhiteSpace(value))
                                            {
                                                p.TextSize = 0;
                                                lineHeight = -10;
                                            }
                                            else
                                            {
                                                lineHeight = 22;
                                            }
                                        }

                                        int yPrint = posY + lineHeight;
                                        //canvas.DrawLine(xText, yPrint, xText + textWidth, yPrint,new Paint() {StrokeWidth = 3 });

                                        canvas.DrawText(value, xAlign + xText, yPrint, p);
                                    }

                                }
                            }

                            posY += lineHeight + lineSpace;
                        }
                    }
                }
                bitmap.Height = posY + 0;// สามารถเปลี่ยน height ให้น้อยลงได้ แต่มากกว่า ตอน createไม่ได้
                return bitmap;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public byte[] DrawString()
        {

            //Bitmap img = new Bitmap();
            BitmapDrawable bitmapDrawable = new BitmapDrawable();
            Bitmap bitmap = Bitmap.CreateBitmap(384, 850, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(bitmap);

            canvas.DrawColor(Android.Graphics.Color.White);

            int posY = 0;

            using (Paint p = new Paint())
            {
                string Header = "ซีเนียร์ซอฟท์ ดีเวลลอปเม้นท์";

                using (Typeface tf = Typeface.CreateFromAsset(Forms.Context.Assets, "fonts/TAHOMA.TTF"))
                {
                    Rect rect = new Rect();
                    p.SetTypeface(tf);
                    p.TextSize = 26;
                    p.Color = Android.Graphics.Color.Black;
                    p.TextAlign = Paint.Align.Center;
                    p.GetTextBounds(Header, 0, Header.Length, rect);
                    int height = rect.Height();
                    posY += height;
                    canvas.DrawText(Header, (384 / 2), posY, p);
                }
            }
            using (Paint p = new Paint())
            {
                string Header = "ซีเนียร์ซอฟท์ ดีเวลลอปเม้นท์";

                using (Typeface tf = Typeface.CreateFromAsset(Forms.Context.Assets, "fonts/UPCKB.TTF"))
                {
                    Rect rect = new Rect();
                    p.SetTypeface(tf);
                    p.TextSize = 38;
                    p.Color = Android.Graphics.Color.Black;
                    p.TextAlign = Paint.Align.Center;
                    p.GetTextBounds(Header, 0, Header.Length, rect);
                    int height = rect.Height();
                    posY += height + 10;
                    canvas.DrawText(Header, (384 / 2), posY, p);
                }
            }

            using (Paint p = new Paint())
            {
                string Header = "ซีเนียร์ซอฟท์ ดีเวลลอปเม้นท์";

                using (Typeface tf = Typeface.Create("UPCDB", TypefaceStyle.Bold))
                {
                    Rect rect = new Rect();
                    p.SetTypeface(tf);
                    p.TextSize = 38;
                    p.Color = Android.Graphics.Color.Black;
                    p.TextAlign = Paint.Align.Center;
                    p.GetTextBounds(Header, 0, Header.Length, rect);
                    int height = rect.Height();
                    posY += height + 10;
                    canvas.DrawText(Header, (384 / 2), posY, p);
                }
            }


            using (Paint p = new Paint())
            {
                using (Typeface tf = Typeface.Create("Tahoma", TypefaceStyle.Normal))
                {
                    string invoice = "TAX#201910290001";
                    Rect rect = new Rect();
                    p.SetTypeface(tf);
                    p.TextSize = 24;
                    p.Color = Android.Graphics.Color.Black;
                    p.TextAlign = Paint.Align.Left;
                    p.GetTextBounds(invoice, 0, invoice.Length, rect);
                    int height = rect.Height();
                    posY += height + 16; // +6 เพิ่มความห่างของตัวอักษร
                    canvas.DrawText(invoice, 0, posY, p);
                }
            }

            using (Paint p = new Paint())
            {
                using (Typeface tf = Typeface.Create("Tahoma", TypefaceStyle.Normal))
                {
                    string time = "29/10/2562";
                    Rect rect = new Rect();
                    p.SetTypeface(tf);
                    p.TextSize = 24;
                    p.Color = Android.Graphics.Color.Black;
                    p.TextAlign = Paint.Align.Right;
                    p.GetTextBounds(time, 0, time.Length, rect);
                    int height = rect.Height();
                    int width = (int)p.MeasureText(time, 0, time.Length);
                    //posY += height + 3; // +3 เพิ่มความห่างของตัวอักษร
                    canvas.DrawText(time, 384, posY, p);
                }
            }

            using (Paint p = new Paint())
            {
                using (Typeface tf = Typeface.Create("Tahoma", TypefaceStyle.Normal))
                {
                    string voucher = "ใบเสร็จรับเงิน/ใบกำกับภาษีอย่างย่อ";
                    Rect rect = new Rect();
                    p.SetTypeface(tf);
                    p.TextSize = 22;
                    p.Color = Android.Graphics.Color.Black;
                    p.TextAlign = Paint.Align.Center;
                    p.GetTextBounds(voucher, 0, voucher.Length, rect);
                    int height = rect.Height();
                    posY += height + 26;
                    canvas.DrawText(voucher, (384 / 2), posY, p);
                }
            }

            List<string> itemName = new List<string>()
            {
                "1   จัมโบ้บิ๊กเปาหมูสับไข่เค็ม",
                "1   ดีน่างาดำ",
                "1   เวเฟอร์ล็อคเกอร์",
                "2   เวเฟอร์ล็อคเกอร์",
                "3   เวเฟอร์ล็อคเกอร์",
                "4   เวเฟอร์ล็อคเกอร์",
                "5   เวเฟอร์ล็อคเกอร์",
                "6   เวเฟอร์ล็อคเกอร์",
                "7   เวเฟอร์ล็อคเกอร์",
                "8   เวเฟอร์ล็อคเกอร์",
                "9   เวเฟอร์ล็อคเกอร์",
                "10   เวเฟอร์ล็อคเกอร์",
                "11   เวเฟอร์ล็อคเกอร์",
                "5   S-point",
                "    ยอดรวม",
                "ยอดสุทธิ   3 ชิ้น       50.00",
                "เงินสด/เงินทอน        100.00",
            };

            List<string> itemPrice = new List<string>()
            {
                "30.00",
                "10.00",
                "10.00",
                "20.00",
                "30.00",
                "40.00",
                "50.00",
                "60.00",
                "70.00",
                "80.00",
                "90.00",
                "100.00",
                "110.00",
                "0.00",
                "50.00",
                "",
                "50.00",
            };

            for (int i = 0; i < itemName.Count; i++)
            {
                using (Paint p = new Paint())
                {
                    using (Typeface tf = Typeface.Create("Tahoma", TypefaceStyle.Normal))
                    {
                        string name = itemName[i];
                        Rect rect = new Rect();
                        p.SetTypeface(tf);
                        p.TextSize = 22;
                        p.Color = Android.Graphics.Color.Black;
                        p.TextAlign = Paint.Align.Left;
                        p.GetTextBounds(name, 0, name.Length, rect);
                        int height = rect.Height();
                        posY += height + 16; // +6 เพิ่มความห่างของตัวอักษร
                        canvas.DrawText(name, 0, posY, p);
                    }
                }

                using (Paint p = new Paint())
                {
                    using (Typeface tf = Typeface.Create("Tahoma", TypefaceStyle.Normal))
                    {
                        string price = itemPrice[i];
                        Rect rect = new Rect();
                        p.SetTypeface(tf);
                        p.TextSize = 22;
                        p.Color = Android.Graphics.Color.Black;
                        p.TextAlign = Paint.Align.Right;
                        p.GetTextBounds(price, 0, price.Length, rect);
                        int height = rect.Height();
                        int width = (int)p.MeasureText(price, 0, price.Length);
                        //posY += height + 3; // +3 เพิ่มความห่างของตัวอักษร
                        canvas.DrawText(price, 384, posY, p);
                    }
                }
            }

            using (Paint p = new Paint())
            {
                using (Typeface tf = Typeface.Create("Tahoma", TypefaceStyle.Bold))
                {
                    string voucher = "-----------------------------------";
                    Rect rect = new Rect();
                    p.SetTypeface(tf);
                    p.TextSize = 30;
                    p.Color = Android.Graphics.Color.Black;
                    p.TextAlign = Paint.Align.Center;
                    p.GetTextBounds(voucher, 0, voucher.Length, rect);
                    int height = rect.Height();
                    posY += height + 30;
                    canvas.DrawText(voucher, (384 / 2), posY, p);
                }
            }
            var byteArr = convertBitmapToByteArrayUncompressed(bitmap);
            return byteArr;
        }

        private string ReplaceText(string text, string elementName, ParamSlip paramSlip, int index)
        {
            // index ใช้เฉพาะ Details ที่ข้อมูลเป็น List
            string replaceValue = "";
            int indexStart = text.IndexOf('@');
            if (indexStart == -1) return text;
            int indexEnd = text.IndexOf(' ', indexStart);
            if (indexEnd == -1) indexEnd = 0;
            indexEnd = indexEnd != 0 ? indexEnd : text.Length - indexEnd;
            string subString = text.Substring(indexStart, indexEnd - indexStart);
            strParam = subString;
            switch (elementName.ToLower())
            {
                case "header":
                    replaceValue = text.Replace(subString, paramSlip.Header.Find(x => x.Key == subString).Value);
                    break;
                case "details":
                    replaceValue = text.Replace(subString, paramSlip.Details[index].Find(x => x.Key == subString).Value);
                    break;
                case "footer":

                    break;
                default:
                    break;
            }

            return replaceValue;
        }
        private string WrapText(string text, int x, int width, Paint paint)
        {// ตัดข้อความให้เหลือเท่าขนาดความกว้างที่กำหนด

            if (String.IsNullOrEmpty(text)) return null;
            char[] textArr = text.ToArray();
            string newText = "";
            int sumWidth = 0;
            for (int i = 0; i < textArr.Length; i++)
            {
                int charWidth = (int)paint.MeasureText(textArr[i].ToString());
                if (sumWidth + charWidth <= width)
                {
                    newText += textArr[i];
                    sumWidth += charWidth;
                }
                else
                {
                    break;
                }

            }
            return newText;
        }
        private byte[] GenBitmapCode(Bitmap bm, bool doubleWidth, bool doubleHeight)
        {
            int MAX_BIT_WIDTH = 576;
            int w = bm.Width;
            int h = bm.Height;
            if (w > MAX_BIT_WIDTH)
                w = MAX_BIT_WIDTH;
            int bitw = ((w + 7) / 8) * 8;
            int bith = h;
            int pitch = bitw / 8;
            byte[] cmd = { 0x1D, 0x76, 0x30, 0x00, (byte)(pitch & 0xff), (byte)((pitch >> 8) & 0xff), (byte)(bith & 0xff), (byte)((bith >> 8) & 0xff) };
            byte[] bits = new byte[bith * pitch];

            // กว้าง2เท่า
            if (doubleWidth)
                cmd[3] |= 0x01;
            // สูง2เท่า
            if (doubleHeight)
                cmd[3] |= 0x02;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    int color = bm.GetPixel(x, y);

                    int R = (color >> 16) & 0xFF;
                    int G = (color >> 8) & 0xFF;
                    int B = (color) & 0xFF;


                    //int grayScale = (int)((R * .3) + (G * .59) + (B * .11));
                    //int grayScale = (int)((R * .33) + (G * .33) + (B * .33));
                    int grayScale = (R + G + B) / 3;

                    //if ((color & 0xFF) < 128)
                    if (grayScale < 128)
                    {
                        var a = 0x80 >> (x % 8);
                        //bits[y * pitch + x / 8] = (byte)a;
                        bits[y * pitch + x / 8] |= (byte)(0x80 >> (x % 8));
                    }

                }
            }
            MemoryStream memory = new MemoryStream(cmd.Length + bits.Length);
            memory.Write(cmd, 0, cmd.Length);
            memory.Write(bits, 0, (int)bits.Length);
            var arr = memory.ToArray();
            return arr;
        }

        public static byte[] convertBitmapToByteArrayUncompressed(Bitmap bitmap)
        {

            ByteBuffer buffer = ByteBuffer.Allocate(bitmap.ByteCount);
            bitmap.CopyPixelsToBuffer(buffer);
            buffer.Rewind();
            int MAX_BIT_WIDTH = 576;
            byte[] bytes = new byte[bitmap.ByteCount];
            int h = bitmap.Height;
            int w = bitmap.Width;
            if (w > MAX_BIT_WIDTH)
                w = MAX_BIT_WIDTH;
            int bitw = ((w + 7) / 8) * 8;
            int bith = h;
            int pitch = bitw / 8;
            byte[] cmd = { 0x1D, 0x76, 0x30, 0x00, (byte)(pitch & 0xff), (byte)((pitch >> 8) & 0xff), (byte)(bith & 0xff), (byte)((bith >> 8) & 0xff) };
            byte[] bits = new byte[bith * pitch];

            int _index = 0;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    //int index = (y * w) + x;
                    //int color = buffer.GetInt(index*4);
                    int color = buffer.GetInt(_index);
                    if (color != -1)
                    {
                        int R = (color >> 16) & 0xFF;
                        int G = (color >> 8) & 0xFF;
                        int B = (color) & 0xFF;
                        int grayScale = (R + G + B) / 3;

                        //if ((color & 0xFF) < 128)
                        if (grayScale < 128)
                        {
                            var a = 0x80 >> (x % 8);
                            bits[y * pitch + x / 8] |= (byte)(0x80 >> (x % 8));
                        }
                    }
                    _index += 4;
                }
            }
            MemoryStream memory = new MemoryStream(cmd.Length + bits.Length);
            memory.Write(cmd, 0, cmd.Length);
            memory.Write(bits, 0, (int)bits.Length);
            var arr = memory.ToArray();
            return arr;
        }

        public byte[] DrawImage(string xmlName, AssetManager assets, Bitmap bitmap)
        {// method อ่าน xml เพื่อสร้าง image และ gen เป็น bytes                        
            byte[] bytesData;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(assets.Open(xmlName));

                #region Test Code
                //var aa = "/storage/emulated/0/Android/data/com.seniorsoft.Gabana3/files/99900217/downloadbill/Receipt #4-000101.png";
                //PicturePath = aa;
                //Java.IO.File imgFile = new Java.IO.File(PicturePath);
                //Android.Graphics.Bitmap myBitmap = Android.Graphics.BitmapFactory.DecodeFile(imgFile.AbsolutePath);

                //using (MemoryStream memoryStream = new MemoryStream())
                //{
                //    myBitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 100, memoryStream);
                //    var imageItemByteArray = memoryStream.ToArray();
                //    bytesData = imageItemByteArray;
                //}
                //bytesData = convertBitmapToByteArrayUncompressed(bitmap);

                ////Create image 
                //var fullpathpng = filename + ".png";
                //string filePathpng = DataCashingAll.PathImageBill;
                ////string fullNamepng = Path.Combine(filePathpng, fullpathpng);
                //string fullNamepng = filePathpng + fullpathpng;
                //if (File.Exists(fullNamepng))
                //{
                //    File.Delete(fullNamepng);
                //}

                //FileNametestByte = fullNamepng;
                //var aa = "/storage/emulated/0/Android/data/com.seniorsoft.Gabana3/files/99900217/downloadbill/Receipt #4-000101.png";
                //PicturePath = aa;
                //Java.IO.File imgFile = new Java.IO.File(PicturePath);

                //Grayscale
                //int width = bitmap.Width;
                //int height = bitmap.Height;
                //int[] arr = new int[225];
                //int i = 0;
                //int p;
                //for (int y = 0; y < height; y++)
                //{
                //    for (int x = 0; x < width; x++)
                //    {
                //        p = bitmap.GetPixel(x, y);
                //        int a = Android.Graphics.Color.GetAlphaComponent(p);
                //        int r = Android.Graphics.Color.GetRedComponent(p);
                //        int g = Android.Graphics.Color.GetGreenComponent(p);
                //        int b = Android.Graphics.Color.GetBlueComponent(p);
                //        int avg = (r + g + b) / 3;
                //        avg = avg < 128 ? 0 : 255;     // Converting gray pixels to either pure black or pure white
                //        bitmap.SetPixel(x, y, Android.Graphics.Color.Argb(a, avg, avg, avg));
                //    }
                //}
                //bitmap =  resizeImage(bitmap,150,250); 
                #endregion

                int newWidth, newHeight;
                newWidth = (int)bitmap.Width / 2;
                newHeight = (int)bitmap.Height / 2;
                //newWidth = (int)bitmap.Width ;
                //newHeight = (int)bitmap.Height;
                bitmap = Android.Graphics.Bitmap.CreateScaledBitmap(bitmap, newWidth, newHeight, true);
                bytesData = convertBitmapToByteArrayUncompressed(bitmap);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return bytesData;
        }

        public byte[] DrawImagePrintFromXml(string xmlName, ParamSlip paramSlip, AssetManager assets, Bitmap bitmapMerchant)
        {// method อ่าน xml เพื่อสร้าง image และ gen เป็น bytes
            // Create bitmap empty
            Bitmap bitmap = null;
            Canvas canvas = null;
            byte[] bytesData;
            try
            {

                XmlDocument doc = new XmlDocument();
                doc.Load(assets.Open(xmlName));
                int slipWidht = doc.DocumentElement.Attributes["Width"] != null ? Convert.ToInt32(doc.DocumentElement.Attributes["Width"].Value) : 0;
                bitmap = Bitmap.CreateBitmap(slipWidht, 50000, Bitmap.Config.Argb8888); // create ให้ height ยาวไว้ก่อน                  
                canvas = new Canvas(bitmap);
                canvas.DrawColor(Android.Graphics.Color.White);
                XmlNodeList xmlNodeList = doc.DocumentElement.ChildNodes;
                int posY = 0;
                int lineSpace = doc.DocumentElement.Attributes["LineSpace"] != null ? Convert.ToInt32(doc.DocumentElement.Attributes["LineSpace"].Value) : 0;
                foreach (XmlNode nodeMain in xmlNodeList) // Header , Details , Footer
                {
                    int loopDraw = nodeMain.Name.ToLower() == "details" ? paramSlip.Details.Count : 1;
                    for (int i = 0; i < loopDraw; i++)
                    {
                        foreach (XmlNode nodeLine in nodeMain.SelectNodes("Line")) // Line
                        {
                            int lineHeight = 0;
                            int lineTextSize = nodeLine.Attributes["TextSize"] != null ? Convert.ToInt32(nodeLine.Attributes["TextSize"].Value) : 0;
                            TypefaceStyle lineTypefaceStyle = TypefaceStyle.Normal;
                            if (nodeLine.Attributes["Bold"] != null)
                            {
                                if (nodeLine.Attributes["Bold"].Value.ToLower() == "true")
                                {
                                    lineTypefaceStyle = TypefaceStyle.Bold;
                                }
                            }
                            foreach (XmlNode nodeText in nodeLine.SelectNodes("Text")) // Text
                            {
                                int textSize;
                                if (lineTextSize == 0)
                                {
                                    textSize = nodeText.Attributes["TextSize"] != null ? Convert.ToInt32(nodeText.Attributes["TextSize"].Value) : 0;
                                }
                                else
                                {
                                    textSize = lineTextSize;
                                }

                                TypefaceStyle typefaceStyle = TypefaceStyle.Normal;
                                if (lineTypefaceStyle != TypefaceStyle.Normal)
                                {
                                    typefaceStyle = lineTypefaceStyle;
                                }
                                else
                                {
                                    if (nodeText.Attributes["Bold"] != null)
                                    {
                                        if (nodeText.Attributes["Bold"].Value.ToLower() == "true")
                                        {
                                            typefaceStyle = TypefaceStyle.Bold;
                                        }
                                    }
                                }
                                int xText = nodeText.Attributes["X"] != null ? Convert.ToInt32(nodeText.Attributes["X"].Value) : 0;
                                int textWidth = nodeText.Attributes["Width"] != null ? Convert.ToInt32(nodeText.Attributes["Width"].Value) : 0;
                                int xAlign = 0;

                                Paint.Align align = Paint.Align.Left;
                                string alignValue = nodeText.Attributes["Align"] != null ? nodeText.Attributes["Align"].Value : "Left";
                                switch (alignValue.ToLower())
                                {
                                    case "center":
                                        align = Paint.Align.Center;
                                        xAlign = (textWidth / 2);
                                        break;
                                    case "left":
                                        align = Paint.Align.Left;
                                        xAlign = 0;
                                        break;
                                    case "right":
                                        align = Paint.Align.Right;
                                        xAlign = textWidth;
                                        break;
                                    default:
                                        break;
                                }
                                using (Paint p = new Paint())
                                {
                                    using (Typeface tf = Typeface.Create(Xamarin.Forms.Font.Default.FontFamily, typefaceStyle))
                                    {
                                        Android.Graphics.Rect rect = new Android.Graphics.Rect();
                                        p.SetTypeface(tf);
                                        if (textSize != 0)
                                            p.TextSize = textSize;
                                        p.Color = Android.Graphics.Color.Black;
                                        p.TextAlign = align;
                                        string value = WrapText(ReplaceText(nodeText.LastChild.Value, nodeMain.Name, paramSlip, i), xText, textWidth, p);
                                        if (String.IsNullOrEmpty(value)) value = " ";
                                        p.GetTextBounds(value, 0, value.Length, rect);
                                        lineHeight = lineHeight < rect.Height() ? rect.Height() : lineHeight;

                                        if ((strParam == "@ToppingName" || strParam == "@ToppingPrice" || strParam == "@Comment"))
                                        {
                                            if (!string.IsNullOrEmpty(value) && value != " ")
                                            {
                                                lineHeight = -6;
                                                p.Color = Android.Graphics.Color.Black;
                                            }
                                            else
                                            {
                                                //lineHeight = -5;
                                                break;
                                            }
                                        }

                                        int yPrint = posY + lineHeight;
                                        //canvas.DrawLine(xText, yPrint, xText + textWidth, yPrint, new Paint() { StrokeWidth = 3 });
                                        if (strParam == "@merchantName")
                                        {
                                            //var aaaaaa =  bitmap.Height;
                                            //bitmapMerchant = Bitmap.CreateBitmap(30, 30, Bitmap.Config.Argb8888);
                                            canvas.DrawBitmap(bitmapMerchant, xAlign + xText, yPrint, null);
                                            if (!string.IsNullOrEmpty(value) && value != " ")
                                            {
                                                lineHeight = 50;
                                                p.Color = Android.Graphics.Color.Black;
                                            }
                                            //canvas.DrawText(value, 10 ,30 , p);
                                            canvas.Save();
                                            //bitmap = bitmapMerchant.Copy(Bitmap.Config.Argb8888, true) ;
                                        }
                                        canvas.DrawText(value, xAlign + xText, yPrint, p);
                                    }
                                }
                            }

                            posY += lineHeight + lineSpace;
                        }
                    }
                }
                bitmap.Height = posY + 0;// สามารถเปลี่ยน height ให้น้อยลงได้ แต่มากกว่า ตอน createไม่ได้
                bytesData = convertBitmapToByteArrayUncompressed(bitmap);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (canvas != null)
                    canvas.Dispose();
                if (bitmap != null)
                    bitmap.Dispose();
            }
            return bytesData;

        }
    }
}