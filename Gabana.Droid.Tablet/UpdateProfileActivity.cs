using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Model;
using Gabana.ShareSource;
using Gabana3.JAM.Merchant;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;
using Xamarin.Essentials;
using static Kotlin.Jvm.Internal.Ref;

namespace Gabana.Droid.Tablet
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
    public class UpdateProfileActivity : AppCompatActivity
    {
        public static UpdateProfileActivity main;
        EditText txtMerchantName, txtUserName , txtRef ;
        Button btnRegister;
        ImageButton imgProfile, btnAddImage;
        string UserName, MerchantName;
        Android.Net.Uri keepCropedUri = null;
        Android.Graphics.Bitmap bitmap;
        Android.Net.Uri cameraTakePictureUri;
        private static byte[] picture;
        int RESULT_OK = -1;
        int CAMERA_REQUEST = 0;
        int CROP_REQUEST = 1;
        int GALLERY_PICTURE = 2;
        public static bool CurrentActivity = false;
        LinearLayout lnBack;
        private static byte[] bytepicture;
        string PathLogo = "";
        public static string TextError = "";

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.updateprofile_activity);
                main = this;

                txtMerchantName = FindViewById<EditText>(Resource.Id.txtMerchantName);
                txtUserName = FindViewById<EditText>(Resource.Id.txtUserName);//OwnerId
                txtRef = FindViewById<EditText>(Resource.Id.txtRef);

                btnRegister = FindViewById<Button>(Resource.Id.btnRegister);
                imgProfile = FindViewById<ImageButton>(Resource.Id.imgProfile);
                btnAddImage = FindViewById<ImageButton>(Resource.Id.btnAddImage);
                lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;
                CurrentActivity = true;

                btnAddImage.Click += BtnAddImage_Click;
                btnRegister.Click += BtnRegister_Click;

                Merchants merchants = DataCashingAll.Merchant;
                if (merchants != null)
                {
                    txtUserName.Text = merchants.Merchant.MerchantID.ToString();
                }
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

        private void LnBack_Click(object sender, EventArgs e)
        {
            Bundle bundle = new Bundle();
            var fragement = new Logout_Dialog_Main();
            fragement.Arguments = bundle;
            fragement.Show(this.SupportFragmentManager, nameof(Logout_Dialog_Main));
        }

        private void BtnAddImage_Click(object sender, EventArgs e)
        {
            var fragment = new UpdateProfile_Dialog_AddImage();
            fragment.Show(SupportFragmentManager, nameof(UpdateProfile_Dialog_AddImage));
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
                    btnRegister.Enabled = true;
                    Bundle bundle = new Bundle();
                    var fragement = new Update_Dialog_Nullcode();      
                    fragement.Arguments = bundle;
                    fragement.Show(SupportFragmentManager, nameof(Update_Dialog_Nullcode));
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
                    },
                    BonusCode = txtRef.Text.Trim(),
                };

                ResultAPI resultAPI = await GabanaAPI.PostMerchant(merchants, picture);
                #endregion

                if (!resultAPI.Status)
                {
                    string language = Preferences.Get("Language", "");
                    string Exception = UtilsAll.GetExceptionPromotion(resultAPI.Message, language);
                    TextError = Exception;
                    Bundle bundle = new Bundle();
                    var fragement = new Update_Dialog_Error();
                    fragement.Arguments = bundle;
                    fragement.Show(SupportFragmentManager, nameof(Update_Dialog_Error));
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
                StartActivity(typeof(SplashActivity));
                this.Finish();
            }
            catch (Exception ex)
            {
                btnRegister.Enabled = true;
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("UpdateMerchant at UpdateMerchant");
                Log.Debug("error", ex.Message);
                Console.WriteLine(ex.Message);
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
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
                CropIntent.PutExtra("return-data", false);


                //string cropedFilePath = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath,
                //                                         Android.OS.Environment.DirectoryPictures,
                //                                         "file_" + Guid.NewGuid().ToString() + ".jpg");

                //Java.IO.File cropedFile = new Java.IO.File(cropedFilePath);

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
                _ = TinyInsights.TrackPageViewAsync("CropImage at UpdateMerchant");
                Toast.MakeText(Application.Context, "error : " + ex.Message, ToastLength.Short).Show(); return;
            }
        }

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
#pragma warning disable CS0618 // 'MediaStore.Images.Media.GetBitmap(ContentResolver?, Uri?)' is obsolete:  
                        bitmap = Android.Provider.MediaStore.Images.Media.GetBitmap(Application.Context.ContentResolver, cropImageURI);
#pragma warning restore CS0618 // 'MediaStore.Images.Media.GetBitmap(ContentResolver?, Uri?)' is obsolete:  

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
    }
}