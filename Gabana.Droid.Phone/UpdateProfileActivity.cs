using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Android.Support.V7.App;
using Android.Util;
using Android.Widget;
using Gabana.Model;
using Gabana.ShareSource;
using Gabana3.JAM.Merchant;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class UpdateProfileActivity : AppCompatActivity
    {
        public static UpdateProfileActivity main;
        public static EditText txtMerchantName, txtUserName, txtRef;
        Button btnRegister;
        ImageButton imgProfile, btnAddImage;
        string UserName, MerchantName;
        Android.Net.Uri keepCropedUri = null;
        Android.Graphics.Bitmap bitmap;
        Android.Net.Uri cameraTakePictureUri;
        private static byte[] bytepicture;
        int RESULT_OK = -1;
        int CAMERA_REQUEST = 0;
        int CROP_REQUEST = 1;
        int GALLERY_PICTURE = 2;
        public static bool CurrentActivity = false;
        LinearLayout lnBack;
        string PathLogo = "";
        public static string TextError = "";
#pragma warning disable CS0649 // Field 'UpdateProfileActivity.permissionCamera' is never assigned to, and will always have its default value
        Permission permissionCamera;
#pragma warning restore CS0649 // Field 'UpdateProfileActivity.permissionCamera' is never assigned to, and will always have its default value
#pragma warning disable CS0169 // The field 'UpdateProfileActivity.permissionRead' is never used
        Permission permissionRead;
#pragma warning restore CS0169 // The field 'UpdateProfileActivity.permissionRead' is never used
#pragma warning disable CS0169 // The field 'UpdateProfileActivity.permissionWrite' is never used
        Permission permissionWrite;
#pragma warning restore CS0169 // The field 'UpdateProfileActivity.permissionWrite' is never used


        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.updateprofile_activity_main);
                main = this;

                txtMerchantName = FindViewById<EditText>(Resource.Id.txtMerchantName);
                txtUserName = FindViewById<EditText>(Resource.Id.txtUserName);//OwnerId
                txtRef = FindViewById<EditText>(Resource.Id.txtRef);// Refferal Code
                btnRegister = FindViewById<Button>(Resource.Id.btnRegister);
                imgProfile = FindViewById<ImageButton>(Resource.Id.imgProfile);
                btnAddImage = FindViewById<ImageButton>(Resource.Id.btnAddImage);
                lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;
                CurrentActivity = true;

                btnAddImage.Click += BtnAddImage_Click;
                btnRegister.Click += BtnRegister_Click;
                CheckJwt();
                CheckPermission();

                Merchants merchants = DataCashingAll.Merchant;
                if (merchants != null)
                {
                    txtUserName.Text = merchants.Merchant.MerchantID.ToString();
                }
                _ = TinyInsights.TrackPageViewAsync("OnCreate : UpdateProfileActivity");

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at UpdateMerchant");
                Log.Debug("error", ex.Message);
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

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

        public async void UpdateMerchant()
        {
            try
            {
                #region Post Merchant to GabanaAPI

                Merchants merchants = new Gabana3.JAM.Merchant.Merchants()
                {
                    Merchant = new ORM.Master.Merchant()
                    {
                        LogoPath = PathLogo,
                        Name = MerchantName
                    },
                    Device = new ORM.Master.Device()
                    {
                        UDID = DataCashingAll.DeviceUDID,
                        Platform = DataCashingAll.DevicePlatform
                    },
                    UserAccountInfo = new List<ORM.Master.UserAccountInfo>()
                    {
                        new ORM.Master.UserAccountInfo()
                        {
                            UserName = txtUserName.Text,
                        }
                    }
                    ,
                    BonusCode = txtRef.Text.Trim(),
                };

                ResultAPI resultAPI = await GabanaAPI.PostMerchant(merchants, bytepicture);

                //update ที่ Saeauth
                //Gabana.Model.useraccount APIUser = new Model.useraccount()
                //{
                //    UserName = "Owner",
                //    FullName = txtUserName.Text,
                //    MainRoles = "Admin",
                //    PasswordHash =  string.Empty,
                //    //Code = "",
                //    //MerchantName = MerchantName 
                //};
                //var result = await GabanaAPI.PutSeAuthDataUserAccount(APIUser);             
                #endregion

                if (!resultAPI.Status)
                {
                    string language = Preferences.Get("Language", "");
                    string Exception = UtilsAll.GetExceptionPromotion(resultAPI.Message, language);
                    TextError = Exception;
                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.update_dialog_error.ToString();
                    bundle.PutString("message", myMessage);
                    dialog.Arguments = bundle;
                    dialog.Show(main.SupportFragmentManager, myMessage);
                    btnRegister.Enabled = true;
                    return;
                }

                CreateNewMerchant createNewMerchant = new CreateNewMerchant()
                {
                    createNew = true,
                    MerchantID = merchants.Merchant.MerchantID,
                };
                var createNewDB = JsonConvert.SerializeObject(createNewMerchant);
                Preferences.Set("CreateDB", createNewDB);
                main.StartActivity(typeof(SplashActivity));
                main.Finish();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("UpdateMerchant at UpdateMerchant");
                Log.Debug("error", ex.Message);
                Console.WriteLine(ex.Message);
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        public override void OnBackPressed()
        {
            try
            {
                MainDialog dialog = new MainDialog();
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.logout_dialog_main.ToString();
                bundle.PutString("message", myMessage);
                dialog.Arguments = bundle;
                dialog.Show(SupportFragmentManager, myMessage);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnBackPressed at UpdateMerchant");
            }
        }

        private void BtnAddImage_Click(object sender, EventArgs e)
        {
            var fragment = new UpdateMerchant_Dialog_AddImage();
            fragment.Show(SupportFragmentManager, nameof(UpdateMerchant_Dialog_AddImage));
        }


        private async void BtnRegister_Click(object sender, EventArgs e)
        {
            btnRegister.Enabled = false;
            try
            {
                if (txtMerchantName.Text == string.Empty || txtUserName.Text == string.Empty)
                {
                    Toast.MakeText(this, "กรุณากรอกข้อมูล", ToastLength.Short).Show();
                    btnRegister.Enabled = true;
                    return;
                }

                MerchantName = txtMerchantName.Text;
                UserName = txtUserName.Text;


                if (keepCropedUri != null)
                {
                    bytepicture = await Utils.streamImage(bitmap);
                    PathLogo = keepCropedUri.ToString();
                }
                else
                {
                    PathLogo = "";
                }

                if (string.IsNullOrEmpty(txtRef.Text))
                {
                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.update_dialog_nullcode.ToString();
                    bundle.PutString("message", myMessage);
                    dialog.Arguments = bundle;
                    dialog.Show(SupportFragmentManager, myMessage);
                }
                else
                {
                    UpdateMerchant();
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnRegister_Click at UpdateMerchant");
                Log.Debug("error", ex.Message);
                Console.WriteLine(ex.Message);
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }


        //-------------------------------------------------------------------------
        //Picture
        //--------------------------------------------------------------------------
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
                _ = TinyInsights.TrackPageViewAsync("GalleryOpen at UpdateMerchant");
                await TinyInsights.TrackErrorAsync(ex);
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
                CropIntent.PutExtra("outputX", 600);
                CropIntent.PutExtra("outputY", 600);
                CropIntent.PutExtra("aspectX", 1);
                CropIntent.PutExtra("aspectY", 1);
                CropIntent.PutExtra("scaleUpIfNeeded", true);
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
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CropImage at UpdateMerchant");
                Toast.MakeText(Application.Context, "error : " + ex.Message, ToastLength.Short).Show(); return;
            }
        }
        public void CameraTakePicture()
        {
            try
            {
                if (permissionCamera != Permission.Granted)
                {
                    Toast.MakeText(Application.Context, "Permission Camera Denie", ToastLength.Short).Show(); return;
                }
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
                _ = TinyInsights.TrackPageViewAsync("CameraTakePicture at UpdateMerchant");
                Toast.MakeText(Application.Context, "error : " + ex.Message, ToastLength.Short).Show(); return;
            }
        }
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

                        imgProfile.SetImageBitmap(bitmap);
                    }
                    else
                    {
                        Toast.MakeText(Application.Context, "error : CROP_REQUEST data is null", ToastLength.Short).Show();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnActivityResult at UpdateMerchant");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'UpdateProfileActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'UpdateProfileActivity.openPage' is assigned but its value is never used
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
