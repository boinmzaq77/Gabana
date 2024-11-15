using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using LinqToDB.SqlQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class AddmyQRActivity : AppCompatActivity
    {
        public static AddmyQRActivity activity;
        EditText QRCodeName, txtCOmment;
        LinearLayout lnBranch, lnAddImage;
        internal Button btnAdd;
        string UserLogin, path, pathFolderPicture, PicturePath, LoginType;
        MyQrCodeManage MyQrCodeManage = new MyQrCodeManage();
        public static MyQrCode MyQrCodeData;
        TextView textBranch, txtmyQRName;
        FrameLayout lnShowDelete, lnDelete;
        public static string selectbranch = string.Empty;
        char setBranch;

        Android.Net.Uri cameraTakePictureUri;
        Android.Net.Uri keepCropedUri = null;
        Android.Graphics.Bitmap bitmap;
        int RESULT_OK = -1;
        int CAMERA_REQUEST = 0;
        int CROP_REQUEST = 1;
        int GALLERY_PICTURE = 2;
        ImageView imgQRCode;
        public static bool CurrentActivity;
        int MyQrCodeRunning = 0;
        public static long Focusitem;
        bool first = true, flagdatachange = false;
        byte[] imageByte;
        internal static List<Branch> ListChooseBranch = new List<Branch>();
        DialogLoading dialogLoading = new DialogLoading();
        BranchManage branchManage = new BranchManage();

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.addmyqr_activity_main);

                activity = this;
                FrameLayout lnBack = FindViewById<FrameLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;

                QRCodeName = FindViewById<EditText>(Resource.Id.txtGiftCode);
                txtCOmment = FindViewById<EditText>(Resource.Id.txtCOmment);
                txtmyQRName = FindViewById<TextView>(Resource.Id.txtmyQRName);
                lnBranch = FindViewById<LinearLayout>(Resource.Id.lnBranch);
                lnAddImage = FindViewById<LinearLayout>(Resource.Id.lnAddImage);
                btnAdd = FindViewById<Button>(Resource.Id.btnAdd);
                textBranch = FindViewById<TextView>(Resource.Id.textBranch);
                lnShowDelete = FindViewById<FrameLayout>(Resource.Id.lnShowDelete);
                lnDelete = FindViewById<FrameLayout>(Resource.Id.lnDelete);
                imgQRCode = FindViewById<ImageView>(Resource.Id.ImgQRCode);

                TextView textTitle = FindViewById<TextView>(Resource.Id.textTitle);

                CheckPermission();
                UserLogin = Preferences.Get("User", "");
                LoginType = Preferences.Get("LoginType", "");
                LoginType = LoginType.ToLower();

                CurrentActivity = true;
                pathFolderPicture = DataCashingAll.PathFolderImage;
                lnBranch.Click += LnBranch_Click;
                lnAddImage.Click += LnAddImage_Click;
                lnDelete.Click += LnDelete_Click;
                lnShowDelete.Click += LnDelete_Click;
                QRCodeName.TextChanged += QRCodeName_TextChanged;
                txtCOmment.TextChanged += QRCodeName_TextChanged;
                textBranch.TextChanged += QRCodeName_TextChanged;

                if (dialogLoading != null & dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
                }

                CheckJwt();

                MyQrCodeRunning = await MyQrCodeManage.GetMyQrCodeNo(DataCashingAll.MerchantId);

                if (MyQrCodeData != null)
                {
                    textTitle.Text = GetString(Resource.String.addmyqr_activity_editmyqr);
                    btnAdd.Text = GetString(Resource.String.addmyqr_activity_editmyqr);
                    await Showdata();
                    btnAdd.Click += BtnEdit_Click;
                    lnShowDelete.Visibility = ViewStates.Visible;                    
                }
                else
                {
                    textTitle.Text = GetString(Resource.String.addmyqr_activity_addmyqr);
                    btnAdd.Text = GetString(Resource.String.textsave);
                    textBranch.Text = "All Branch";
                    selectbranch = null;
                    setBranch = 'A';                    
                    var lstBranch = await branchManage.GetAllBranch(DataCashingAll.MerchantId);
                    ListChooseBranch = lstBranch;
                    btnAdd.Click += BtnAdd_Click;
                    lnShowDelete.Visibility = ViewStates.Gone;                    
                }
                first = false;
                SetButtonAdd(false);

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }

                _ = TinyInsights.TrackPageViewAsync("OnCreate : AddmyQRActivity");

            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Oncreate at add Qr");
                return;
            }
        }

        private void CheckDataChange()
        {
            if (first)
            {
                SetButtonAdd(false);
                return;
            }
            if (MyQrCodeData == null)
            {
                if (!string.IsNullOrEmpty(QRCodeName.Text))
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                if (!string.IsNullOrEmpty(txtCOmment.Text))
                {
                    flagdatachange = true;
                    return;
                }
                if (keepCropedUri != null)
                {
                    flagdatachange = true;
                    return;
                }
                if (selectbranch == null)
                {
                    if (setBranch == 'A')
                    {
                        flagdatachange = false;
                    }
                    else
                    {
                        flagdatachange = true;
                    }
                }
                else
                {
                    flagdatachange = true;
                }

                SetButtonAdd(false);
            }
            else
            {
                if (QRCodeName.Text != MyQrCodeData.MyQrCodeName)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                if (txtCOmment.Text != MyQrCodeData.Comments)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }

                if (MyQrCodeData.FMyQrAllBranch != setBranch)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }

                if (MyQrCodeData.FMyQrAllBranch == 'B')
                {
                    int? sysBranchida = MyQrCodeData.SysBranchID == null ? (int?)0 : Convert.ToInt32(MyQrCodeData.SysBranchID);
                    if (sysBranchida != Convert.ToInt32(selectbranch))
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }
                }

                if (keepCropedUri != null)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                if (setBranch != MyQrCodeData.FMyQrAllBranch)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                if (setBranch != MyQrCodeData.FMyQrAllBranch)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                if (selectbranch != MyQrCodeData.SysBranchID?.ToString() && MyQrCodeData.SysBranchID != null)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }

                SetButtonAdd(false);
            }
        }

        private void SetButtonAdd(bool enable)
        {
            if (enable)
            {
                btnAdd.SetBackgroundResource(Resource.Drawable.btnblue);
                btnAdd.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnAdd.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnAdd.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
            }
            btnAdd.Enabled = enable;
        }

        private async void QRCodeName_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                CheckDataChange();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _= TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("QRCodeName_TextChanged at add Qr");
                return;
            }
        }

        private async void LnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                bool checkManinRole = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "myqr");
                if (checkManinRole)
                {
                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                    bundle.PutString("message", myMessage);
                    bundle.PutString("deleteType", "myqrcode");
                    dialog.Arguments = bundle;
                    dialog.Show(SupportFragmentManager, myMessage);
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.notperm), ToastLength.Short).Show();
                    return;
                }                
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("LnDelete_Click at add Qr");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void LnAddImage_Click(object sender, EventArgs e)
        {
            try
            {
                MainDialog dialog = new MainDialog();
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.addcustomer_dialog_addimage.ToString();
                bundle.PutString("message", myMessage);
                bundle.PutString("OpenPicture", "qrcode");
                dialog.Arguments = bundle;
                dialog.Show(SupportFragmentManager, myMessage);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("LnAddImage_Click at add Qr");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void LnBranch_Click(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Android.Content.Intent(Application.Context, typeof(myQRBranchActivity)));
                myQRBranchActivity.listChooseBranch = ListChooseBranch;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("LnBranch_Click at add Qr");
                return;
            }
        }

        private async Task Showdata()
        {
            try
            {
                var lstBranch = await branchManage.GetAllBranch(DataCashingAll.MerchantId);
                btnAdd.Text = GetString(Resource.String.textsave);
                txtCOmment.Text = MyQrCodeData.Comments;
                QRCodeName.Text = MyQrCodeData.MyQrCodeName;

                if (MyQrCodeData.FMyQrAllBranch == 'A')
                {
                    textBranch.Text = "All Branch";
                    selectbranch = null;
                    setBranch = 'A';
                    ListChooseBranch = lstBranch;
                }
                else
                {
                    ListChooseBranch = new List<Branch>();
                    var branch = lstBranch.Where(x=>x.SysBranchID == MyQrCodeData.SysBranchID).FirstOrDefault();
                    ListChooseBranch.Add(branch);
                    selectbranch = string.Empty;

                    if (lstBranch != null)
                    {
                        foreach (var item in ListChooseBranch)
                        {
                            if (selectbranch != "" && selectbranch != "All Branch")
                            {
                                selectbranch += "," + item.BranchName;
                            }
                            else
                            {
                                selectbranch = item.BranchName;
                            }
                        }
                        textBranch.Text = selectbranch;

                        var result = lstBranch.Where(x => x.SysBranchID == Convert.ToInt64(MyQrCodeData.SysBranchID)).FirstOrDefault();
                        selectbranch = result.SysBranchID.ToString();
                        setBranch = 'B';
                    }
                }
                string pathpic = Utils.SplitCloundPath(MyQrCodeData.PicturePath);
                txtmyQRName.Text = pathpic;
                var paths = MyQrCodeData.PicturePath;
                if (!string.IsNullOrEmpty(paths))
                {
                    if (await GabanaAPI.CheckSpeedConnection())
                    {
                        Utils.SetImage(imgQRCode, paths);
                    }
                    else
                    {
                        var local = MyQrCodeData.PictureLocalPath;
                        Android.Net.Uri uri = Android.Net.Uri.Parse(local);
                        imgQRCode.SetImageURI(uri);
                    }
                }
                else
                {
                    imgQRCode.SetImageResource(Resource.Mipmap.DefaultMyQR);
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Showdata at add Qr");
                return;
            }
        }

        private async void BtnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                btnAdd.Enabled = false;
                if (!await GabanaAPI.CheckNetWork())
                {
                    Toast.MakeText(this, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    btnAdd.Enabled = true;
                    return;
                }

                if (!await GabanaAPI.CheckSpeedConnection())
                {
                    Toast.MakeText(this, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                    btnAdd.Enabled = true;
                    return;
                }

                btnAdd.Enabled = false;

                string localpath = string.Empty;
                string Cloudpath = string.Empty;
                CheckNull();
                int? myInt = MyQrCodeData.Ordinary == null ? (int?)null : Convert.ToInt32(MyQrCodeData.Ordinary);
                int? branch = string.IsNullOrEmpty(selectbranch) ? (int?)null : Convert.ToInt32(selectbranch);
                var data = await MyQrCodeManage.GetMyQrCode(DataCashingAll.MerchantId, (int)MyQrCodeData.MyQrCodeNo);
                if (data != null)
                {
                    if (keepCropedUri != null)
                    {
                        path = Utils.SplitPath(keepCropedUri.ToString());
                        var checkResultPicture = await Utils.InsertImageToPicture(path, bitmap);
                        if (checkResultPicture)
                        {
                            PicturePath = pathFolderPicture + path;
                        }
                        imageByte = await Utils.streamImage(bitmap);

                        if (!string.IsNullOrEmpty(data.PictureLocalPath))
                        {
                            Java.IO.File imgFile = new Java.IO.File(data.PictureLocalPath);
                            if (System.IO.File.Exists(imgFile.AbsolutePath))
                            {
                                System.IO.File.Delete(imgFile.AbsolutePath);
                            }
                        }
                    }
                    else
                    {
                        localpath = data.PictureLocalPath;
                        Cloudpath = data.PicturePath;
                    }
                }

                ORM.Master.MyQrCode myQrCode = new ORM.Master.MyQrCode()
                {
                    DateCreated = Utils.GetTranDate(MyQrCodeData.DateCreated),
                    DateModified = Utils.GetTranDate(DateTime.UtcNow),
                    MerchantID = (int)DataCashingAll.MerchantId,
                    Ordinary = myInt,
                    UserNameModified = UserLogin,
                    Comments = txtCOmment.Text,
                    FMyQrAllBranch = setBranch,
                    MyQrCodeName = QRCodeName.Text,
                    MyQrCodeNo = (int)MyQrCodeData.MyQrCodeNo,
                    PicturePath = Cloudpath,
                    SysBranchID = branch
                };

                var result = await GabanaAPI.PutDataMyQrCode(myQrCode, imageByte);
                if (!result.Status)
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    btnAdd.Enabled = true;
                    return;
                }

                ORM.MerchantDB.MyQrCode localmyQrCode = new ORM.MerchantDB.MyQrCode()
                {
                    DateCreated = Utils.GetTranDate(MyQrCodeData.DateCreated),
                    DateModified = Utils.GetTranDate(DateTime.UtcNow),
                    MerchantID = (int)DataCashingAll.MerchantId,
                    Ordinary = myInt,
                    UserNameModified = UserLogin,
                    Comments = txtCOmment.Text,
                    FMyQrAllBranch = setBranch,
                    MyQrCodeName = QRCodeName.Text,
                    MyQrCodeNo = (int)MyQrCodeData.MyQrCodeNo,
                    PicturePath = PicturePath,
                    PictureLocalPath = localpath,
                    SysBranchID = branch
                };
                var update = await MyQrCodeManage.UpdateMyQrCode(localmyQrCode);
                if (!update)
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                    btnAdd.Enabled = true;
                    return;
                }

                MyQrCodeData = null;
                Toast.MakeText(this, GetString(Resource.String.savesucess), ToastLength.Short).Show();
                SettingmyQRActivity.SetFocusQR(localmyQrCode.MyQrCodeNo);
                DataCashingAll.flagMyQrCodeChange = true;
                btnAdd.Enabled = true;
                this.Finish();
            }
            catch (Exception ex)
            {
                btnAdd.Enabled = true;
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnEdit_Click at add Qr");
                return;
            }
        }

        private async void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                btnAdd.Enabled = false;
                if (!await GabanaAPI.CheckNetWork())
                {
                    Toast.MakeText(this, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    btnAdd.Enabled = true;
                    return;
                }

                if (!await GabanaAPI.CheckSpeedConnection())
                {
                    Toast.MakeText(this, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                    btnAdd.Enabled = true;
                    return;
                }

                btnAdd.Enabled = false;
                CheckNull();

                if (keepCropedUri != null)
                {
                    path = Utils.SplitPath(keepCropedUri.ToString());
                    var checkResultPicture = await Utils.InsertImageToPicture(path, bitmap);
                    if (checkResultPicture)
                    {
                        PicturePath = pathFolderPicture + path;
                    }
                    imageByte = await Utils.streamImage(bitmap);
                }

                ORM.Master.MyQrCode myQrCode = new ORM.Master.MyQrCode()
                {
                    DateCreated = Utils.GetTranDate(DateTime.UtcNow),
                    DateModified = Utils.GetTranDate(DateTime.UtcNow),
                    MerchantID = (int)DataCashingAll.MerchantId,
                    Ordinary = null,
                    UserNameModified = UserLogin,
                    Comments = txtCOmment.Text,
                    FMyQrAllBranch = setBranch,
                    MyQrCodeName = QRCodeName.Text,
                    MyQrCodeNo = MyQrCodeRunning,
                    PicturePath = PicturePath,
                    SysBranchID = selectbranch == null ? (int?)null : Convert.ToInt32(selectbranch)
                    // FMyQrAllBranch = 'A' : null,FMyQrAllBranch = 'B' : SysBranchId ที่เลือก
                };

                var result = await GabanaAPI.PostDataMyQrCode(myQrCode, imageByte);
                if (!result.Status)
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    btnAdd.Enabled = true;
                    return;
                }

                ORM.MerchantDB.MyQrCode localmyQrCode = new ORM.MerchantDB.MyQrCode()
                {
                    DateCreated = Utils.GetTranDate(DateTime.UtcNow),
                    DateModified = Utils.GetTranDate(DateTime.UtcNow),
                    MerchantID = (int)DataCashingAll.MerchantId,
                    Ordinary = null,
                    UserNameModified = UserLogin,
                    Comments = txtCOmment.Text,
                    FMyQrAllBranch = setBranch,
                    MyQrCodeName = QRCodeName.Text,
                    MyQrCodeNo = MyQrCodeRunning,
                    PicturePath = PicturePath,
                    SysBranchID = selectbranch == null ? (int?)null : Convert.ToInt32(selectbranch)
                };
                var insert = await MyQrCodeManage.InsertMyQrCode(localmyQrCode);
                if (!insert)
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotinsert), ToastLength.Short).Show();
                    btnAdd.Enabled = true;
                    return;
                }

                Toast.MakeText(this, GetString(Resource.String.insertsucess), ToastLength.Short).Show();
                SettingmyQRActivity.SetFocusQR(localmyQrCode.MyQrCodeNo);
                DataCashingAll.flagMyQrCodeChange = true;
                btnAdd.Enabled = true;
                this.Finish();
            }
            catch (Exception ex)
            {
                btnAdd.Enabled = true;
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnAdd_Click at add Qr");
                return;
            }
        }

        void CheckNull()
        {
            if (string.IsNullOrEmpty(QRCodeName.Text) | setBranch == '\0')
            {
                Toast.MakeText(this, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                return;
            }
        }

        async void SetBranchData()
        {
            var lstBranch = await branchManage.GetAllBranch(DataCashingAll.MerchantId);


            if (lstBranch.Count == ListChooseBranch.Count || ListChooseBranch.Count == 0)
            {
                setBranch = 'A';
                selectbranch = null;
                textBranch.Text = GetString(Resource.String.addemployee_allbranch);
            }
            else
            {
                setBranch = 'B';
                textBranch.Text = "";
                foreach (var item in ListChooseBranch)
                {
                    if (textBranch.Text != "")
                    {
                        textBranch.Text += "," + item.BranchName;
                        selectbranch += "," + item.SysBranchID;

                    }
                    else
                    {
                        textBranch.Text = item.BranchName;
                        selectbranch = item.SysBranchID.ToString();
                    }
                }
            }
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            try
            {
                OnBackPressed();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("LnBack_Click at add qr");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show(); return;
            }
        }

        public void DialogCheckBack()
        {
            base.OnBackPressed();
            flagdatachange = false;
            if (MyQrCodeData != null)
            {
                SettingmyQRActivity.SetFocusQR(MyQrCodeData.MyQrCodeNo);
                DataCashingAll.flagMyQrCodeChange = false;
            }
            MyQrCodeData = null;
        }

        public override void OnBackPressed()
        {
            try
            {
                if (MyQrCodeData != null)
                {
                    if (!flagdatachange)
                    {
                        DialogCheckBack(); return;
                    }

                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.add_dialog_back.ToString();
                    bundle.PutString("message", myMessage);
                    bundle.PutString("fromPage", "myqr");
                    dialog.Arguments = bundle;
                    dialog.Show(SupportFragmentManager, myMessage);
                    return;
                }
                else
                {
                    if (!flagdatachange)
                    {
                        DialogCheckBack(); return;
                    }

                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.edit_dialog_back.ToString();
                    bundle.PutString("message", myMessage);
                    bundle.PutString("fromPage", "myqr");
                    dialog.Arguments = bundle;
                    dialog.Show(SupportFragmentManager, myMessage);
                    return;

                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnBackPressed at add qr");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show(); return;
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                CheckJwt();
                UserLogin = Preferences.Get("User", "");
                SetBranchData();
            }
            catch (Exception)
            {
                base.OnRestart();
            }
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
                Toast.MakeText(this, "error : " + ex.Message, ToastLength.Short).Show(); return;
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

                //string cropedFilePath = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath,
                //                                         Android.OS.Environment.DirectoryPictures,
                //                                         "file_" + Guid.NewGuid().ToString() + ".jpg");
                //Java.IO.File cropedFile = new Java.IO.File(cropedFilePath);


                string filePath = DataCashingAll.PathImageBill;
                string fullName = filePath + "file_" + Guid.NewGuid().ToString() + ".jpg";

                Java.IO.File tempFile = new Java.IO.File(fullName);


                // create new file handle to get full resolution crop
                Android.Net.Uri cropedUri;

                if (Build.VERSION.SdkInt >= BuildVersionCodes.N)
                {
                    cropedUri = Android.Support.V4.Content.FileProvider.GetUriForFile(Application.Context, Application.Context.ApplicationContext.PackageName + ".fileProvider", tempFile);

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
                    cropedUri = Android.Net.Uri.FromFile(tempFile);
                }
                keepCropedUri = cropedUri;  // เก็บเอาไว้ใช้งานที่ OnActionResult เพราะ Android ที่ต่ำกว่า Android.N จะ Get เอาจาก ค่าที่ส่งไปใน Functio ไม่ได้
                CheckDataChange();

                CropIntent.PutExtra(MediaStore.ExtraOutput, cropedUri);
                StartActivityForResult(CropIntent, CROP_REQUEST);
            }
            catch (ActivityNotFoundException ex)
            {
                Log.Error("error", "CropImage" + ex.Message);
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CropImage at add qr");
                Toast.MakeText(this, "error : CropImage : " + ex.Message, ToastLength.Short).Show(); return;
            }
        }


        public void CameraTakePicture()
        {
            try
            {
                Intent CamIntent = new Intent(MediaStore.ActionImageCapture);
                CamIntent.AddFlags(ActivityFlags.GrantWriteUriPermission);

                //ถ้ากำหนดชื่อชื่อไฟล์ มันจะ Save ลงที่ไฟล์นี้แล้วส่งไปให้ OnActivityResult
                //string filePath = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath,
                //                                         Android.OS.Environment.DirectoryPictures,
                //                                         "file_" + Guid.NewGuid().ToString() + ".jpg");
                //Java.IO.File tempFile = new Java.IO.File(filePath);

                string filePath = DataCashingAll.PathImageBill;
                string fullName = filePath + "file_" + Guid.NewGuid().ToString() + ".jpg";

                Java.IO.File tempFile = new Java.IO.File(fullName);


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
                Toast.MakeText(this, "error : " + ex.Message, ToastLength.Short).Show(); return;
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
#pragma warning disable CS0618 // Type or member is obsolete
                        bitmap = Android.Provider.MediaStore.Images.Media.GetBitmap(Application.Context.ContentResolver, cropImageURI);
#pragma warning restore CS0618 // Type or member is obsolete

                        // Solution 3 : อ่าน BitMap จากไฟล์ โดยเอาค่าไฟล์จาก bundle.GetParcelable(MediaStore.ExtraOutput) : จะ error กับ Andord ที่ต่ำกว่า Android.N
                        //Android.Net.Uri cropImageURI = (Android.Net.Uri)bundle.GetParcelable(MediaStore.ExtraOutput); // ใช้กับ Andord ที่ต่ำกว่า Android.N ไม่ได้ เพราะจะมีค่าเป็น Null
                        //Bitmap bitmap = Android.Provider.MediaStore.Images.Media.GetBitmap(Application.Context.ContentResolver, cropImageURI);

                        imgQRCode.SetImageBitmap(bitmap);
                        string PictureName = Utils.SplitPath(keepCropedUri.ToString());
                        txtmyQRName.Text = PictureName;
                    }
                    else
                    {
                        Toast.MakeText(this, "error : CROP_REQUEST data is null", ToastLength.Short).Show();
                        return;
                    }
                }

                base.OnActivityResult(requestCode, resultCode, data);
            }
            catch (Exception ex)
            {
                Log.Error("error", "OnActivityResult" + ex.Message);
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnActivityResult at add Qr");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        #endregion

        public static void SetMyQrCodeDetail(MyQrCode myQrCode)
        {
            MyQrCodeData = myQrCode;
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

    }
}