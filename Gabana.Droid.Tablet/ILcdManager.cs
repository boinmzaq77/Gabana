using Android.App;
using Android.Graphics;
using Java.IO;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Threading;
using static Android.Graphics.Bitmap;

namespace Gabana.Droid.Tablet
{
    public class ILcdManager
    {
        private static ILcdManager iLcdManager;
        private ILcdManager()
        {
            // Constructor logic here
        }
        public static ILcdManager GetInstance()
        {
            lock (typeof(ILcdManager))
            {
                if (iLcdManager == null)
                {
                    iLcdManager = new ILcdManager();
                }
            }
            return iLcdManager;
        }
        public void SendLCDCommand(int flag)
        {
            System.Console.WriteLine("sendLCDCommand flag " + flag);
            try
            {
                if (flag != 4)
                {
                    using (var writer = new StreamWriter("sys/spi_lcm/spi_lcm_power"))
                    {
                        writer.Write(flag);
                    }
                }
                else
                {
                    using (var writer = new StreamWriter("sys/spi_lcm/spi_lcm_power"))
                    {
                        writer.Write("5");
                    }
                }
            }
            catch (System.IO.IOException ex)
            {
                System.Console.WriteLine(ex.Message);
            }
        }

        public Android.Graphics.Bitmap GetTextAndroidBitmap(string txt)
        {
            // Concept ในการสร้างภาพเพื่อออก LCD
            // 1. สร้างภาพ แนวนอน (Lanscape) ขนาด width = 320, height = 240
            // 2. Rotate ภาพ ให้เป็นแนวตั้ง (Portrait) width = 240, height = 320
            // 3. Return ภาพ แนวตั้ง width = 240, height = 320

            try
            {
                // 1.สร้างภาพ แนวนอน ขนาด width = 320, height = 240
                Android.Graphics.Bitmap lanscapeBmp = Android.Graphics.Bitmap.CreateBitmap(320, 240, Android.Graphics.Bitmap.Config.Argb8888);
                Android.Graphics.Canvas canvas = new Android.Graphics.Canvas(lanscapeBmp);
                //canvas.DrawColor(Android.Graphics.Color.Blue);
                //canvas.DrawColor(Android.Graphics.Color.Green);
                canvas.DrawColor(Android.Graphics.Color.White);
                using (Paint paint = new Paint())
                {
                    //canvas.DrawRect(new Rect(1, 1, 318, 248), paint);
                    //canvas.DrawRect(new Rect(1, 1, 50, 10), paint);
                    paint.TextSize = 50;
                    paint.TextAlign = Paint.Align.Center;
                    paint.Color = new Android.Graphics.Color(124, 191, 167);
                    canvas.DrawText(txt, 160, 140, paint);
                }
                //MainActivity.imageView.SetImageBitmap(lanscapeBmp);

                // 2. Rotate ภาพ ให้เป็นแนวตั้ง width = 240, height = 320
                Android.Graphics.Matrix matrix = new Android.Graphics.Matrix();
                //matrix.PostRotate(-180.0F);
                matrix.PostRotate(-90.0F);
                Android.Graphics.Bitmap portraitBmp = Android.Graphics.Bitmap.CreateBitmap(lanscapeBmp, 0, 0, lanscapeBmp.Width, lanscapeBmp.Height, matrix, true);


                return portraitBmp;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                return null;
            }
        }
        public Android.Graphics.Bitmap GetQrAndroidBitmap(Android.Graphics.Bitmap qrimg)
        {
            // Concept ในการสร้างภาพเพื่อออก LCD
            // 1. สร้างภาพ แนวนอน (Lanscape) ขนาด width = 320, height = 240
            // 2. Rotate ภาพ ให้เป็นแนวตั้ง (Portrait) width = 240, height = 320
            // 3. Return ภาพ แนวตั้ง width = 240, height = 320

            try
            {
                // 1.สร้างภาพ แนวนอน ขนาด width = 320, height = 240
                Android.Graphics.Bitmap lanscapeBmp = Android.Graphics.Bitmap.CreateBitmap(320, 240, Android.Graphics.Bitmap.Config.Argb8888);
                Android.Graphics.Canvas canvas = new Android.Graphics.Canvas(lanscapeBmp);
                //canvas.DrawColor(Android.Graphics.Color.Blue);
                //canvas.DrawColor(Android.Graphics.Color.Green);
                canvas.DrawColor(Android.Graphics.Color.White);
                using (Paint paint = new Paint())
                {
                    //canvas.DrawRect(new Rect(1, 1, 318, 248), paint);
                    //canvas.DrawRect(new Rect(1, 1, 50, 10), paint);

                    canvas.DrawBitmap(qrimg, 45, 5, paint);
                }
                //MainActivity.imageView.SetImageBitmap(lanscapeBmp);

                // 2. Rotate ภาพ ให้เป็นแนวตั้ง width = 240, height = 320
                Android.Graphics.Matrix matrix = new Android.Graphics.Matrix();
                //matrix.PostRotate(-180.0F);
                matrix.PostRotate(-90.0F);
                Android.Graphics.Bitmap portraitBmp = Android.Graphics.Bitmap.CreateBitmap(lanscapeBmp, 0, 0, lanscapeBmp.Width, lanscapeBmp.Height, matrix, true);


                return portraitBmp;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                return null;
            }
        }
        public Android.Graphics.Bitmap GetTextPriceAndroidBitmap(decimal price)
        {
            // Concept ในการสร้างภาพเพื่อออก LCD
            // 1. สร้างภาพ แนวนอน (Lanscape) ขนาด width = 320, height = 240
            // 2. Rotate ภาพ ให้เป็นแนวตั้ง (Portrait) width = 240, height = 320
            // 3. Return ภาพ แนวตั้ง width = 240, height = 320

            try
            {
                // 1.สร้างภาพ แนวนอน ขนาด width = 320, height = 240
                Android.Graphics.Bitmap lanscapeBmp = Android.Graphics.Bitmap.CreateBitmap(320, 240, Android.Graphics.Bitmap.Config.Argb8888);
                Android.Graphics.Canvas canvas = new Android.Graphics.Canvas(lanscapeBmp);
                //canvas.DrawColor(Android.Graphics.Color.Blue);
                //canvas.DrawColor(Android.Graphics.Color.Green);
                canvas.DrawColor(Android.Graphics.Color.White);
                using (Paint paint = new Paint())
                {
                    //canvas.DrawRect(new Rect(1, 1, 318, 248), paint);
                    //canvas.DrawRect(new Rect(1, 1, 50, 10), paint);
                    paint.TextSize = 30;
                    paint.TextAlign = Paint.Align.Left;
                    paint.Color = new Android.Graphics.Color(0, 0, 0);
                    canvas.DrawText("ยอดชำระ :", 20, 100, paint);
                    //canvas.DrawColor(Android.Graphics.Color.Red);

                }
                using (Paint paint = new Paint())
                {
                    //paint.TextSize = 45;
                    //paint.TextAlign = Paint.Align.Right;
                    paint.Color = new Android.Graphics.Color(242, 249, 247);
                    var rect = new Rect();

                    canvas.DrawRect(new Rect(10, 120, 310, 190), paint);
                }
                using (Paint paint = new Paint())
                {
                    if (price < 10000000)
                    {
                        paint.TextSize = 45;
                    }
                    else if (price < 1000000000)
                    {
                        paint.TextSize = 40;
                    }
                    else if (price < 100000000000)
                    {
                        paint.TextSize = 35;
                    }
                    else
                    {
                        paint.TextSize = 30;
                    }

                    paint.TextAlign = Paint.Align.Right;
                    paint.Color = new Android.Graphics.Color(124, 191, 167);
                    canvas.DrawText(price.ToString("#,##0.00"), 300, 170, paint);
                }


                //MainActivity.imageView.SetImageBitmap(lanscapeBmp);

                // 2. Rotate ภาพ ให้เป็นแนวตั้ง width = 240, height = 320
                Android.Graphics.Matrix matrix = new Android.Graphics.Matrix();
                //matrix.PostRotate(-180.0F);
                matrix.PostRotate(-90.0F);
                Android.Graphics.Bitmap portraitBmp = Android.Graphics.Bitmap.CreateBitmap(lanscapeBmp, 0, 0, lanscapeBmp.Width, lanscapeBmp.Height, matrix, true);


                return portraitBmp;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                return null;
            }
        }

