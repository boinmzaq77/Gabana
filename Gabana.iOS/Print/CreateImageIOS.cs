using CoreGraphics;
using Foundation;
using Gabana.Model;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;

namespace Gabana.iOS
{
    static class CreateImageIOS
    {
        public static byte[] DrawImage(TranWithDetailsLocal tran, List<string> address)
        { 
            DumpView dumpView = new DumpView();
            CGRect rect;
            float height = 500;
            var countdetail = tran.tranDetailItemWithToppings.Count();
            
            //height = height + (countdetail * 36);
            if (DataCashingAll.setting.TYPEPAGE == "58mm")
            {
                rect = new CGRect(0, 0, 380, height);
            }
            else 
            {
                rect = new CGRect(0, 0, 560, height);
            }
            //CGRect rect = new CGRect(0, 0, 384, 350);
            dumpView.Draw2(rect, tran, address);
            
            UIImage uIImage = new UIImage();
            dumpView.Layer.RenderInContext(UIGraphics.GetCurrentContext());
            var img = UIGraphics.GetImageFromCurrentImageContext();
            //img.Size.Height = img.Size.Height+200;
            //img. = new CGSize() {Height = height , Width = img.Size.Width}; 
            UIGraphics.EndImageContext();
            NSData imageData = img.AsJPEG();
            //NSData imageDatas = NSData.FromStream(new MemoryStream(imageData.ToArray()));
            UIImage image = UIImage.LoadFromData(imageData, (nfloat)0.7);
            var ee =  image.AsJPEG().ToArray();
            byte[] byteData = GenBitmapCode(image, false, false);
            return byteData;
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static UIImage DrawImageSave(TranWithDetailsLocal tran)
        {
            DumpView dumpView = new DumpView();
            CGRect rect;
            float height = 500;
            var countdetail = tran.tranDetailItemWithToppings.Count();
            height = height + (countdetail * 36);
            if (DataCashingAll.setting.TYPEPAGE == "58mm")
            {
                rect = new CGRect(0, 0, 400, height);
            }
            else
            {
                rect = new CGRect(0, 0, 580, height);
            }
            //CGRect rect = new CGRect(0, 0, 384, 350);
            //dumpView.Draw2(rect, tran);

            UIImage uIImage = new UIImage();
            dumpView.Layer.RenderInContext(UIGraphics.GetCurrentContext());
            var img = UIGraphics.GetImageFromCurrentImageContext();
            //img.Size.Height = img.Size.Height+200;
            //img. = new CGSize() {Height = height , Width = img.Size.Width}; 
            UIGraphics.EndImageContext();
            NSData imageData = img.AsJPEG();
            //NSData imageDatas = NSData.FromStream(new MemoryStream(imageData.ToArray()));
            UIImage image  = UIImage.LoadFromData(imageData);
            
            //UIImageWriteToSavedPhotosAlbum(imageToSave!, nil, nil, nil)
            //byte[] byteData = GenBitmapCode(image, false, false);
            return image;
        }

        public static async Task< List<string>> DrawString(TranWithDetailsLocal tran)
        {
            DumpView dumpView = new DumpView();
            int size;
            if (DataCashingAll.setting.TYPEPAGE == "58mm")
            {
                size = 1;

            }
            else
            {
                size = 2;
            }
            var list = await dumpView.Draw3(size, tran);
            return list;
        }

        public static byte[] GenBitmapCode(UIImage image, bool doubleWidth, bool doubleHeight)
        {
            NSMutableArray<UIColor> result = new NSMutableArray<UIColor>();
            // First get the image into your data buffer
            CGImage imageRef = image.CGImage;

            nint width = imageRef.Width;
            nint height = imageRef.Height;

            byte[] rawData = new byte[height * width * 4];
            nint bytesPerPixel = 4;
            nint bytesPerRow = bytesPerPixel * width;

            CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB();
            nint bitsPerComponent = 8;
            CGContext context = new CGBitmapContext(rawData, width, height, bitsPerComponent, bytesPerRow, colorSpace, CGBitmapFlags.ByteOrder32Big | CGBitmapFlags.PremultipliedLast);
            colorSpace.Dispose();
            context.DrawImage(new CGRect(0, 0, width, height), imageRef);
            context.Dispose();

            int MAX_BIT_WIDTH = 576;
            int w = (int)width;
            int h = (int)height;
            if (w > MAX_BIT_WIDTH)
                w = MAX_BIT_WIDTH;
            int bitw = ((w + 7) / 8) * 8;   // คำนวณปัดเต็ม
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
            // Now your rawData contains the image data in the RGBA8888 pixel format.

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    nint byteIndex = (bytesPerRow * y) + x * bytesPerPixel;
                    float alpha = rawData[byteIndex + 3];
                    float red = rawData[byteIndex];
                    float green = rawData[byteIndex + 1];
                    float blue = rawData[byteIndex + 2];
                    byteIndex += bytesPerPixel;
                    var color = Color.FromRgb(((int)red >> 16) & 0xff, ((int)green >> 8) & 0xff, ((int)blue >> 0) & 0xff);

                    if (red < 128 || green < 128 || blue < 128) // ตรวจหาสีไปทางดำ ให้จุดที่ pixel นั้น
                    {
                        bits[y * pitch + x / 8] |= (byte)(0x80 >> (x % 8));
                    }
                    //UIColor acolor = new UIColor(red, green, blue, alpha);
                    //result.Add(acolor);
                }
            }
            MemoryStream memory = new MemoryStream(cmd.Length + bits.Length);
            memory.Write(cmd, 0, cmd.Length);
            memory.Write(bits, 0, (int)bits.Length);
            var arr = memory.ToArray();
            return arr;
        }

        public static byte[] DrawImageTestPrint(TranWithDetailsLocal tran, List<string> address)
        {
            DumpView dumpView = new DumpView();
            CGRect rect;
            float height = 500;
            var countdetail = tran.tranDetailItemWithToppings.Count();

            if (DataCashingAll.setting.TYPEPAGE == "58mm")
            {
                rect = new CGRect(0, 0, 380, height);
            }
            else
            {
                rect = new CGRect(0, 0, 560, height);
            }
            dumpView.Draw2TestPrint(rect, tran, address);

            UIImage uIImage = new UIImage();
            dumpView.Layer.RenderInContext(UIGraphics.GetCurrentContext());
            var img = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            NSData imageData = img.AsJPEG();
            UIImage image = UIImage.LoadFromData(imageData, (nfloat)0.7);
            var ee = image.AsJPEG().ToArray();
            byte[] byteData = GenBitmapCode(image, false, false);
            return byteData;
        }

        public static async Task<List<string>> DrawStringTestPrint(TranWithDetailsLocal tran)
        {
            DumpView dumpView = new DumpView();
            int size;
            if (DataCashingAll.setting.TYPEPAGE == "58mm")
            {
                size = 1;

            }
            else
            {
                size = 2;
            }
            var list = await dumpView.Draw3TestPrint(size, tran);
            return list;
        }


    }
}