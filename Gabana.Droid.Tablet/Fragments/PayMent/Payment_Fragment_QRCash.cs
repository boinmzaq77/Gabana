using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ShareSource;
using Gabana3.JAM.Trans;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using ZXing.Common;
using ZXing;
using Android.Content.PM;
using Android;
using System.Threading.Tasks;
using Gabana.Droid.Tablet.Dialog;
using ZXing.QrCode.Internal;

namespace Gabana.Droid.Tablet.Fragments.PayMent
{
    public class Payment_Fragment_QRCash : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Payment_Fragment_QRCash NewInstance()
        {
            Payment_Fragment_QRCash frag = new Payment_Fragment_QRCash();
            return frag;
        }
        View view;
        Payment_Fragment_QRCash payment_fragment_qrcash;
        public static TranWithDetailsLocal tranWithDetails;
        public static respone_QrKBank qrResult = new respone_QrKBank();

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.payment_fragment_qrcash, container, false);
            try
            {
                payment_fragment_qrcash = this;
                tranWithDetails = MainActivity.tranWithDetails;
                ComBineUI();
                SetEvenUI();
                CheckJwt();                
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
            }
            return view;
        }

        public override void OnResume()
        {
            try
            {
                base.OnResume();

                if (!IsAdded)
                {
                    return;
                }

                //if (!IsVisible)
                //{
                //    return;
                //}

                CheckJwt(); 
                tranWithDetails = MainActivity.tranWithDetails;
                if (!string.IsNullOrEmpty(qrResult?.qrCode))
                {
                    Setbitmap(imvQRCash, qrResult.qrCode);
                    Utils.Setbitmap(bitmapqrcodeimvQRCash, qrResult.qrCode);
                    bitmapqrcodeimvQRCash.Visibility = ViewStates.Gone; //ไม่แสดงที่แอป
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnResume at Payment_Fragment_QrCash");
            }
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            Utils.ShowHi("Welcome");
        }

        public static void SetResponeQRKBank(respone_QrKBank r)
        {
            qrResult = r;
        }

        private  async void CheckJwt()
        {
            try
            {
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                    MainActivity.main_activity.Finish();
                    return;
                }

                Utils.AddNullValue();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CheckJwt at Payment_Fragment_QrCash");
            }
        }
        public void showqrclick()
        {
            showqr(qrResult.qrCode);
        }
        public void showqr(string qrcode)
        {
            try
            {
                if (string.IsNullOrEmpty(qrcode)) return;
                //test qrstring to bitmap
                Utils.Getqrcodetodisplay2(qrcode);
            }
            catch (Exception ex )
            {

            }
            
                
        }
        private void SetEvenUI()
        {
            lnBack.Click += LnBack_Click;
            btnSave.Click += BtnSave_Click;
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                btnSave.Enabled = false;
                Status_QrKBank status_Qr = new Status_QrKBank();
                status_Qr = await GetDataStatusQRPayment();
                if (status_Qr.statusCode == "-1" || status_Qr.statusCode == "10")
                {
                    Toast.MakeText(this.Activity, status_Qr.errorDesc, ToastLength.Short).Show();
                    btnSave.Enabled = true;
                    return;
                }

                string txnStatus = status_Qr.txnStatus;
                switch (txnStatus)
                {
                    //payment_dialog_qrcash
                    case "EXPIRED":
                    case "CANCELLED":                        
                        var fragment = new Payment_Dialog_QrCash() { Cancelable = false };
                        fragment.Show(Activity.SupportFragmentManager, nameof(Payment_Dialog_QrCash));
                        Payment_Dialog_QrCash.SetDetail(txnStatus);
                        break;  
                    case "PAID":
                        PaymentActivity.payment_main.LoadFragment(Resource.Id.lnCash, "payment", "myqrreceipt");
                        break;
                    case "REQUESTED":
                    default:
                        Toast.MakeText(this.Activity, "กรุณาลองใหม่อีกครั้ง", ToastLength.Short).Show();
                        break;
                }
                btnSave.Enabled = true;
            }
            catch (Exception ex)
            {
                btnSave.Enabled = true;
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnSave_Click at QRCashActivity");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private async void LnBack_Click(object sender, EventArgs e)
        {
            try
            {
                lnBack.Enabled = false;
                Status_QrKBank status_Qr = new Status_QrKBank();
                status_Qr = await GetDataStatusQRPayment();
                if (status_Qr.statusCode == "-1" || status_Qr.statusCode == "10")
                {
                    Toast.MakeText(this.Activity, status_Qr.errorDesc, ToastLength.Short).Show();
                    lnBack.Enabled = true;
                    return;
                }

                //00
                string txnStatus = status_Qr.txnStatus;
                switch (txnStatus)
                {
                    case "REQUESTED":
                        var fragment = new Payment_Dialog_Request() { Cancelable = false };
                        fragment.Show(Activity.SupportFragmentManager, nameof(Payment_Dialog_Request));
                        break;
                    case "PAID":
                        var fragmentQrCash = new Payment_Dialog_QrCash() { Cancelable = false };
                        fragmentQrCash.Show(Activity.SupportFragmentManager, nameof(Payment_Dialog_QrCash));
                        Payment_Dialog_QrCash.SetDetail(txnStatus);
                        break;
                    case "EXPIRED":
                    case "CANCELLED":
                        PaymentActivity.payment_main.LoadFragment(Resource.Id.lnCash, "payment", "default");
                        break;
                    default:
                        Toast.MakeText(this.Activity, "กรุณาลองใหม่อีกครั้ง", ToastLength.Short).Show();
                        break;
                }
                lnBack.Enabled = true;
            }
            catch (Exception ex)
            {
                lnBack.Enabled = true;
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("LnBack_Click at QRCashActivity");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        internal ImageView imvQRCash, bitmapqrcodeimvQRCash;
        Button btnSave;
        LinearLayout lnBack;
        private void ComBineUI()
        {
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            imvQRCash = view.FindViewById<ImageView>(Resource.Id.imvQRCash);
            bitmapqrcodeimvQRCash = view.FindViewById<ImageView>(Resource.Id.bitmapqrcodeimvQRCash);
            btnSave = view.FindViewById<Button>(Resource.Id.btnSave);
        }

        public  void Setbitmap(ImageView imageView, string message)
        {
            try
            {
                int size = 660;
                #region Complete Code                   
                BitMatrix bitmapMatrix = new MultiFormatWriter().encode(message, BarcodeFormat.QR_CODE, size, size);

                var width = bitmapMatrix.Width;
                var height = bitmapMatrix.Height;
                int[] pixelsImage = new int[width * height];

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (bitmapMatrix[j, i])
                            pixelsImage[i * width + j] = (int)Convert.ToInt64(0xff000000);
                        else
                            pixelsImage[i * width + j] = (int)Convert.ToInt64(0xffffffff);
                    }
                }

                Bitmap bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
                bitmap.SetPixels(pixelsImage, 0, width, 0, 0, width, height);

                Bitmap logo = BitmapFactory.DecodeResource(Resources, Resource.Mipmap.Thai_QR_Payment_Mini);

                float deltaHeight = bitmap.Height - logo.Height;
                float deltaWidth = bitmap.Width - logo.Height;


                var marginLeft = ((float)Math.Round(deltaWidth / 2));
                var marginTop = ((float)Math.Round(deltaHeight / 2));


                Canvas comboImage = new Canvas(bitmap);
                comboImage.DrawBitmap(logo, marginLeft, marginTop, null);


                //var sdpath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
                string image_PATH = "";
                image_PATH = DataCashingAll.PathImageBill;
                var path = System.IO.Path.Combine(image_PATH, "qrcashpayment.jpg");
                var stream = new FileStream(path, FileMode.Create);
                bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                stream.Close();

                imageView.SetImageBitmap(bitmap);
                #endregion
            }
            catch (Exception ex)
            {
                _= TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Setbitmap at Payment_Fragment_QRCash");
                return;
            }
        }

        public static async Task<Status_QrKBank> GetDataStatusQRPayment()
        {
            try
            {
                respone_QrKBank qrStatus = new respone_QrKBank();
                qrStatus = await GabanaAPI.GetDataStatusQRPayment(DataCashing.countGen.Tranno);
                Status_QrKBank status_Qr = new Status_QrKBank();
                status_Qr.statusCode = qrStatus.statusCode;
                status_Qr.txnStatus = qrStatus.txnStatus;
                status_Qr.errorDesc = qrStatus.errorDesc;
                return status_Qr;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GetDataStatusQRPayment at QRCashActivity");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                Status_QrKBank status_Qr = new Status_QrKBank();
                return status_Qr;
            }
        }

    }
}