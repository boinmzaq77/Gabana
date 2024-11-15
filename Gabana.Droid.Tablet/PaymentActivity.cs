using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Droid.Tablet.Fragments.Customers;
using Gabana.Droid.Tablet.Fragments.Employee;
using Gabana.Droid.Tablet.Fragments.Items;
using Gabana.Droid.Tablet.Fragments.PayMent;
using Gabana.Droid.Tablet.Fragments.POS;
using Gabana.Droid.Tablet.Fragments.Setting;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using static LinqToDB.Reflection.Methods.LinqToDB.Insert;

namespace Gabana.Droid.Tablet
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
    public class PaymentActivity : AppCompatActivity
    {
        public static PaymentActivity payment_main;

        Payment_Fragment_Main payment_fragment_main;
        Payment_Fragment_Cash payment_fragment_cash;
        Payment_Fragment_SaveOrder payment_fragment_saveorder;
        Payment_Fragment_Balance payment_fragment_balance;
        Payment_Fragment_Receipt payment_fragment_receipt;
        Payment_Fragment_GiftVoucher payment_fragment_giftvoucher;
        Payment_Fragment_Credit payment_fragment_credit;
        Payment_Fragment_Debit payment_fragment_debit;
        Payment_Fragment_MyQR payment_fragment_myqr;
        Payment_Fragment_MyQRReceipt payment_fragment_myqrreceipt;
        Payment_Fragment_QRCash payment_fragment_qrcash;

        public static TranWithDetailsLocal tranWithDetails;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.payment_main);
                payment_main = this;
                CombineUI();
                //SetFragment();

                #region initial page
                framL = Resource.Id.content_frameL;
                framR = Resource.Id.content_frameR;

                payment_fragment_main = Payment_Fragment_Main.NewInstance();
                payment_fragment_cash = Payment_Fragment_Cash.NewInstance();

                var transactionCreate = SupportFragmentManager.BeginTransaction();
                transactionCreate.Add(framL, payment_fragment_main, "payment");
                transactionCreate.Show(payment_fragment_main).Commit();

                activeL = payment_main.payment_fragment_main;
                activeR = payment_main.payment_fragment_cash;

                LoadFragment(framL, "payment", "default");
                #endregion
                SetFragment();
                tranWithDetails = MainActivity.tranWithDetails;
            }
            catch (Exception ex)
            {
                 Console.Write(ex.Message);
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                Utils.ShowHi("Welcome");
                tranWithDetails = MainActivity.tranWithDetails;
                
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            Utils.ShowHi("Welcome");
        }
        private void SetFragment()
        {
            try
            {
                framL = Resource.Id.content_frameL;
                framR = Resource.Id.content_frameR;

                payment_fragment_main = Payment_Fragment_Main.NewInstance();
                payment_fragment_cash = Payment_Fragment_Cash.NewInstance();
                payment_fragment_saveorder = Payment_Fragment_SaveOrder.NewInstance();
                payment_fragment_balance = Payment_Fragment_Balance.NewInstance();
                payment_fragment_receipt = Payment_Fragment_Receipt.NewInstance();
                payment_fragment_giftvoucher = Payment_Fragment_GiftVoucher.NewInstance();
                payment_fragment_credit = Payment_Fragment_Credit.NewInstance();
                payment_fragment_debit = Payment_Fragment_Debit.NewInstance();
                payment_fragment_myqr = Payment_Fragment_MyQR.NewInstance();
                payment_fragment_myqrreceipt = Payment_Fragment_MyQRReceipt.NewInstance();
                payment_fragment_qrcash = Payment_Fragment_QRCash.NewInstance();
                

                payment_main.SupportFragmentManager.BeginTransaction().Add(framR, payment_fragment_cash, "cash").Hide(payment_main.payment_fragment_cash).Commit();
                payment_main.SupportFragmentManager.BeginTransaction().Add(framR, payment_fragment_saveorder, "saveorder").Hide(payment_main.payment_fragment_saveorder).Commit();
                payment_main.SupportFragmentManager.BeginTransaction().Add(framL, payment_fragment_balance, "balance").Hide(payment_fragment_balance).Commit();
                payment_main.SupportFragmentManager.BeginTransaction().Add(framR, payment_fragment_receipt, "receipt").Hide(payment_main.payment_fragment_receipt).Commit();
                payment_main.SupportFragmentManager.BeginTransaction().Add(framR, payment_fragment_giftvoucher, "giftvoucher").Hide(payment_main.payment_fragment_giftvoucher).Commit();
                payment_main.SupportFragmentManager.BeginTransaction().Add(framR, payment_fragment_credit, "credit").Hide(payment_main.payment_fragment_credit).Commit();
                payment_main.SupportFragmentManager.BeginTransaction().Add(framR, payment_fragment_debit, "debit").Hide(payment_main.payment_fragment_debit).Commit();
                payment_main.SupportFragmentManager.BeginTransaction().Add(framR, payment_fragment_myqr, "myqr").Hide(payment_main.payment_fragment_myqr).Commit();
                payment_main.SupportFragmentManager.BeginTransaction().Add(framR, payment_fragment_myqrreceipt, "myqrreceipt").Hide(payment_main.payment_fragment_myqrreceipt).Commit();
                payment_main.SupportFragmentManager.BeginTransaction().Add(framR, payment_fragment_qrcash, "qrcash").Hide(payment_main.payment_fragment_qrcash).Commit();


                payment_main.SupportFragmentManager.BeginTransaction().Add(framL, payment_fragment_main, "payment").Show(payment_fragment_main).Commit();
                activeL = payment_main.payment_fragment_main;
                activeR = payment_main.payment_fragment_cash;

            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show(); 
            }
        }

        FrameLayout frameLeft, frameRight;
        int framL, framR;

        private void CombineUI()
        {
            //fragment
            frameLeft = FindViewById<FrameLayout>(Resource.Id.content_frameL);
            frameRight = FindViewById<FrameLayout>(Resource.Id.content_frameR);
        }

        public AndroidX.Fragment.App.Fragment activeL, activeR;
        public void LoadFragment(int id, string left, string right)
        {
            Utils.ShowHi("Welcome");
            DialogLoading dialogLoading = new DialogLoading();
            dialogLoading.Cancelable = false;
            try
            {

                dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                AndroidX.Fragment.App.Fragment fragmentLeft = null;
                AndroidX.Fragment.App.Fragment fragmentRight = null;

                switch (left)
                {
                    case "payment":
                        frameRight.Visibility = ViewStates.Visible;
                        payment_main.SupportFragmentManager.BeginTransaction().Hide(payment_main.activeL).Show(payment_main.payment_fragment_main).Commit();
                        payment_main.activeL = payment_main.payment_fragment_main;
                        switch (right)
                        {
                            case "default":
                                frameRight.Visibility = ViewStates.Invisible;
                                payment_fragment_main.OnResume();
                                break;
                            case "cash":
                                frameRight.Visibility = ViewStates.Visible;
                                payment_main.SupportFragmentManager.BeginTransaction().Hide(payment_main.activeR).Show(payment_main.payment_fragment_cash).Commit();
                                payment_main.activeR = payment_main.payment_fragment_cash;
                                payment_fragment_cash.OnResume();
                                break;
                            case "saveorder":
                                frameRight.Visibility = ViewStates.Visible;
                                payment_main.SupportFragmentManager.BeginTransaction().Hide(payment_main.activeR).Show(payment_main.payment_fragment_saveorder).Commit();
                                payment_main.activeR = payment_main.payment_fragment_saveorder;
                                payment_fragment_saveorder.OnResume();
                                break;
                            case "giftvoucher":
                                frameRight.Visibility = ViewStates.Visible;
                                payment_main.SupportFragmentManager.BeginTransaction().Hide(payment_main.activeR).Show(payment_main.payment_fragment_giftvoucher).Commit();
                                payment_main.activeR = payment_main.payment_fragment_giftvoucher;
                                payment_fragment_giftvoucher.OnResume();
                                break;
                            case "credit":
                                frameRight.Visibility = ViewStates.Visible;
                                payment_main.SupportFragmentManager.BeginTransaction().Hide(payment_main.activeR).Show(payment_main.payment_fragment_credit).Commit();
                                payment_main.activeR = payment_main.payment_fragment_credit;
                                payment_fragment_credit.OnResume();
                                break;
                            case "debit":
                                frameRight.Visibility = ViewStates.Visible;
                                payment_main.SupportFragmentManager.BeginTransaction().Hide(payment_main.activeR).Show(payment_main.payment_fragment_debit).Commit();
                                payment_main.activeR = payment_main.payment_fragment_debit;
                                payment_fragment_debit.OnResume();
                                break;
                            case "myqr":
                                frameRight.Visibility = ViewStates.Visible;
                                payment_main.SupportFragmentManager.BeginTransaction().Hide(payment_main.activeR).Show(payment_main.payment_fragment_myqr).Commit();
                                payment_main.activeR = payment_main.payment_fragment_myqr;
                                payment_fragment_myqr.OnResume();
                                payment_fragment_myqr.showqrclick();
                                break;
                            case "myqrreceipt":
                                frameRight.Visibility = ViewStates.Visible;
                                payment_main.SupportFragmentManager.BeginTransaction().Hide(payment_main.activeR).Show(payment_main.payment_fragment_myqrreceipt).Commit();
                                payment_main.activeR = payment_main.payment_fragment_myqrreceipt;
                                //DataCashing.saveqrReceipt = true;
                                payment_fragment_myqrreceipt.OnResume();
                                break;
                            case "qrcash":
                                frameRight.Visibility = ViewStates.Visible;
                                payment_main.SupportFragmentManager.BeginTransaction().Hide(payment_main.activeR).Show(payment_main.payment_fragment_qrcash).Commit();
                                payment_main.activeR = payment_main.payment_fragment_qrcash;
                                payment_fragment_qrcash.OnResume();
                                payment_fragment_qrcash.showqrclick();
                                break;
                            default:
                                break;
                        }
                        break;
                    case "balance":
                        frameRight.Visibility = ViewStates.Visible;
                        payment_main.SupportFragmentManager.BeginTransaction().Hide(payment_main.activeL).Show(payment_main.payment_fragment_balance).Commit();
                        payment_main.activeL = payment_main.payment_fragment_balance;
                        payment_fragment_balance.OnResume();
                        switch (right)
                        {
                            case "default":
                                frameRight.Visibility = ViewStates.Invisible;
                                break;
                            case "receipt":
                                frameRight.Visibility = ViewStates.Visible;
                                payment_main.SupportFragmentManager.BeginTransaction().Hide(payment_main.activeR).Show(payment_main.payment_fragment_receipt).Commit();
                                payment_main.activeR = payment_main.payment_fragment_receipt;
                                payment_fragment_receipt.OnResume();
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                dialogLoading.Dismiss();

            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                dialogLoading.Dismiss();
            }
        }

        public bool CheckPermission()
        {
            Permission permissionCamera = CheckSelfPermission(Manifest.Permission.Camera);
            Permission permissionRead = CheckSelfPermission(Manifest.Permission.ReadExternalStorage);
            Permission permissionWrite = CheckSelfPermission(Manifest.Permission.WriteExternalStorage);
            Permission permissionBluetooth = CheckSelfPermission(Manifest.Permission.Bluetooth);
            Permission permissionBluetoothC = CheckSelfPermission(Manifest.Permission.BluetoothConnect);

            string[] PERMISSIONS;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
            {
                PERMISSIONS = new string[]
                {
                    "android.permission.READ_EXTERNAL_STORAGE",
                    "android.permission.WRITE_EXTERNAL_STORAGE",
                    "android.permission.CAMERA",
                    "android.permission.BLUETOOTH",
                    "android.permission.BLUETOOTH_CONNECT"
                };
                if (permissionCamera != Permission.Granted
                    || permissionRead != Permission.Granted
                    || permissionWrite != Permission.Granted
                    || permissionBluetooth != Permission.Granted
                    || permissionBluetoothC != Permission.Granted)
                {
                    RequestPermissions(PERMISSIONS, 1);
                    return false;
                }
                return true;
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
                if (permissionCamera != Permission.Granted || permissionRead != Permission.Granted || permissionWrite != Permission.Granted || permissionBluetooth != Permission.Granted)
                {
                    RequestPermissions(PERMISSIONS, 1);
                    return false;
                }
                return true;
            }
        }




        int CAMERA_REQUEST = 0;
        int GALLERY_PICTURE = 2;
        Android.Net.Uri cameraTakePictureUri;
        public void CameraTakePicture()
        {
            try
            {
                Intent CamIntent = new Intent(MediaStore.ActionImageCapture);
                CamIntent.AddFlags(ActivityFlags.GrantWriteUriPermission);

                //ถ้ากำหนดชื่อชื่อไฟล์ มันจะ Save ลงที่ไฟล์นี้แล้วส่งไปให้ OnActivityResult

                string filePath = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath,
                                                         Android.OS.Environment.DirectoryPictures,
                                                         "file_" + Guid.NewGuid().ToString() + ".jpg");

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
            catch (ActivityNotFoundException ed)
            {
                Toast.MakeText(this, "Can not open CAMERA", ToastLength.Short).Show();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Tack Pic at add merchant");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        public void GalleryOpen()
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
                GalIntent.SetType("image/*");
                StartActivityForResult(Intent.CreateChooser(GalIntent, "Select image from gallery"), GALLERY_PICTURE);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GalleryOpen at Merchant");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        int RESULT_OK = -1;
        int CROP_REQUEST = 1;
        Android.Net.Uri keepCropedUri = null;
        Android.Graphics.Bitmap bitmap;
        protected async override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);

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
                        Toast.MakeText(this, "error : GALLERY_PICTURE data is null", ToastLength.Short).Show();
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
                        bitmap = Android.Provider.MediaStore.Images.Media.GetBitmap(Application.Context.ContentResolver, cropImageURI);

                        // Solution 3 : อ่าน BitMap จากไฟล์ โดยเอาค่าไฟล์จาก bundle.GetParcelable(MediaStore.ExtraOutput) : จะ error กับ Andord ที่ต่ำกว่า Android.N
                        //Android.Net.Uri cropImageURI = (Android.Net.Uri)bundle.GetParcelable(MediaStore.ExtraOutput); // ใช้กับ Andord ที่ต่ำกว่า Android.N ไม่ได้ เพราะจะมีค่าเป็น Null
                        //Bitmap bitmap = Android.Provider.MediaStore.Images.Media.GetBitmap(Application.Context.ContentResolver, cropImageURI);

                        DataCashing.flagChooseMedia = true;
                        if (activeR == payment_main.payment_fragment_myqrreceipt)
                        {
                            Payment_Fragment_MyQRReceipt.imageViewShowReciept.SetImageBitmap(bitmap);
                            Payment_Fragment_MyQRReceipt.keepCropedUri = keepCropedUri;
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, "error : CROP_REQUEST data is null", ToastLength.Short).Show();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnActivityResult at Merchant");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        public async void CropImage(Android.Net.Uri imageUri)
        {
            try
            {

                Intent CropIntent = new Intent("com.android.camera.action.CROP");
                CropIntent.SetDataAndType(imageUri, "image/*");
                CropIntent.AddFlags(ActivityFlags.GrantReadUriPermission); // **** ต้อง อนุญาติให้อ่าน ได้ด้วยนะครับ สำคัญ มันจะสามารถอ่านไฟล์ที่ได้จากการ CaptureImage ได้ ****

                CropIntent.PutExtra("crop", "true");
                CropIntent.PutExtra("outputX", 600);
                CropIntent.PutExtra("outputY", 600);
                CropIntent.PutExtra("aspectX", 1);
                CropIntent.PutExtra("aspectY", 1);
                CropIntent.PutExtra("scaleUpIfNeeded", true);
                // do not use return data for big images
                CropIntent.PutExtra("return-data", false);


                //string cropedFilePath = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath,
                //                                         Android.OS.Environment.DirectoryPictures,
                //                                         "file_" + Guid.NewGuid().ToString() + ".jpg");

                string filePath = DataCashingAll.PathImageBill;
                string fullName = filePath + "file_" + Guid.NewGuid().ToString() + ".jpg";

                Java.IO.File cropedFile = new Java.IO.File(fullName); 

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
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("CropImage at add merchant");
                Toast.MakeText(this, "error : " + ex.Message, ToastLength.Short).Show(); return;
            }
        }
    }
    public class ListPayment
    {
        public List<TranPayment> tranPayments;
        static List<TranPayment> builitem;
        public ListPayment(List<TranPayment> tranPayments)
        {
            builitem = tranPayments;
            this.tranPayments = builitem;
        }
        public int Count
        {
            get
            {
                return tranPayments == null ? 0 : tranPayments.Count;
            }
        }
        public TranPayment this[int i]
        {
            get { return tranPayments == null ? null : tranPayments[i]; }
        }
    }
}