        public Android.Graphics.Bitmap resizeBitmap(string photoPath, int targetW, int targetH)
        {
            using (Android.Graphics.Bitmap selectedImage = BitmapFactory.DecodeFile(photoPath))
            {
                return Android.Graphics.Bitmap.CreateScaledBitmap(selectedImage, targetW, targetH, true);
            }
        }

        public Android.Graphics.Bitmap GetFileImageAndroidBitmap(string photoPath)
        {
            // Concept ในการสร้างภาพเพื่อออก LCD
            // 1. Load ภาพจาก FileImage & Scale ภาพ ให้เป็น แนวนอน (Lanscape) ขนาด width = 320, height = 240
            // 2. Rotate ภาพ ให้เป็นแนวตั้ง (Portrait) width = 240, height = 320
            // 3. Return ภาพ แนวตั้ง width = 240, height = 320

            try
            {
                // 1. Load ภาพจาก FileImage & Scale ภาพ ให้เป็น แนวนอน (Lanscape) ขนาด width = 320, height = 240
                Android.Graphics.Bitmap lanscapeBmp = resizeBitmap(photoPath, 320, 240);
                //MainActivity.imageView.SetImageBitmap(lanscapeBmp);

                // 2. Rotate ภาพ ให้เป็นแนวตั้ง width = 240, height = 320
                Android.Graphics.Matrix matrix = new Android.Graphics.Matrix();
                //matrix.PostRotate(-180.0F);
                matrix.PostRotate(-90.0F);
                Android.Graphics.Bitmap portraitBmp = Android.Graphics.Bitmap.CreateBitmap(lanscapeBmp, 0, 0, lanscapeBmp.Width, lanscapeBmp.Height, matrix, true);

                return portraitBmp;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                return null;
            }
        }



