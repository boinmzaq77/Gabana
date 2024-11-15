using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Android.Support.V7.App;
using Android.Widget;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using TinyInsightsLib;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class myQRReceiptActivity : AppCompatActivity
    {
        public static myQRReceiptActivity myQRReceipt;
        LinearLayout lnBack, lnAddimage;
        public static LinearLayout lnNoCustomer, lnHaveCustomer;
        TransManage transManage = new TransManage();
        public static TranWithDetailsLocal tranWithDetails;
        public static TranPayment tranPayment = new TranPayment();
        int PaymentNo;
        string CURRENCYSYMBOLS, path, pathFolderPicture;
        EditText txtReceiptName, txtComment;
        Android.Net.Uri cameraTakePictureUri;
        int RESULT_OK = -1;
        int CAMERA_REQUEST = 0;
        int CROP_REQUEST = 1;
        int GALLERY_PICTURE = 2;
        Android.Net.Uri keepCropedUri = null;
        Android.Graphics.Bitmap bitmap;
        TextView textNamePic;
        ImageView imageViewShowReciept;
        Button btnSave;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                SetContentView(Resource.Layout.myqrreceipt_activity);
                myQRReceipt = this;

                lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnAddimage = FindViewById<LinearLayout>(Resource.Id.lnAddimage);
                txtReceiptName = FindViewById<EditText>(Resource.Id.txtReceiptName);
                txtComment = FindViewById<EditText>(Resource.Id.txtComment);
                textNamePic = FindViewById<TextView>(Resource.Id.textNamePic);
                imageViewShowReciept = FindViewById<ImageView>(Resource.Id.imageViewShowReciept);

                btnSave = FindViewById<Button>(Resource.Id.btnSave);
                CheckJwt();
                CheckPermission();

                lnBack.Click += LnBack_Click;
                lnAddimage.Click += LnAddimage_Click;
                btnSave.Click += ButtonSave_Click;

                CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
                pathFolderPicture = DataCashingAll.PathFolderImage;

                txtReceiptName.Text = "Receipt-" + DateTime.Now.ToString("HH:mm");
                txtComment.Text = "จ่ายแล้ว";

                _ = TinyInsights.TrackPageViewAsync("OnCreate : myQRReceiptActivity");


            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void LnAddimage_Click(object sender, EventArgs e)
        {
            try
            {
                MainDialog dialog = new MainDialog();
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.addcustomer_dialog_addimage.ToString();
                bundle.PutString("message", myMessage);
                bundle.PutString("OpenPicture", "receipt");
                dialog.Arguments = bundle;
                dialog.Show(SupportFragmentManager, myMessage);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("LnAddimage_Click at add myQRReceipt");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void ButtonSave_Click(object sender, EventArgs e)
        {
            try
            {
                btnSave.Enabled = false;
                initialData();
                int PaymentNo = tranWithDetails.tranPayments.Count();
                PaymentNo++;

                decimal paymentAmount = 0;
                foreach (var item in tranWithDetails.tranPayments)
                {
                    paymentAmount += item.PaymentAmount;
                }

                decimal amount = tranWithDetails.tran.GrandTotal - paymentAmount;

                tranPayment.PaymentNo = PaymentNo;
                tranPayment.PaymentAmount = amount; //เงินที่จ่าย
                tranPayment.Comments = txtReceiptName.Text;

                if (keepCropedUri != null)
                {
                    path = Utils.SplitPath(keepCropedUri.ToString());
                    var checkResultPicture = await Utils.InsertImageToPicture(path, bitmap);
                    tranPayment.PicturePath = pathFolderPicture + path;
                }
                else
                {
                    tranPayment.PicturePath = null;
                }

                tranWithDetails.tranPayments.Add(tranPayment);

                StartActivity(new Intent(Application.Context, typeof(BalanceActivity)));
                BalanceActivity.SetTranDetail(tranWithDetails);
                btnSave.Enabled = true;                
                this.Finish();
            }
            catch (Exception ex)
            {
                btnSave.Enabled = true;
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Long).Show();
                return;
            }

        }

        private void BtnCustomer_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(SelectCustomerActivity)));
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(myQRCashActivity)));
            PaymentActivity.SetTranDetail(tranWithDetails);
            this.Finish();
        }
        public override void OnBackPressed()
        {
            lnBack.PerformClick();
        }

        public static void SetTranDetail(TranWithDetailsLocal t)
        {
            tranWithDetails = t;
        }

        private void initialData()
        {
            if (tranWithDetails == null)
            {
                return;
            }

            tranPayment = new TranPayment()
            {
                MerchantID = DataCashingAll.MerchantId,
                SysBranchID = DataCashingAll.SysBranchId,
                TranNo = tranWithDetails.tran.TranNo,
                PaymentNo = PaymentNo,
                PaymentType = DataCashing.PaymentType,
                PaymentAmount = (decimal)0, //เงินที่ต้องจ่าย
                CreditCardType = null,
                CardNo = null,
                ExprieDateYYYYMM = null,
                ApproveCode = null,
                TotalRedeemPoint = null,
                //EPaymentType = "QR",
                RequestNum = null,
                RequestDateTime = null,
                FEPaymentCancel = 0,
                ReferenceNo1 = null,
                ReferenceNo2 = null,
                ReferenceNo3 = null,
                ReferenceNo4 = null,
                Comments = null,
            };
        }
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        protected async override void OnResume()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            base.OnResume();
            CheckJwt();
            CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
        }

        //-------------------------------------------------------------------------
        //Picture
        //--------------------------------------------------------------------------
        #region Picture

        public async void GalleryOpen()
        {
            try
            {
                string action;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    action = Intent.ActionOpenDocument;
                }
                else
                {
                    action = Intent.ActionPick;
                }
                Intent GalIntent = new Intent(action, MediaStore.Images.Media.ExternalContentUri);
                StartActivityForResult(Intent.CreateChooser(GalIntent, "Select image from galery"), GALLERY_PICTURE);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GalleryOpen at add Qr");
                Toast.MakeText(Application.Context, "error : " + ex.Message, ToastLength.Short).Show(); return;
            }
        }

        //Android.Net.Uri keepCropedUri;    // เก็บเอาไว้ใช้งานที่ OnActionResult  ของการ Crop เพราะ Androd ที่ตำกว่า Android.N จะไม่มีชื่อไฟล์กลับไป
        public void CropImage(Android.Net.Uri imageUri)
        {
            try
            {
                Intent CropIntent = new Intent("com.android.camera.action.CROP");
                CropIntent.SetDataAndType(imageUri, "image/*");
                CropIntent.AddFlags(ActivityFlags.GrantReadUriPermission); // **** ต้อง อนุญาติให้อ่าน ได้ด้วยนะครับ สำคัญ มันจะสามารถอ่านไฟล์ที่ได้จากการ CaptureImage ได้ ****

                CropIntent.PutExtra("crop", "true");
                CropIntent.PutExtra("outputX", 1260);
                CropIntent.PutExtra("outputY", 1680);
                CropIntent.PutExtra("aspectX", 3);
                CropIntent.PutExtra("aspectY", 4);
                CropIntent.PutExtra("scaleUpIfNeeded", true);
                // do not use return data for big images
                CropIntent.PutExtra("return-data", false);

 
#pragma warning disable CS0618 // Type or member is obsolete
                string cropedFilePath = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath,
                                                         Android.OS.Environment.DirectoryPictures,
                                                         "file_" + Guid.NewGuid().ToString() + ".jpg");
#pragma warning restore CS0618 // Type or member is obsolete
 
                Java.IO.File cropedFile = new Java.IO.File(cropedFilePath);
                // create new file handle to get full resolution crop
                Android.Net.Uri cropedUri;

                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    cropedUri = Android.Support.V4.Content.FileProvider.GetUriForFile(Application.Context, Application.Context.ApplicationContext.PackageName + ".fileProvider", cropedFile);

                    //this is the stuff that was missing - but only if you get the uri from FileProvider
                    CropIntent.AddFlags(ActivityFlags.GrantWriteUriPermission);

                    //กำหนดสิทธิให้ Inten อื่นสามารถ เขียน Uri ได้
                    List<ResolveInfo> resolvedIntentActivities = Application.Context.PackageManager.QueryIntentActivities(CropIntent, PackageInfoFlags.MatchDefaultOnly).ToList();
                    foreach (ResolveInfo resolvedIntentInfo in resolvedIntentActivities)
                    {
                        String packageName = resolvedIntentInfo.ActivityInfo.PackageName;
                        Application.Context.GrantUriPermission(packageName, cropedUri, ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission);
                    }
                }
                else
                {
                    cropedUri = Android.Net.Uri.FromFile(cropedFile);
                }
                keepCropedUri = cropedUri;  // เก็บเอาไว้ใช้งานที่ OnActionResult เพราะ Android ที่ต่ำกว่า Android.N จะ Get เอาจาก ค่าที่ส่งไปใน Functio ไม่ได้

                CropIntent.PutExtra(MediaStore.ExtraOutput, cropedUri);
                StartActivityForResult(CropIntent, CROP_REQUEST);
            }
            catch (ActivityNotFoundException ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CropImage at add qr");
                Toast.MakeText(Application.Context, "error : CropImage : " + ex.Message, ToastLength.Short).Show(); return;
            }
        }

        public void CameraTakePicture()
        {
            try
            {
                Intent CamIntent = new Intent(MediaStore.ActionImageCapture);
                CamIntent.AddFlags(ActivityFlags.GrantWriteUriPermission);

                //ถ้ากำหนดชื่อชื่อไฟล์ มันจะ Save ลงที่ไฟล์นี้แล้วส่งไปให้ OnActivityResult
 
#pragma warning disable CS0618 // Type or member is obsolete
                string filePath = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath,
                                                         Android.OS.Environment.DirectoryPictures,
                                                         "file_" + Guid.NewGuid().ToString() + ".jpg");
#pragma warning restore CS0618 // Type or member is obsolete
 
                Java.IO.File tempFile = new Java.IO.File(filePath);
                Android.Net.Uri tempURI;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    tempURI = Android.Support.V4.Content.FileProvider.GetUriForFile(Application.Context, Application.Context.ApplicationContext.PackageName + ".fileProvider", tempFile);

                    //this is the stuff that was missing - but only if you get the uri from FileProvider
                    CamIntent.AddFlags(ActivityFlags.GrantWriteUriPermission);
                }
                else
                {
                    tempURI = Android.Net.Uri.FromFile(tempFile);
                }
                cameraTakePictureUri = tempURI;
                CamIntent.PutExtra(MediaStore.ExtraOutput, tempURI);
                CamIntent.PutExtra("return-data", false);
                CamIntent.AddFlags(ActivityFlags.GrantWriteUriPermission);

                StartActivityForResult(CamIntent, CAMERA_REQUEST);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("take Pic at add qr");
                Toast.MakeText(Application.Context, "error : " + ex.Message, ToastLength.Short).Show(); return;
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                if (requestCode == CAMERA_REQUEST && Convert.ToInt32(resultCode) == RESULT_OK)
                {
                    // Solution 1 : เอาชื่อไฟล์ที่ได้ส่งไป crop
                    CropImage(cameraTakePictureUri);

                    // Solution 2 : เอา Data ที่เป็น Bitmap Save ลง Temp โรสำ แล้ว ชื่อไฟล์ที่ได้ส่งไป crop
                    //            : แบบนี้ ภาพไม่ชัด
                    //Bundle bundle = data.Extras;
                    //Bitmap bitmap = (Bitmap)bundle.GetParcelable("data");
                }
                else if (requestCode == GALLERY_PICTURE && Convert.ToInt32(resultCode) == RESULT_OK)
                {
                    // มาจาก User เลื่อกรุปจาก Gallory : (case นี้จะมี uri)
                    if (data != null)
                    {
                        Android.Net.Uri selectPictureUri = data.Data;
                        CropImage(selectPictureUri);
                    }
                    else
                    {
                        Toast.MakeText(Application.Context, "error : GALLERY_PICTURE data is null", ToastLength.Short).Show();
                        return;
                    }
                }
                else if (requestCode == CROP_REQUEST && Convert.ToInt32(resultCode) == RESULT_OK)
                {
                    // มาจาก User เลื่อกถ่ายรูป หรือ เลื่อกรุปจาก Gallory แล้วผ่าน function CropImage();
                    if (data != null)
                    {

                        Bundle bundle = data.Extras;

                        // Solution 1 : เอาค่า BitMap มาจัดการเลย (ok) แต่ใช้กับ Android 10 ไม่ได้ครับ
                        //Bitmap bitmap = (Bitmap)bundle.GetParcelable("data");

                        // Solution 2 : อ่าน BitMap จากไฟล์ (ok)
                        Android.Net.Uri cropImageURI = keepCropedUri;
#pragma warning disable CS0618 // Type or member is obsolete
                        bitmap = Android.Provider.MediaStore.Images.Media.GetBitmap(Application.Context.ContentResolver, cropImageURI);
#pragma warning restore CS0618 // Type or member is obsolete

                        // Solution 3 : อ่าน BitMap จากไฟล์ โดยเอาค่าไฟล์จาก bundle.GetParcelable(MediaStore.ExtraOutput) : จะ error กับ Andord ที่ต่ำกว่า Android.N
                        //Android.Net.Uri cropImageURI = (Android.Net.Uri)bundle.GetParcelable(MediaStore.ExtraOutput); // ใช้กับ Andord ที่ต่ำกว่า Android.N ไม่ได้ เพราะจะมีค่าเป็น Null
                        //Bitmap bitmap = Android.Provider.MediaStore.Images.Media.GetBitmap(Application.Context.ContentResolver, cropImageURI);


                        imageViewShowReciept.SetImageBitmap(bitmap);
                        string PictureName = Utils.SplitPath(keepCropedUri.ToString());
                        textNamePic.Text = PictureName;
                    }
                    else
                    {
                        Toast.MakeText(Application.Context, "error : CROP_REQUEST data is null", ToastLength.Short).Show();
                        return;
                    }
                }

                base.OnActivityResult(requestCode, resultCode, data);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnActivityResult at add Qr");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        #endregion
        public void CheckPermission()
        {
            string[] PERMISSIONS;


            if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
            {
                PERMISSIONS = new string[]
                {
                    "android.permission.POST_NOTIFICATION",
                    "android.permission.READ_MEDIA_IMAGES",
                    "android.permission.CAMERA",
                    "android.permission.ACTION_OPEN_DOCUMENT",
                    "android.permission.BLUETOOTH",
                    "android.permission.BLUETOOTH_CONNECT",
                    "android.permission.BLUETOOTH_SCAN",
                    "android.permission.INTERNET",
                    "android.permission.LOCATION_HARDWARE",
                    "android.permission.ACCESS_LOCATION_EXTRA_COMMANDS",
                    "android.permission.ACCESS_MOCK_LOCATION",
                    "android.permission.ACCESS_NETWORK_STATE",
                    "android.permission.ACCESS_WIFI_STATE",
                    "android.permission.INTERNET",

                };
            }
            else if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            {
                PERMISSIONS = new string[]
                {
                    "android.permission.READ_EXTERNAL_STORAGE",
                    "android.permission.WRITE_EXTERNAL_STORAGE",
                    "android.permission.CAMERA",
                    "android.permission.BLUETOOTH",
                    "android.permission.BLUETOOTH_CONNECT"
                };
            }
            else
            {
                PERMISSIONS = new string[]
               {
                    "android.permission.READ_EXTERNAL_STORAGE",
                    "android.permission.WRITE_EXTERNAL_STORAGE",
                    "android.permission.CAMERA",
                    "android.permission.BLUETOOTH"
                };
            }

            int RequestLocationId = 0;

            foreach (var item in PERMISSIONS)
            {
                if (CheckSelfPermission(item) != Permission.Granted)
                {
                    RequestPermissions(PERMISSIONS, RequestLocationId);
                }
                ShouldShowRequestPermissionRationale(item);
            }
        }


        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'myQRReceiptActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'myQRReceiptActivity.openPage' is assigned but its value is never used
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