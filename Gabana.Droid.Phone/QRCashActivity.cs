using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
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
using System.Threading.Tasks;
using TinyInsightsLib;
using ZXing.Common;
using ZXing;
using Android.Content.PM;
using Android;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Gabana.Droid.Phone;
using Android.Graphics.Drawables;
using static Android.Webkit.WebStorage;
using static Java.Text.Normalizer;
using Android.Accounts;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class QRCashActivity : AppCompatActivity
    {
        internal static QRCashActivity mainacitivity;
#pragma warning disable CS0169 // The field 'QRCashActivity.bitmap' is never used
        Bitmap bitmap;
#pragma warning restore CS0169 // The field 'QRCashActivity.bitmap' is never used
        internal ImageView imvQRCash;
        Button btnSave;
        LinearLayout lnBack;
        public static TranWithDetailsLocal tranWithDetails;
        public static respone_QrKBank qrResult = new respone_QrKBank();
        

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mainacitivity = this;
            SetContentView(Resource.Layout.qrcash_activity_main);
            lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
            lnBack.Click += LnBack_Click;
            imvQRCash = FindViewById<ImageView>(Resource.Id.imvQRCash);                       
            btnSave = FindViewById<Button>(Resource.Id.btnSave);
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
                    Toast.MakeText(this, status_Qr.errorDesc, ToastLength.Short).Show();
                    btnSave.Enabled = true;
                    return;
                }

                string txnStatus = status_Qr.txnStatus;
                switch (txnStatus)
                {                    
                    case "EXPIRED":
                        MainDialog dialogEXPIRED = new MainDialog();
                        Bundle bundleEXPIRED = new Bundle();
                        String myMessageEXPIRED = Resource.Layout.qrcash_dialog_payment.ToString();
                        bundleEXPIRED.PutString("message", myMessageEXPIRED);
                        Qrcash_Dialog_Payment.SetDetail(tranWithDetails,  txnStatus);
                        dialogEXPIRED.Arguments = bundleEXPIRED;
                        dialogEXPIRED.Show(SupportFragmentManager, myMessageEXPIRED);
                        break;
                    case "CANCELLED":
                        MainDialog dialog = new MainDialog();
                        Bundle bundle = new Bundle();
                        String myMessage = Resource.Layout.qrcash_dialog_payment.ToString();
                        bundle.PutString("message", myMessage);
                        Qrcash_Dialog_Payment.SetDetail(tranWithDetails,  txnStatus);
                        dialog.Arguments = bundle;
                        dialog.Show(SupportFragmentManager, myMessage);
                        break;
                    case "PAID":
                        StartActivity(new Intent(Application.Context, typeof(myQRReceiptActivity)));
                        myQRReceiptActivity.SetTranDetail(tranWithDetails);
                        this.Finish();
                        break;
                    case "REQUESTED":
                    default:
                        Toast.MakeText(this, "กรุณาลองใหม่อีกครั้ง", ToastLength.Short).Show();
                        break;
                }
                btnSave.Enabled = true;
            }
            catch (Exception ex)
            {
                btnSave.Enabled = true;
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnSave_Click at QRCashActivity");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private async void LnBack_Click(object sender, EventArgs e)
        {
            try
            {
                lnBack.Enabled = false;
                //เช็คเพิ่มว่ายิง qr สำเร็จหรือยัง ถ้าสำเร็จ จบการขาย ไปที่ไหนต่อ ถามพี่ฝ้าย พี่ชะเอมอีกที
                //ถ้าไม่สำเร็จ void ทิ้งเหมือนเดิม
                Status_QrKBank status_Qr = new Status_QrKBank();
                status_Qr = await GetDataStatusQRPayment();
                if (status_Qr.statusCode == "-1" || status_Qr.statusCode == "10")
                {
                    Toast.MakeText(this, status_Qr.errorDesc, ToastLength.Short).Show();
                    lnBack.Enabled = true;
                    return;
                }

                //00
                string txnStatus = status_Qr.txnStatus;
                switch (txnStatus)
                {
                    case "REQUESTED":
                        MainDialog dialog = new MainDialog();
                        Bundle bundle = new Bundle();
                        String myMessage = Resource.Layout.qrcash_dialog.ToString();
                        bundle.PutString("message", myMessage);
                        Qrcash_Dialog.SetTranDetail(tranWithDetails);
                        dialog.Arguments = bundle;
                        dialog.Show(SupportFragmentManager, myMessage);
                        break;
                    case "EXPIRED":                        
                    case "CANCELLED":                      
                        StartActivity(new Intent(Application.Context, typeof(PaymentActivity)));
                        PaymentActivity.SetTranDetail(tranWithDetails);
                        this.Finish();
                        break;
                    case "PAID":                        
                        MainDialog dialogPAID = new MainDialog();
                        Bundle bundlePAID = new Bundle();
                        String myMessagePAID = Resource.Layout.qrcash_dialog_payment.ToString();
                        bundlePAID.PutString("message", myMessagePAID);
                        Qrcash_Dialog_Payment.SetDetail(tranWithDetails,  txnStatus);
                        dialogPAID.Arguments = bundlePAID;
                        dialogPAID.Show(SupportFragmentManager, myMessagePAID);
                        break;
                    default:
                        Toast.MakeText(this, "กรุณาลองใหม่อีกครั้ง", ToastLength.Short).Show();
                        break;
                }
                lnBack.Enabled = true;
            }
            catch (Exception ex)
            {
                lnBack.Enabled = true;
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("LnBack_Click at QRCashActivity");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        public override void OnBackPressed()
        {
            try
            {
                lnBack.PerformClick();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnBackPressed at QRCashActivity");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        public async void Setbitmap(ImageView imageView, string message)
        {

            Permission permissionRead = CheckSelfPermission(Manifest.Permission.ReadExternalStorage);
            Permission permissionWrite = CheckSelfPermission(Manifest.Permission.WriteExternalStorage);
            string[] PERMISSIONS =
                {
                    "android.permission.READ_EXTERNAL_STORAGE",
                    "android.permission.WRITE_EXTERNAL_STORAGE",
                    "android.permission.CAMERA"
                };
            if (permissionRead != Permission.Granted || permissionWrite != Permission.Granted)
            {
                RequestPermissions(PERMISSIONS, 1);
                return ;
            }
            try
            {
                if (permissionRead == Permission.Granted && permissionWrite == Permission.Granted)
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


                    var marginLeft = ((float)Math.Round(deltaWidth/2)) ;
                    var marginTop = ((float)Math.Round(deltaHeight / 2));


                    Canvas comboImage = new Canvas(bitmap);
                    comboImage.DrawBitmap(logo, marginLeft, marginTop, null);


                    //var sdpath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
                    string image_PATH = "";                    
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                    {
                        image_PATH = Android.App.Application.Context.GetExternalFilesDir("").AbsolutePath + "/" + DataCashingAll.MerchantId + "/picture/";
                    }
                    else
                    {
                        image_PATH = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DataDirectory.Path).ToString()
                             + "/" + DataCashingAll.MerchantId + "/picture/";
                    }

                    var path = System.IO.Path.Combine(image_PATH, "logeshbarcode.jpg");
                    var stream = new FileStream(path, FileMode.Create);
                    bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                    stream.Close();

                    imageView.SetImageBitmap(bitmap);
                    #endregion

                }
                else
                {
                    Console.WriteLine("No Permission");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception {ex} ");
                await TinyInsights.TrackErrorAsync(ex);
                return;
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                CheckJwt();

                //Function Creat Bitmap 
                Setbitmap(imvQRCash, qrResult.qrCode); 
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnResume at QRCashActivity");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        public static void SetTranDetail(TranWithDetailsLocal t)
        {
            tranWithDetails = t;
        }

        public static void SetResponeQRKBank(respone_QrKBank r)
        {
            qrResult = r;
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

        bool deviceAsleep = false;
        bool openPage = false;
        public DateTime pauseDate = DateTime.Now;
        async void CheckJwt()
        {
            try
            {
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                    this.Finish();
                    return;
                }

                Utils.AddNullValue();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CheckJwt at changePass");
            }
        }

        public override void OnUserInteraction()
        {
            base.OnUserInteraction();
            if (deviceAsleep)
            {
                deviceAsleep = false;
                TimeSpan span = DateTime.Now.Subtract(pauseDate);

                long DISCONNECT_TIMEOUT = 5 * 60 * 1000; // 1 min = 1 * 60 * 1000 ms
                if ((span.Minutes * 60 * 1000) >= DISCONNECT_TIMEOUT)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(SplashActivity)));
                    this.Finish();
                    return;
                }
                else
                {
                    pauseDate = DateTime.Now;
                }
            }
            else
            {
                pauseDate = DateTime.Now;

            }
            if (DataCashingAll.UsePinCode && !UtilsAll.CheckPincode())
            {
                StartActivity(new Android.Content.Intent(Application.Context, typeof(PinCodeActitvity)));
                PinCodeActitvity.SetPincode("Pincode");
                openPage = true;
            }

        }
    }
}