        public static byte[] convertBitmapToByteArray15Btis(Android.Graphics.Bitmap srcBitmap, int destWidth, int destHeight)
        {
            if (srcBitmap.Width != destWidth) throw new Exception("scrBitMap.Width not match destWidth");
            if (srcBitmap.Height != destHeight) throw new Exception("scrBitMap.Height not match destHeight");
            byte[] bmpHeader = { 0x42, 0x4d, 0x38, 0x58, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x36, 0x00, 0x00, 0x00, 0x28, 0x00,
                                 0x00, 0x00, 0xf0, 0x00, 0x00, 0x00, 0x40, 0x01, 0x00, 0x00, 0x01, 0x00, 0x10, 0x00, 0x00, 0x00,
                                 0x00, 0x00, 0xCA, 0x64, 0x00, 0x00, 0x12, 0x0b, 0x00, 0x00, 0x12, 0x0b, 0x00, 0x00, 0x00, 0x00,
                                 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            byte[] bmpTail = { 0x00, 0x00 };

            byte[] bmpPixel15Bits = new byte[destWidth * destHeight * 2];

            int[] bmpPixelArray = new int[destWidth * destHeight];
            srcBitmap.GetPixels(bmpPixelArray, 0, destWidth, 0, 0, destWidth, destHeight);

            for (int y = destHeight - 1, yy = 0; y >= 0; y--, yy++)
            {
                for (int x = 0; x < destWidth; x++)
                {
                    int color32Bits = bmpPixelArray[destWidth * yy + x];
                    int R = (color32Bits >> 19) & 0x1f;  // 8Bits to 5Bits    shift 3 + 8 + 8
                    int G = (color32Bits >> 11) & 0x1f;  // 8Bits to 5Bits    shift 3 + 8
                    int B = (color32Bits >> 3) & 0x1f;   // 8Bits to 5Bits    shift 3
                    int pixel15Bits = (R << 10) | (G << 5) | B;
                    int tagetIndex = (y * destWidth * 2) + (x * 2);
                    bmpPixel15Bits[tagetIndex] = (byte)(pixel15Bits & 0xff);
                    bmpPixel15Bits[tagetIndex + 1] = (byte)(pixel15Bits >> 8);
                }
            }

            MemoryStream memory = new MemoryStream(bmpHeader.Length + bmpPixel15Bits.Length + bmpTail.Length);
            memory.Write(bmpHeader, 0, bmpHeader.Length);
            memory.Write(bmpPixel15Bits, 0, bmpPixel15Bits.Length);
            memory.Write(bmpTail, 0, bmpTail.Length);

            byte[] byteArray = memory.ToArray();

            return byteArray;
        }





    }
}


