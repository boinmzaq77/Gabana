using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.ShareSource.Manage;
using Gabana.ShareSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static LinqToDB.Reflection.Methods.LinqToDB;
using TinyInsightsLib;
using static FFImageLoading.Work.ImageInformation;
using Gabana.ORM.MerchantDB;
using Xamarin.Essentials;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Model;
using System.Threading.Tasks;
using Android.Graphics;
using EDMTDev.ZXingXamarinAndroid;
using ZXing;

namespace Gabana.Droid.Tablet.Fragments.Setting
{
    public class Setting_Fragment_AddMyQR : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Setting_Fragment_AddMyQR NewInstance()
        {
            Setting_Fragment_AddMyQR frag = new Setting_Fragment_AddMyQR();
            return frag;
        }
        View view;
        public static Setting_Fragment_AddMyQR fragment_myqr;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.setting_fragment_addmyqr, container, false);
            try
            {
                fragment_addmyqr = this;
                CheckJwt();
                CombineUI();
                SetUIEvent();
                return view;
            }
            catch (Exception)
            {
                return view;
            }
        }

        private void SetUIEvent()
        {
            lnBack.Click += LnBack_Click;
            lnBranch.Click += LnBranch_Click;
            lnAddImage.Click += LnAddImage_Click;
            lnDelete.Click += LnDelete_Click;
            lnShowDelete.Click += LnDelete_Click;
            QRCodeName.TextChanged += QRCodeName_TextChanged;
            txtCOmment.TextChanged += QRCodeName_TextChanged;
            textBranch.TextChanged += QRCodeName_TextChanged;
            btnAdd.Click += BtnAdd_Click;
        }

        EditText QRCodeName, txtCOmment;
        TextView textBranch, txtmyQRName;
        LinearLayout lnBranch, lnAddImage;
        internal Button btnAdd;
        FrameLayout lnShowDelete, lnDelete;
        TextView textTitle;
        public static string selectbranch = string.Empty;
        char setBranch;
        internal static List<Branch> ListChooseBranch = new List<Branch>();
        MyQrCodeManage myqrCodeManage = new MyQrCodeManage();
        string UserLogin, path, pathFolderPicture, PicturePath;
        byte[] imageByte;
        Android.Graphics.Bitmap bitmap;
        int MyQrCodeRunning = 0;
        bool first = true;
        internal static bool flagdatachange = false;
        FrameLayout lnBack;
        internal static ImageView imgQRCode;
        internal static Android.Net.Uri keepCropedUri = null;
        public static Setting_Fragment_AddMyQR fragment_addmyqr;
        BranchManage branchManage = new BranchManage();
        private void CombineUI()
        {
            QRCodeName = view.FindViewById<EditText>(Resource.Id.txtGiftCode);
            txtCOmment = view.FindViewById<EditText>(Resource.Id.txtCOmment);
            txtmyQRName = view.FindViewById<TextView>(Resource.Id.txtmyQRName);
            lnBranch = view.FindViewById<LinearLayout>(Resource.Id.lnBranch);
            lnAddImage = view.FindViewById<LinearLayout>(Resource.Id.lnAddImage);
            btnAdd = view.FindViewById<Button>(Resource.Id.btnAdd);
            textBranch = view.FindViewById<TextView>(Resource.Id.textBranch);
            lnShowDelete = view.FindViewById<FrameLayout>(Resource.Id.lnShowDelete);
            lnDelete = view.FindViewById<FrameLayout>(Resource.Id.lnDelete);
            imgQRCode = view.FindViewById<ImageView>(Resource.Id.ImgQRCode);
            textTitle = view.FindViewById<TextView>(Resource.Id.textTitle);
            lnBack = view.FindViewById<FrameLayout>(Resource.Id.lnBack);
        }

        public override void OnResume()
        {
            base.OnResume();

            //if (!IsVisible)
            //{
            //    return;
            //}

            CheckJwt();
            if (DataCashing.flagChooseMedia)
            {
                SetImgMyQr();
                return;
            }
            first = true;
            UINewMyQR();
            SetDetailMyQr();
            first = false;
            flagdatachange = false;
            SetButtonAdd(false);
        }

        private void SetImgMyQr()
        {
            try
            {
                if (keepCropedUri != null)
                {
                    //Clear รูปภาพก่อนทำอะไรใหม่
                    string setpathnull = string.Empty;
                    Android.Net.Uri urisetpathnull = Android.Net.Uri.Parse(setpathnull);
                    imgQRCode.SetImageURI(urisetpathnull);

                    Android.Net.Uri cropImageURI = keepCropedUri;
                    bitmap = Android.Provider.MediaStore.Images.Media.GetBitmap(Application.Context.ContentResolver, cropImageURI);
                    imgQRCode.SetImageBitmap(bitmap);

                    //เพิ่มฟังก์ชันสำหรับแสดงชื่อรูปภาพ
                    string PictureName = Utils.SplitPath(keepCropedUri.ToString());
                    txtmyQRName.Text = PictureName;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetImgProfile at add Customer");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void SetDetailMyQr()
        {
            try
            {
                UserLogin = Preferences.Get("User", "");
                pathFolderPicture = DataCashingAll.PathFolderImage;
                               
                if (DataCashing.EditMyQR != null)
                {
                    MyQrCodeData = DataCashing.EditMyQR;
                    textTitle.Text = GetString(Resource.String.addmyqr_activity_editmyqr);
                    btnAdd.Text = GetString(Resource.String.addmyqr_activity_editmyqr);
                    await Showdata();
                    lnShowDelete.Visibility = ViewStates.Visible;                    
                }
                else
                {
                    MyQrCodeRunning = await myqrCodeManage.GetMyQrCodeNo(DataCashingAll.MerchantId);
                    textTitle.Text = GetString(Resource.String.addmyqr_activity_addmyqr);
                    btnAdd.Text = GetString(Resource.String.textsave);
                    textBranch.Text = "All Branch";
                    selectbranch = null;
                    setBranch = 'A';                    
                    var lstBranch = await branchManage.GetAllBranch(DataCashingAll.MerchantId);
                    ListChooseBranch = lstBranch;                    
                    lnShowDelete.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        public void UINewMyQR()
        {
            try
            {
                QRCodeName.Text = string.Empty;
                txtCOmment.Text = string.Empty;
                txtmyQRName.Text = string.Empty;
                textBranch.Text = string.Empty;
                textTitle.Text = string.Empty;
                string setpathnull = string.Empty;
                Android.Net.Uri urisetpathnull = Android.Net.Uri.Parse(setpathnull);
                imgQRCode.SetImageURI(urisetpathnull);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private async void CheckJwt()
        {
            try
            {
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                    this.Activity.Finish();
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

        private void QRCodeName_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                CheckDataChange();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                _= TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("QRCodeName_TextChanged at add Qr");
                return;
            }
        }
        private async void LnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                MainDialog dialog = new MainDialog();
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.setting_dialog_deletemyqr.ToString();
                bundle.PutString("message", myMessage);
                dialog.Arguments = bundle;
                dialog.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("LnDelete_Click at add Qr");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private void LnAddImage_Click(object sender, EventArgs e)
        {
            try
            {
                DataCashing.flagChooseMedia = true;
                var fragment = new Setting_Dailog_AddImageMyQR();
                fragment.Show(MainActivity.main_activity.SupportFragmentManager, nameof(Setting_Dailog_AddImageMyQR));
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("LnAddImage_Click at add Qr");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        public static MyQrCode MyQrCodeData;
        public void CheckDataChange()
        {
            if (first)
            {
                SetButtonAdd(false);
                return;
            }
            if (DataCashing.EditMyQR == null)
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
                MyQrCodeData = DataCashing.EditMyQR;
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
                if (selectbranch == null)
                {
                    if (setBranch == 'A')
                    {
                        flagdatachange = false;
                    }
                    SetButtonAdd(true);
                    return;
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
                btnAdd.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnAdd.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnAdd.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnAdd.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }
            btnAdd.Enabled = enable;
        }
        private async void LnBranch_Click(object sender, EventArgs e)
        {
            try
            {
                Myqr_Dialog_SelectBranch.listChooseBranch = ListChooseBranch;
                var fragment = new Myqr_Dialog_SelectBranch();
                fragment.Show(MainActivity.main_activity.SupportFragmentManager, nameof(Myqr_Dialog_SelectBranch));
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("LnBranch_Click at add Qr");
                return;
            }
        }
        private void LnBack_Click(object sender, EventArgs e)
        {
            try
            {
                if (!flagdatachange)
                {
                    SetClearData();
                    return;
                }

                if (DataCashing.EditMyQR == null)
                {
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.add_dialog_back.ToString();
                    bundle.PutString("message", myMessage);
                    Add_Dialog_Back.SetPage("myqr");
                    Add_Dialog_Back add_Dialog = Add_Dialog_Back.NewInstance();
                    add_Dialog.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
                    return;
                }
                else
                {
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.edit_dialog_back.ToString();
                    bundle.PutString("message", myMessage);
                    Edit_Dialog_Back.SetPage("myqr");
                    Edit_Dialog_Back edit_Dialog = Edit_Dialog_Back.NewInstance();
                    edit_Dialog.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
                    return;
                }                 
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("LnBack_Click at add qr");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show(); return;
            }
        }

        public async void SetClearData()
        {
            try
            {
                if (MyQrCodeData != null)
                {
                    Setting_Fragment_MyQR.SetFocusQR(MyQrCodeData.MyQrCodeNo);
                }
                UINewMyQR();
                DataCashing.flagChooseMedia = false;
                flagdatachange = false;
                DataCashing.EditMyQR = null;
                MyQrCodeData = null;
                keepCropedUri = null;
                bitmap = null;
                imageByte = null;
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "myqr");
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetClearData at AddMyQr");
            }
        }
        private async void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                btnAdd.Enabled = false;
                ManageMyQr();
                btnAdd.Enabled = true;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnAdd_Click at add Qr");
                return;
            }
        }

        private async void ManageMyQr()
        {
            try
            {
                bool check = false;
                if (DataCashing.EditMyQR == null)
                {
                    check = await InsertMyQR();
                    if (!check) return;
                }
                else
                {
                    check = await UpdateMyQR();
                    if (!check) return;
                }
                
                SetClearData();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private async Task<bool> UpdateMyQR()
        {
            try
            {
                if (!DataCashing.CheckNet)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    return false;
                }
                btnAdd.Enabled = false;

                string localpath = string.Empty;
                string Cloudpath = string.Empty;
                CheckNull();
                int? myInt = MyQrCodeData.Ordinary == null ? (int?)null : Convert.ToInt32(MyQrCodeData.Ordinary);
                int? branch = string.IsNullOrEmpty(selectbranch) ? (int?)null : Convert.ToInt32(selectbranch);
                var data = await myqrCodeManage.GetMyQrCode(DataCashingAll.MerchantId, MyQrCodeData.MyQrCodeNo);
                if (data == null)
                {
                    return false;
                }
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
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    return false;
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
                var update = await myqrCodeManage.UpdateMyQrCode(localmyQrCode);
                MyQrCodeData = null;

                Toast.MakeText(this.Activity, GetString(Resource.String.savesucess), ToastLength.Short).Show();
                Setting_Fragment_MyQR.SetFocusQR(localmyQrCode.MyQrCodeNo);
                DataCashingAll.flagMyQrCodeChange = true;
                return true;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnEdit_Click at add Qr");
                return false;
            }
        }

        private async Task<bool> InsertMyQR()
        {
            try
            {
                if (!DataCashing.CheckNet)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    return false;
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
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotedit), ToastLength.Short).Show();
                    return false;
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
                var insert = await myqrCodeManage.InsertMyQrCode(localmyQrCode);
                if (!insert)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotinsert), ToastLength.Short).Show();
                    return false;
                }

                Toast.MakeText(this.Activity, GetString(Resource.String.insertsucess), ToastLength.Short).Show();
                Setting_Fragment_MyQR.SetFocusQR(localmyQrCode.MyQrCodeNo);
                DataCashingAll.flagMyQrCodeChange = true;
                return true;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return false;
            }
        }

        void CheckNull()
        {
            if (string.IsNullOrEmpty(QRCodeName.Text) == true | setBranch == '\0')
            {
                Toast.MakeText(this.Activity, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                return;
            }
        }

        async Task Showdata()
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
                    if (await GabanaAPI.CheckNetWork())
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Showdata at add Qr");
                return;
            }
        }

    }
}