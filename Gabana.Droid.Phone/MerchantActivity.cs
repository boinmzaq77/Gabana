using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common.Apis;
using Android.OS;
using Android.Provider;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ORM.Master;
using Gabana.ORM.MerchantDB;
using Gabana.ORM.PoolDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class MerchantActivity : AppCompatActivity
    {
        public static MerchantActivity merchant;
        ImageButton imgProfile, btnAddImage, btnShowDetail, btnprovinces, btnAmphures, btnDistrict;
        EditText txtMerchantName, txtTax, txtRegistrationNo, txtPhoneNumber, txtaddress, txtcomment, txtlinkpromaxx;
        TextView txtzipcode;
        private bool showdetail;
        FrameLayout lnShowDetail;
        LinearLayout lnDetails;
        internal Button btnSave;
        Gabana3.JAM.Merchant.Merchants MerchantDetail;
        ORM.MerchantDB.Merchant MerchantLocal;
        Spinner spinnerProvince, spinneramphures, spinnerdistrict;

        int ProvincesId, AmphuresId, DistrictsId;
        List<string> Provinces;
        List<string> Amphures;
        List<string> District;
        List<string> zipcode;
        PoolManage poolManage = new PoolManage();

        Android.Net.Uri keepCropedUri = null;
        Android.Graphics.Bitmap bitmap;
        Android.Net.Uri cameraTakePictureUri;
        int RESULT_OK = -1;
        int CAMERA_REQUEST = 0;
        int CROP_REQUEST = 1;
        int GALLERY_PICTURE = 2;

        ORM.MerchantDB.Branch BranchDetail;
        string usernamelogin, LoginType;
        ORM.Master.Branch UpdatetBranch;
        List<ORM.PoolDB.Province> GetProvinces = new List<ORM.PoolDB.Province>();
        List<ORM.PoolDB.Amphure> GetAmphures = new List<ORM.PoolDB.Amphure>();
        List<ORM.PoolDB.District> GetDistricts = new List<ORM.PoolDB.District>();
        string Phone = string.Empty;
        string Name = string.Empty;
        string TaxID = string.Empty;
        string RegMark = string.Empty;
        DialogLoading dialogLoading;
        byte[] imageByte;
        BranchManage branchManage = new BranchManage();
        MerchantManage merchantManage = new MerchantManage();
        bool first = true, flagdatachange = false;
        public static bool CurrentActivity = false;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.merchant_activity);

                merchant = this;

                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;
                imgProfile = FindViewById<ImageButton>(Resource.Id.imgProfile);
                btnAddImage = FindViewById<ImageButton>(Resource.Id.btnAddImage);
                btnShowDetail = FindViewById<ImageButton>(Resource.Id.btnShowDetail);
                txtMerchantName = FindViewById<EditText>(Resource.Id.txtMerchantName);
                txtMerchantName.TextChanged += TextChanged;
                lnDetails = FindViewById<LinearLayout>(Resource.Id.lnDetails);
                lnShowDetail = FindViewById<FrameLayout>(Resource.Id.lnShowDetail);                
                btnSave = FindViewById<Button>(Resource.Id.btnSave);

                btnprovinces = FindViewById<ImageButton>(Resource.Id.btnprovinces);
                btnAmphures = FindViewById<ImageButton>(Resource.Id.btnAmphures);
                btnDistrict = FindViewById<ImageButton>(Resource.Id.btnDistrict);
                spinnerProvince = FindViewById<Spinner>(Resource.Id.spinnerProvince);
                spinneramphures = FindViewById<Spinner>(Resource.Id.spinneramphures);
                spinnerdistrict = FindViewById<Spinner>(Resource.Id.spinnerdistrict);

                txtTax = FindViewById<EditText>(Resource.Id.txtTax);
                txtTax.TextChanged += TextChanged;
                txtRegistrationNo = FindViewById<EditText>(Resource.Id.txtRegistrationNo);
                txtRegistrationNo.TextChanged += TextChanged;
                txtPhoneNumber = FindViewById<EditText>(Resource.Id.txtPhoneNumber);
                txtaddress = FindViewById<EditText>(Resource.Id.txtaddress);
                txtaddress.TextChanged += TextChanged;
                txtcomment = FindViewById<EditText>(Resource.Id.txtcomment);
                txtcomment.TextChanged += TextChanged;
                txtlinkpromaxx = FindViewById<EditText>(Resource.Id.txtlinkpromaxx);
                txtlinkpromaxx.TextChanged += TextChanged;
                txtzipcode = FindViewById<TextView>(Resource.Id.txtzipcode);
                txtzipcode.TextChanged += TextChanged;

                txtPhoneNumber.TextChanged += TxtPhoneNumber_TextChanged;
                lnShowDetail.Click += LnShowDetail_Click;
                btnSave.Click += BtnSave_Click;
                btnprovinces.Click += Btnprovinces_Click;
                btnAmphures.Click += BtnAmphures_Click;
                btnDistrict.Click += BtnDistrict_Click;
                imgProfile.Click += ImgProfile_Click;
                btnAddImage.Click += ImgProfile_Click;
                showdetail = false;
                CurrentActivity = true;

                CheckPermission();

                CheckJwt();
                LoginType = Preferences.Get("LoginType", "");

                dialogLoading = new DialogLoading();
                dialogLoading.Cancelable = false;
                dialogLoading.Show(SupportFragmentManager, nameof(DialogLoading));

                Android.Content.Res.Resources res = this.Resources;
                string select = res.GetString(Resource.String.addcustomer_activity_selectzipcode);

                List<string> items = new List<string>();
                zipcode = new List<string>();
                string temp = "0";
                string temp2 = select;
                items.Add(temp2);
                zipcode.Add(temp);

                Provinces = new List<string>();
                Amphures = new List<string>();
                District = new List<string>();

                Phone = txtPhoneNumber.Text;
                SelectProvince();

                
                if (!await GabanaAPI.CheckNetWork())
                {
                    Toast.MakeText(this, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    MerchantLocal = DataCashingAll.MerchantLocal;
                }
                else  if (!await GabanaAPI.CheckSpeedConnection())
                {
                    Toast.MakeText(this, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                    MerchantLocal = DataCashingAll.MerchantLocal;
                }
                else
                {
                    if (string.IsNullOrEmpty(DataCashingAll.DevicePlatform))
                    {
                        DataCashingAll.DevicePlatform = "FCM";
                    }

                    if (string.IsNullOrEmpty(DataCashingAll.DeviceUDID))
                    {
                        DataCashingAll.DeviceUDID = Preferences.Get("DeviceUDID", "");
                    }
                    MerchantDetail = await GabanaAPI.GetMerchantDetail(DataCashingAll.DevicePlatform, DataCashingAll.DeviceUDID);

                    ORM.MerchantDB.Merchant Updatemerchant = new ORM.MerchantDB.Merchant()
                    {
                        MerchantID = MerchantDetail.Merchant.MerchantID,
                        Name = MerchantDetail.Merchant.Name,
                        FMasterMerchant = MerchantDetail.Merchant.FMasterMerchant, // ถ้า FMasterMerchant = 1 ค่าของ RefMasterMerchantID จะเป็นเลขเดียวกับ MerchantID 
                        RefMasterMerchantID = MerchantDetail.Merchant.MerchantID,//รอแก้ไขเรื่อง ถ้า FMasterMerchant = 0 ค่าของ RefMasterMerchantID จะเป็นค่าของ MerchantID ที่เป็น Master ที่ Merchant นี้เป็น Franchise อยู่
                        Status = MerchantDetail.Merchant.Status,
                        DateOpenMerchant = MerchantDetail.Merchant.DateOpenMerchant,
                        RefPackageID = MerchantDetail.Merchant.RefPackageID, //Reference ID ของ Package ไปยังระบบ MerchantLicence
                        DayOfPeriod = MerchantDetail.Merchant.DayOfPeriod,//วันที่ของรอบการคิดเงิน ของ Package เช่นทุกวันที่ 10 ของทุกเดือน
                        DueDate = MerchantDetail.Merchant.DueDate,
                        LanguageCountryCode = MerchantDetail.Merchant.LanguageCountryCode,//Default 'th-TH'
                        TimeZoneName = MerchantDetail.Merchant.TimeZoneName,
                        TimeZoneUTCOffset = MerchantDetail.Merchant.TimeZoneUTCOffset,
                        LogoPath = MerchantDetail.Merchant.LogoPath,
                        DateCreated = MerchantDetail.Merchant.DateCreated,
                        DateModified = MerchantDetail.Merchant.DateModified,
                        UserNameModified = MerchantDetail.Merchant.UserNameModified,
                        DateCloseMerchant = MerchantDetail.Merchant.DateCloseMerchant,
                        FPrivacyPolicy = MerchantDetail.Merchant.FPrivacyPolicy,
                        FTermConditions = MerchantDetail.Merchant.FTermConditions,
                        RegMark = MerchantDetail.Merchant.RegMark,
                        TaxID = MerchantDetail.Merchant.TaxID
                    };

                    var merchantlocal = await merchantManage.GetMerchant(DataCashingAll.MerchantId);
                    string logoPath = string.Empty;
                    string pathClound = MerchantDetail.Merchant.LogoPath;
                    logoPath = Utils.SplitCloundPath(merchantlocal?.LogoPath);

                    if ((logoPath?.ToString() != Utils.SplitCloundPath(MerchantDetail?.Merchant.LogoPath)) || (string.IsNullOrEmpty(merchantlocal?.LogoLocalPath)))
                    {
                        string path = await Utils.InsertLocalPictureMerchantMaster(pathClound);
                        Updatemerchant.LogoLocalPath = path;
                    }
                    else
                    {
                        Updatemerchant.LogoLocalPath = merchantlocal?.LogoLocalPath;
                    }
                    await merchantManage.UpdateMerchant(Updatemerchant);
                    DataCashingAll.Merchant.Merchant = MerchantDetail.Merchant;

                    var BranchDetail = MerchantDetail.Branch.Where(x => x.SysBranchID == 1).FirstOrDefault();
                    if (BranchDetail != null)
                    {
                        ORM.MerchantDB.Branch Update = new ORM.MerchantDB.Branch();
                        Update.MerchantID = BranchDetail.MerchantID;
                        Update.SysBranchID = int.Parse(BranchDetail.BranchID);
                        Update.Ordinary = BranchDetail.Ordinary;
                        Update.BranchName = BranchDetail.BranchName;
                        Update.BranchID = BranchDetail.BranchID;
                        Update.Address = BranchDetail.Address;
                        Update.ProvincesId = BranchDetail.ProvincesId;
                        Update.AmphuresId = BranchDetail.AmphuresId;
                        Update.DistrictsId = BranchDetail.DistrictsId;
                        Update.Status = BranchDetail.Status;
                        Update.DisplayLanguage = BranchDetail.DisplayLanguage;
                        Update.Lat = BranchDetail.Lat;
                        Update.Lng = BranchDetail.Lng;
                        Update.Email = BranchDetail.Email;
                        Update.Tel = BranchDetail.Tel;
                        Update.Line = BranchDetail.Line;
                        Update.Facebook = BranchDetail.Facebook;
                        Update.Instagram = BranchDetail.Instagram;
                        Update.TaxBranchName = BranchDetail.TaxBranchName;
                        Update.TaxBranchID = BranchDetail.TaxBranchID;
                        Update.LinkProMaxxID = BranchDetail.LinkProMaxxID;
                        Update.Comments = BranchDetail.Comments;
                        await branchManage.InsertorReplacrBranch(Update);
                    }
                }

                TokenResult ressult = await TokenServiceBase.GetToken();
                if (!ressult.status)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                    this.Finish();
                    return;
                }

                SetShowdetail();
                ShowBranchDetail();
                await ShowDetailMerchant();

                SetButtonAdd(false);
                SetUIFromMainRole(LoginType);
                first = false;
                dialogLoading.Dismiss();

                _ = TinyInsights.TrackPageViewAsync("OnCreate : MerchantActivity");

            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                _ = TinyInsights.TrackPageViewAsync("OnCreate at Merchant");
                _ = TinyInsights.TrackErrorAsync(ex);
                return;
            }
        }
        private void SetUIFromMainRole(string typeLogin)
        {
            bool check = UtilsAll.CheckPermissionRoleUser(typeLogin, "insert", "merchant");
            //case enable
            if (check)
            {
                imgProfile.Enabled = true;
                btnAddImage.Enabled = true;
                btnAddImage.Visibility = ViewStates.Visible;
                txtMerchantName.Enabled = true;
                txtMerchantName.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtMerchantName.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                txtTax.Enabled = true;
                txtTax.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtTax.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                txtRegistrationNo.Enabled = true;
                txtRegistrationNo.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtRegistrationNo.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                txtPhoneNumber.Enabled = true;
                txtPhoneNumber.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtPhoneNumber.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                txtaddress.Enabled = true;
                txtaddress.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtaddress.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                spinnerProvince.Enabled = true;
                btnprovinces.Enabled = true;
                spinneramphures.Enabled = true;
                btnAmphures.Enabled = true;
                spinnerdistrict.Enabled = true;
                btnDistrict.Enabled = true;
                txtzipcode.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtcomment.Enabled = true;
                txtcomment.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtcomment.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                btnSave.Enabled = true;
                btnSave.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnSave.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtlinkpromaxx.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtlinkpromaxx.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                btnprovinces.SetBackgroundResource(Resource.Mipmap.Next);
                btnAmphures.SetBackgroundResource(Resource.Mipmap.Next);
                btnDistrict.SetBackgroundResource(Resource.Mipmap.Next);
            }
            else
            {
                imgProfile.Enabled = false;
                btnAddImage.Enabled = false;
                btnAddImage.Visibility = ViewStates.Invisible;
                txtMerchantName.Enabled = false;
                txtMerchantName.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtMerchantName.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtTax.Enabled = false;
                txtTax.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtTax.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtRegistrationNo.Enabled = false;
                txtRegistrationNo.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtRegistrationNo.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtPhoneNumber.Enabled = false;
                txtPhoneNumber.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtPhoneNumber.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtaddress.Enabled = false;
                txtaddress.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtaddress.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                spinnerProvince.Enabled = false;
                btnprovinces.Enabled = false;
                spinneramphures.Enabled = false;
                btnAmphures.Enabled = false;
                spinnerdistrict.Enabled = false;
                btnDistrict.Enabled = false;
                txtzipcode.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtzipcode.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtcomment.Enabled = false;
                txtcomment.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtcomment.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                btnSave.Enabled = false;
                btnSave.SetBackgroundResource(Resource.Drawable.btnbordergray);
                btnSave.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtlinkpromaxx.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtlinkpromaxx.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                btnprovinces.SetBackgroundResource(Resource.Mipmap.NextG);
                btnAmphures.SetBackgroundResource(Resource.Mipmap.NextG);
                btnDistrict.SetBackgroundResource(Resource.Mipmap.NextG);
            }
        }

        private void TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
        }

        private void SetButtonAdd(bool enable)
        {
            if (enable)
            {
                btnSave.SetBackgroundResource(Resource.Drawable.btnblue);
                btnSave.SetTextColor(Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnSave.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnSave.SetTextColor(Resources.GetColor(Resource.Color.editbluecolor, null));
            }
            btnSave.Enabled = enable;
        }
        private void CheckDataChange()
        {
            try
            {
                if (first)
                {
                    SetButtonAdd(false);
                    flagdatachange = false;
                    return;
                }
                var showName = Name == null ? string.Empty : Name;
                if (txtMerchantName.Text != showName)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                var showTaxID = TaxID == null ? string.Empty : TaxID;
                if (txtTax.Text != showTaxID)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                var showRegMark = TaxID == null ? string.Empty : RegMark;
                if (txtRegistrationNo.Text != showRegMark)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                if (txtPhoneNumber.Text != addTextTel(BranchDetail.Tel))
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                var address = BranchDetail.Address == null ? string.Empty : BranchDetail.Address;
                if (txtaddress.Text != address)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                var Comments = BranchDetail.Comments == null ? string.Empty : BranchDetail.Comments;
                if (txtcomment.Text != Comments)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                var BProvincesId = BranchDetail.ProvincesId == null ? 0 : BranchDetail.ProvincesId;
                if (ProvincesId != BProvincesId)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                var BAmphuresId = BranchDetail.AmphuresId == null ? 0 : BranchDetail.AmphuresId;
                if (AmphuresId != BAmphuresId)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                var BDistrictsId = BranchDetail.DistrictsId == null ? 0 : BranchDetail.DistrictsId;
                if (DistrictsId != BDistrictsId)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                if (keepCropedUri != null)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                SetButtonAdd(false);
                SetUIFromMainRole(LoginType);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackPageViewAsync("CheckDataChange at Merchant");
                _ = TinyInsights.TrackErrorAsync(ex);
                return;
            }
        }
        private void TxtPhoneNumber_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                Phone = txtPhoneNumber.Text;
                int textlength = txtPhoneNumber.Text.Length;

                if (Phone.EndsWith(" "))
                    return;

                if (Phone.StartsWith("02"))
                {
                    if (Phone.Length == 12)
                    {
                        Phone = Phone.Remove(Phone.Length - 1);
                        txtPhoneNumber.Text = Phone;
                        txtPhoneNumber.SetSelection(txtPhoneNumber.Text.Length);
                    }
                    if (textlength == 3)
                    {
                        var index = txtPhoneNumber.Text.LastIndexOf("-");
                        if (textlength == 3 & index == 2)
                        {
                            Phone.Remove(2, 1);
                        }
                        else
                        {
                            txtPhoneNumber.Text = Phone.Insert(Phone.Length - 1, "-").ToString();
                            txtPhoneNumber.SetSelection(txtPhoneNumber.Text.Length);
                        }
                    }
                    else if (textlength == 7)
                    {
                        var index = txtPhoneNumber.Text.LastIndexOf("-");
                        if (textlength == 7 & index == 6)
                        {
                            Phone.Remove(6, 1);
                        }
                        else
                        {
                            txtPhoneNumber.Text = Phone.Insert(Phone.Length - 1, "-").ToString();
                            txtPhoneNumber.SetSelection(txtPhoneNumber.Text.Length);
                        }
                    }
                }
                else if (Phone.StartsWith("03") | Phone.StartsWith("04") | Phone.StartsWith("05") | Phone.StartsWith("07"))
                {
                    if (Phone.Length == 12)
                    {
                        Phone = Phone.Remove(Phone.Length - 1);
                        txtPhoneNumber.Text = Phone;
                        txtPhoneNumber.SetSelection(txtPhoneNumber.Text.Length);
                    }
                    if (textlength == 4)
                    {
                        var index = txtPhoneNumber.Text.LastIndexOf("-");
                        if (textlength == 4 & index == 3)
                        {
                            Phone.Remove(3, 1);
                        }
                        else
                        {
                            txtPhoneNumber.Text = Phone.Insert(Phone.Length - 1, "-").ToString();
                            txtPhoneNumber.SetSelection(txtPhoneNumber.Text.Length);
                        }
                    }
                    else if (textlength == 8)
                    {
                        var index = txtPhoneNumber.Text.LastIndexOf("-");
                        if (textlength == 8 & index == 7)
                        {
                            Phone.Remove(7, 1);
                        }
                        else
                        {
                            txtPhoneNumber.Text = Phone.Insert(Phone.Length - 1, "-").ToString();
                            txtPhoneNumber.SetSelection(txtPhoneNumber.Text.Length);
                        }
                    }
                }
                else
                {
                    if (textlength == 4)
                    {
                        var index = txtPhoneNumber.Text.LastIndexOf("-");
                        if (textlength == 4 & index == 3)
                        {
                            Phone.Remove(3, 1);
                        }
                        else
                        {
                            txtPhoneNumber.Text = Phone.Insert(Phone.Length - 1, "-").ToString();
                            txtPhoneNumber.SetSelection(txtPhoneNumber.Text.Length);
                        }
                    }
                    else if (textlength == 8)
                    {
                        var index = txtPhoneNumber.Text.LastIndexOf("-");
                        if (textlength == 8 & index == 7)
                        {
                            Phone.Remove(7, 1);
                        }
                        else
                        {
                            txtPhoneNumber.Text = Phone.Insert(Phone.Length - 1, "-").ToString();
                            txtPhoneNumber.SetSelection(txtPhoneNumber.Text.Length);
                        }
                    }
                }
                CheckDataChange();
            }
            catch (Exception)
            {
                return;
            }
        }

        string addTextTel(string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    return string.Empty;
                }

                if (value.Length == 9 & value.StartsWith("02"))
                {
                    var Phone = string.Empty;
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (i == 2 | i == 5)
                        {
                            Phone += "-";
                        }
                        Phone += value[i];
                    }
                    return Phone;
                }

                if (value.Length == 9 & value.StartsWith("03") | value.StartsWith("04") | value.StartsWith("05") | value.StartsWith("07"))
                {
                    var Phone = string.Empty;
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (i == 3 | i == 6)
                        {
                            Phone += "-";
                        }
                        Phone += value[i];
                    }
                    return Phone;
                }

                if (value.Length == 10)
                {
                    var Phone = string.Empty;
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (i == 3 | i == 6)
                        {
                            Phone += "-";
                        }
                        Phone += value[i];
                    }
                    return Phone;
                }
                return value;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("addTextTel at Merchant");
                return value;
            }
        }

        private void ImgProfile_Click(object sender, EventArgs e)
        {
            var fragment = new UpdateMerchant_Dialog_AddImage();
            fragment.Show(SupportFragmentManager, nameof(UpdateMerchant_Dialog_AddImage));
        }

        private void BtnDistrict_Click(object sender, EventArgs e)
        {
            spinnerdistrict.PerformClick();
        }

        private void BtnAmphures_Click(object sender, EventArgs e)
        {
            spinneramphures.PerformClick();
        }

        private void Btnprovinces_Click(object sender, EventArgs e)
        {
            spinnerProvince.PerformClick();
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (!await GabanaAPI.CheckNetWork())
            {
                Toast.MakeText(this, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                return;
            }

            if (!await GabanaAPI.CheckSpeedConnection())
            {
                Toast.MakeText(this, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                return;
            }

            EditMerchantConfig();
        }

        private void LnShowDetail_Click(object sender, EventArgs e)
        {
            if (showdetail)
            {
                showdetail = false;
            }
            else
            {
                showdetail = true;
            }
            SetShowdetail();

            SetUIFromMainRole(LoginType);

        }

        private void ShowMerchantDetail()
        {
            try
            {
                Gabana.ORM.Master.Merchant merchants = new ORM.Master.Merchant();
                if (DataCashingAll.Merchant != null)
                {
                    //online                
                    merchants = DataCashingAll.Merchant.Merchant;
                    Name = merchants.Name;
                    TaxID = merchants.TaxID;
                    RegMark = merchants.RegMark;
                }
                else
                {
                    //offline
                    MerchantLocal = DataCashingAll.MerchantLocal;
                    Name = MerchantLocal.Name;
                    TaxID = MerchantLocal.TaxID;
                    RegMark = MerchantLocal.RegMark;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void SetShowdetail()
        {
            if (showdetail)
            {
                lnDetails.Visibility = ViewStates.Visible;
                btnShowDetail.SetBackgroundResource(Resource.Mipmap.DetailShow);
            }
            else
            {
                lnDetails.Visibility = ViewStates.Gone;
                btnShowDetail.SetBackgroundResource(Resource.Mipmap.DetailNotShow);
            }
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        public void DialogCheckBack()
        {
            base.OnBackPressed();
            flagdatachange = false;
            CurrentActivity = false;
        }

        public override void OnBackPressed()
        {
            try
            {
                if (!flagdatachange)
                {
                    DialogCheckBack(); return;
                }

                MainDialog dialog = new MainDialog();
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.edit_dialog_back.ToString();
                bundle.PutString("message", myMessage);
                bundle.PutString("fromPage", "merchant");
                dialog.Arguments = bundle;
                dialog.Show(SupportFragmentManager, myMessage);
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        protected override void OnResume()
        {
            base.OnResume();
            CheckJwt();
        }

        async void ShowBranchDetail()
        {
            try
            {                
                BranchDetail = await branchManage.GetBranch(DataCashingAll.MerchantId, 1); //สาขาสำนักงานใหญ่
                //BranchDetail = DataCashing.branchDeatail; //สาขาที่เลือกจาก select branch

                if (BranchDetail != null)
                {
                    if (!string.IsNullOrEmpty(BranchDetail.Tel))
                    {
                        txtPhoneNumber.Text = addTextTel(BranchDetail.Tel);
                    }

                    txtaddress.Text = BranchDetail.Address;
                    if (BranchDetail.ProvincesId == null)
                    {
                        ProvincesId = 0;
                    }
                    else
                    {
                        ProvincesId = (int)BranchDetail.ProvincesId;
                    }
                    if (BranchDetail.AmphuresId == null)
                    {
                        AmphuresId = 0;
                    }
                    else
                    {
                        AmphuresId = (int)BranchDetail.AmphuresId;
                    }
                    if (BranchDetail.DistrictsId == null)
                    {
                        DistrictsId = 0;
                    }
                    else
                    {
                        DistrictsId = (int)BranchDetail.DistrictsId;
                    }
                    txtcomment.Text = BranchDetail.Comments;
                    txtlinkpromaxx.Text = BranchDetail.LinkProMaxxID;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowBranchDetail at Merchant");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private void spinnerProvince_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            try
            {
                string select = spinnerProvince.SelectedItem.ToString();
                if (DataCashing.Provinces == null)
                {
                    return;
                }

                //ProvincesId = int.Parse(DataCashing.Provinces[e.Position].ToString());                       

                if (BranchDetail?.ProvincesId == ProvincesId)
                {
                    ProvincesId = (int)GetProvinces.Find(x => x.ProvincesId == ProvincesId).ProvincesId;
                }
                else
                {
                    ProvincesId = (int)GetProvinces[e.Position].ProvincesId;
                }

                if (select != "จังหวัด")
                {
                    ProvincesId = (int)GetProvinces.Find(x => x.ProvincesNameTH == select).ProvincesId;
                    if (ProvincesId != BranchDetail?.ProvincesId)
                    {
                        AmphuresId = 0;
                        DistrictsId = 0;
                    }
                }

                int position = GetProvinces.FindIndex(x => x.ProvincesId == ProvincesId);
                spinnerProvince.SetSelection(position);
                SelectAmphures();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private void spinneramphures_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            try
            {
                string select = spinneramphures.SelectedItem.ToString();
                if (DataCashing.Amphures == null)
                {
                    return;
                }

                //AmphuresId = int.Parse(DataCashing.Amphures[e.Position]);

                if (BranchDetail?.AmphuresId == AmphuresId)
                {
                    AmphuresId = (int)GetAmphures.Find(x => x.AmphuresId == AmphuresId).AmphuresId;
                }
                else
                {
                    AmphuresId = (int)GetAmphures[e.Position].AmphuresId;
                }

                if (select != "อำเภอ")
                {
                    AmphuresId = (int)GetAmphures.Find(x => x.AmphuresNameTH == select).AmphuresId;
                    if (AmphuresId != BranchDetail?.AmphuresId)
                    {
                        DistrictsId = 0;
                    }
                }

                int position = GetAmphures.FindIndex(x => x.AmphuresId == AmphuresId);
                spinneramphures.SetSelection(position);
                SelectDistrict();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private async void spinnerdistrict_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            try
            {
                string select = spinnerdistrict.SelectedItem.ToString();
                if (DataCashing.Districts == null)
                {
                    return;
                }
                //DistrictsId = int.Parse(DataCashing.Districts[e.Position]);
                if (BranchDetail?.DistrictsId == DistrictsId)
                {
                    DistrictsId = (int)GetDistricts.Find(x => x.DistrictsId == DistrictsId).DistrictsId;
                }
                else
                {
                    DistrictsId = (int)GetDistricts[e.Position].DistrictsId;
                }


                if (select != "ตำบล")
                {
                    DistrictsId = (int)GetDistricts.Find(x => x.DistrictsNameTH == select).DistrictsId;
                }

                int position = GetDistricts.FindIndex(x => x.DistrictsId == DistrictsId);
                spinnerdistrict.SetSelection(position);

                PoolManage poolManage = new PoolManage();
                var lstzipcode = new ORM.PoolDB.District();
                lstzipcode = await poolManage.GetZipcode(AmphuresId, DistrictsId);
                if (lstzipcode != null)
                {
                    if (lstzipcode.ZipCode != null)
                    {
                        txtzipcode.Text = lstzipcode.ZipCode;
                    }
                }                
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        #region Spinner
        public async void SelectProvince()
        {
            try
            {
                string temp = "";
                string temp2 = "";
                List<string> items = new List<string>();
                ORM.PoolDB.Province province = new ORM.PoolDB.Province();
                var lstprovince = new List<ORM.PoolDB.Province>();
                var getallprovince = new List<ORM.PoolDB.Province>();

                Android.Content.Res.Resources res = this.Resources;
                string select = res.GetString(Resource.String.addcustomer_activity_selectprovince);

                province = new ORM.PoolDB.Province()
                {
                    ProvincesId = 0,
                    ProvincesNameEN = "Province",
                    ProvincesNameTH = select
                };
                lstprovince.Add(province);
                getallprovince = await poolManage.GetProvinces();
                lstprovince.AddRange(getallprovince);
                GetProvinces = lstprovince;

                for (int i = 0; i < lstprovince.Count; i++)
                {
                    temp = lstprovince[i].ProvincesId.ToString();
                    temp2 = lstprovince[i].ProvincesNameTH.ToString();
                    items.Add(temp2);
                    Provinces.Add(temp);
                }

                DataCashing.Provinces = Provinces;

                //Province
                spinnerProvince.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinnerProvince_ItemSelected);
                var adapterspinnerProvince = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, items);
                adapterspinnerProvince.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinnerProvince.Adapter = adapterspinnerProvince;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SelectProvince at Merchant");
                Log.Debug("error", ex.Message);
            }
        }

        public async void SelectAmphures()
        {
            try
            {
                string temp = "";
                string temp2 = "";
                List<string> items = new List<string>();
                Amphures = new List<string>();
                ORM.PoolDB.Amphure amphure = new ORM.PoolDB.Amphure();
                var lstamphures = new List<ORM.PoolDB.Amphure>();
                var getallamphures = new List<ORM.PoolDB.Amphure>();

                Android.Content.Res.Resources res = this.Resources;
                string select = res.GetString(Resource.String.addcustomer_activity_selectdistrict);

                amphure = new ORM.PoolDB.Amphure()
                {
                    AmphuresId = 0,
                    AmphuresNameEN = "Amphure",
                    AmphuresNameTH = select
                };
                lstamphures.Add(amphure);
                getallamphures = await poolManage.GetAmphures(ProvincesId);
                lstamphures.AddRange(getallamphures);
                GetAmphures = lstamphures;

                for (int i = 0; i < lstamphures.Count; i++)
                {
                    temp = lstamphures[i].AmphuresId.ToString();
                    temp2 = lstamphures[i].AmphuresNameTH.ToString();
                    items.Add(temp2);
                    Amphures.Add(temp);
                }

                DataCashing.Amphures = Amphures;
                //Amphures
                spinneramphures.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinneramphures_ItemSelected);
                var adapterspinneramphures = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, items);
                adapterspinneramphures.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinneramphures.Adapter = adapterspinneramphures;

                int position = GetAmphures.FindIndex(x => x.AmphuresId == AmphuresId);
                spinneramphures.SetSelection(position);
                SelectDistrict();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SelectAmphures at Merchant");
                Log.Debug("error", ex.Message);
            }

        }

        public async void SelectDistrict()
        {
            try
            {
                string temp = "";
                string temp2 = "";
                List<string> items = new List<string>();
                District = new List<string>();
                ORM.PoolDB.District district = new ORM.PoolDB.District();
                var lstdistrict = new List<ORM.PoolDB.District>();
                var getalldistrict = new List<ORM.PoolDB.District>();

                Android.Content.Res.Resources res = this.Resources;
                string select = res.GetString(Resource.String.addcustomer_activity_selectsubdistrict);

                district = new ORM.PoolDB.District()
                {
                    DistrictsId = 0,
                    DistrictsNameEN = "District",
                    DistrictsNameTH = select
                };
                lstdistrict.Add(district);
                getalldistrict = await poolManage.GetDistricts(AmphuresId);
                lstdistrict.AddRange(getalldistrict);
                GetDistricts = lstdistrict;

                for (int i = 0; i < lstdistrict.Count; i++)
                {
                    temp = lstdistrict[i].DistrictsId.ToString();
                    temp2 = lstdistrict[i].DistrictsNameTH.ToString();
                    items.Add(temp2);
                    District.Add(temp);
                }

                DataCashing.Districts = District;
                //subdistrict
                spinnerdistrict.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinnerdistrict_ItemSelected);
                var adapterspinnerdistrict = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, items);
                adapterspinnerdistrict.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinnerdistrict.Adapter = adapterspinnerdistrict;

                int position = GetDistricts.FindIndex(x => x.DistrictsId == DistrictsId);
                spinnerdistrict.SetSelection(position);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SelectDistrict at Merchant");
                Log.Debug("error", ex.Message);
            }
        }
        #endregion

        async Task ShowDetailMerchant()
        {
            try
            {
                #region Local Picture               
                var merchant = await merchantManage.GetMerchant(DataCashingAll.MerchantId);
                var cloudpath = merchant.LogoPath == null ? string.Empty : merchant.LogoPath;
                var localpath = merchant.LogoLocalPath == null ? string.Empty : merchant.LogoLocalPath;

                if (await GabanaAPI.CheckSpeedConnection())
                {
                    if (string.IsNullOrEmpty(localpath))
                    {
                        if (string.IsNullOrEmpty(cloudpath))
                        {
                            //defalut
                            imgProfile.SetBackgroundResource(Resource.Mipmap.LogoDefault);
                        }
                        else
                        {
                            //cloud
                            Utils.SetImage(imgProfile, cloudpath);
                        }
                    }
                    else
                    {
                        //local                        
                        Android.Net.Uri uri = Android.Net.Uri.Parse(localpath);
                        imgProfile.SetImageURI(uri);
                    }

                    //online
                    Gabana.ORM.Master.Merchant merchantsOnline = new ORM.Master.Merchant();
                    merchantsOnline = DataCashingAll.Merchant.Merchant;
                    Name = merchantsOnline.Name;
                    TaxID = merchantsOnline.TaxID;
                    RegMark = merchantsOnline.RegMark;
                }
                else
                {
                    if (!string.IsNullOrEmpty(localpath))
                    {
                        Android.Net.Uri uri = Android.Net.Uri.Parse(localpath);
                        imgProfile.SetImageURI(uri);
                    }
                    else
                    {
                        imgProfile.SetBackgroundResource(Resource.Mipmap.LogoDefault);
                    }

                    //offline
                    MerchantLocal = DataCashingAll.MerchantLocal;
                    Name = MerchantLocal.Name;
                    TaxID = MerchantLocal.TaxID;
                    RegMark = MerchantLocal.RegMark;
                }
                #endregion

                Gabana.ORM.Master.Merchant merchants = new ORM.Master.Merchant();
                if (DataCashingAll.Merchant != null)
                {
                    //online                
                    merchants = DataCashingAll.Merchant.Merchant;
                    Name = merchants.Name;
                    TaxID = merchants.TaxID;
                    RegMark = merchants.RegMark;
                }
                else
                {
                    //offline
                    MerchantLocal = DataCashingAll.MerchantLocal;
                    Name = MerchantLocal.Name;
                    TaxID = MerchantLocal.TaxID;
                    RegMark = MerchantLocal.RegMark;
                }
                txtMerchantName.Text = Name;
                txtTax.Text = TaxID;
                txtRegistrationNo.Text = RegMark;
                txtlinkpromaxx.Text = BranchDetail.LinkProMaxxID;
                txtcomment.Text = BranchDetail.Comments;


                #region Set Select Spinner
                //Set Select Spinner
                //Province
                List<string> items = new List<string>();
                Provinces = new List<string>();
                ORM.PoolDB.Province province = new ORM.PoolDB.Province();
                var lstprovince = new List<ORM.PoolDB.Province>();
                var getallprovince = new List<ORM.PoolDB.Province>();

                Android.Content.Res.Resources res = this.Resources;
                string select = res.GetString(Resource.String.addcustomer_activity_selectprovince);

                province = new ORM.PoolDB.Province()
                {
                    ProvincesId = 0,
                    ProvincesNameEN = "Province",
                    ProvincesNameTH = select
                };
                lstprovince.Add(province);
                getallprovince = await poolManage.GetProvinces();
                lstprovince.AddRange(getallprovince);

                string[] province_array = lstprovince.Select(i => i.ProvincesNameTH.ToString()).ToArray();
                var adapterProvince = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, province_array);
                adapterProvince.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinnerProvince.Adapter = adapterProvince;

                long? provinceid = BranchDetail.ProvincesId;
                if (provinceid != null)
                {
                    var data = getallprovince.Where(x => x.ProvincesId == provinceid).FirstOrDefault();
                    if (data != null)
                    {
                        int position;
                        //if (Language == "th")
                        //{
                        //position = adapterProvince.GetPosition(data.ProvincesNameTH);
                        //}
                        //else
                        //{
                        //position = adapterProvince.GetPosition(data.ProvincesNameEN);
                        //}  
                        position = adapterProvince.GetPosition(data.ProvincesNameTH);
                        spinnerProvince.SetSelection(position);
                        ProvincesId = provinceid == 0 ? 0 : (int)provinceid;
                    }
                }
                else
                {
                    int position = adapterProvince.GetPosition(select);
                    spinnerProvince.SetSelection(position);
                    SelectProvince();
                }

                if (provinceid == null)
                {
                    provinceid = 0;
                }

                //Amphures                
                Amphures = new List<string>();
                ORM.PoolDB.Amphure amphure = new ORM.PoolDB.Amphure();
                var lstamphures = new List<ORM.PoolDB.Amphure>();
                var getallamphures = new List<ORM.PoolDB.Amphure>();

                Android.Content.Res.Resources resamphur = this.Resources;
                string selectamphure = resamphur.GetString(Resource.String.addcustomer_activity_selectdistrict);

                amphure = new ORM.PoolDB.Amphure()
                {
                    AmphuresId = 0,
                    AmphuresNameEN = "Amphure",
                    AmphuresNameTH = selectamphure
                };
                lstamphures.Add(amphure);
                getallamphures = await poolManage.GetAmphures((int)provinceid);
                lstamphures.AddRange(getallamphures);

                string[] amphure_array = lstamphures.Select(i => i.AmphuresNameTH.ToString()).ToArray();
                var adapterAmphure = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, amphure_array);
                adapterAmphure.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinneramphures.Adapter = adapterAmphure;

                long? amphureID = BranchDetail.AmphuresId;

                if (amphureID != null)
                {
                    var data = getallamphures.Where(x => x.AmphuresId == amphureID).FirstOrDefault();
                    if (data != null)
                    {
                        int position;
                        position = adapterAmphure.GetPosition(data.AmphuresNameTH);
                        spinneramphures.SetSelection(position);
                        AmphuresId = amphureID == 0 ? 0 : (int)amphureID;
                    }
                }
                else
                {
                    int position = adapterAmphure.GetPosition(selectamphure);
                    spinneramphures.SetSelection(position);
                }

                if (amphureID == null)
                {
                    amphureID = 0;
                }

                //District
                District = new List<string>();
                ORM.PoolDB.District district = new ORM.PoolDB.District();
                var lstdistrict = new List<ORM.PoolDB.District>();
                var getalldistrict = new List<ORM.PoolDB.District>();

                Android.Content.Res.Resources resdistrict = this.Resources;
                string selectdistrict = resdistrict.GetString(Resource.String.addcustomer_activity_selectsubdistrict);

                district = new ORM.PoolDB.District()
                {
                    DistrictsId = 0,
                    DistrictsNameEN = "District",
                    DistrictsNameTH = selectdistrict
                };
                lstdistrict.Add(district);
                getalldistrict = await poolManage.GetDistricts((int)amphureID);
                lstdistrict.AddRange(getalldistrict);


                string[] district_array = lstdistrict.Select(i => i.DistrictsNameTH.ToString()).ToArray();
                var adapterDistrict = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, district_array);
                adapterDistrict.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinnerdistrict.Adapter = adapterDistrict;

                long? districtID = BranchDetail.DistrictsId;

                if (districtID != null)
                {
                    var data = getalldistrict.Where(x => x.DistrictsId == districtID).FirstOrDefault();
                    if (data != null)
                    {
                        int position;
                        position = adapterDistrict.GetPosition(data.DistrictsNameTH);
                        spinnerdistrict.SetSelection(position);
                        DistrictsId = districtID == 0 ? 0 : (int)districtID;
                    }
                }
                else
                {
                    int position = adapterDistrict.GetPosition(selectdistrict);
                    spinnerdistrict.SetSelection(position);
                }

                if (districtID == null)
                {
                    districtID = 0;
                }

                var lstzipcode = new ORM.PoolDB.District();
                if (amphureID == 0 & districtID == 0)
                {
                    txtzipcode.Text = GetString(Resource.String.addcustomer_activity_selectzipcode);
                }
                else
                {
                    lstzipcode = await poolManage.GetZipcode((int)amphureID, (int)districtID);
                    if (lstzipcode.ZipCode != null)
                    {
                        txtzipcode.Text = lstzipcode.ZipCode;
                    }
                }

                first = true;
                flagdatachange = false;
                #endregion
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("UpdateDetailCustomer at Merchant");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        #region Picture
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
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("CropImage at add merchant");
                Toast.MakeText(this, "error : " + ex.Message, ToastLength.Short).Show(); return;
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
                _ = TinyInsights.TrackPageViewAsync("Tack Pic at add merchant");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
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

                        imgProfile.SetImageBitmap(bitmap);
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
        #endregion

        async void EditMerchantConfig()
        {
            try
            {
                usernamelogin = Preferences.Get("User", "");

                Gabana3.JAM.Merchant.Merchants PutMerchant = new Gabana3.JAM.Merchant.Merchants();
                string logolocalPath = string.Empty;
                string logoPath = string.Empty;

                //Merchant
                Gabana.ORM.Master.Merchant merchants = new ORM.Master.Merchant();
                merchants = DataCashingAll.Merchant.Merchant;

                if (keepCropedUri != null)
                {
                    imageByte = await Utils.streamImage(bitmap);
                    merchants.LogoPath = null;
                }
                else
                {
                    var dataGetMerchant = await merchantManage.GetMerchant(DataCashingAll.MerchantId);
                    if (dataGetMerchant != null)
                    {
                        logolocalPath = dataGetMerchant?.LogoLocalPath;
                        logoPath = Utils.SplitCloundPath(dataGetMerchant?.LogoPath);
                    }
                }

                if (!string.IsNullOrEmpty(txtMerchantName.Text))
                {
                    merchants.Name = txtMerchantName.Text;
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                    return;
                }
                merchants.TaxID = txtTax.Text.Trim();
                merchants.RegMark = txtRegistrationNo.Text.Trim();

                //Update Branch
                var resultBranch = await UpdatetBranchDetail();
                if (!resultBranch)
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                    return;
                }

                PutMerchant.Merchant = merchants;
                var updatemerchant = await GabanaAPI.PutMerchant(PutMerchant, imageByte);
                if (!updatemerchant.Status)
                {

                    Toast.MakeText(this, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                    return;
                }

                Toast.MakeText(this, GetString(Resource.String.savesucess), ToastLength.Short).Show();
                Gabana3.JAM.Merchant.Merchants data = new Gabana3.JAM.Merchant.Merchants();
                data = await GabanaAPI.GetMerchantDetail(DataCashingAll.DevicePlatform, DataCashingAll.DeviceUDID);

                if (logoPath?.ToString() != Utils.SplitCloundPath(data?.Merchant.LogoPath))
                {
                    merchants.LogoPath = data?.Merchant.LogoPath;
                    var merchantlocal = await merchantManage.GetMerchant(merchants.MerchantID);
                    merchantlocal.LogoPath = data?.Merchant.LogoPath;
                    await merchantManage.UpdateMerchant(merchantlocal);
                    await Utils.InsertLocalPictureMerchant(merchantlocal);
                    merchantlocal = await merchantManage.GetMerchant(merchants.MerchantID);
                    logolocalPath = merchantlocal.LogoLocalPath;
                }

                DataCashingAll.Merchant.Merchant = merchants;

                //insert to local
                //merchant
                ORM.MerchantDB.Merchant merchant = new ORM.MerchantDB.Merchant()
                {
                    MerchantID = merchants.MerchantID,
                    Name = merchants.Name,
                    FMasterMerchant = merchants.FMasterMerchant,
                    RefMasterMerchantID = merchants.MerchantID,
                    Status = merchants.Status,
                    DateOpenMerchant = merchants.DateOpenMerchant,
                    RefPackageID = merchants.RefPackageID,
                    DayOfPeriod = merchants.DayOfPeriod,
                    DueDate = merchants.DueDate,
                    LanguageCountryCode = merchants.LanguageCountryCode,
                    TimeZoneName = merchants.TimeZoneName,
                    TimeZoneUTCOffset = merchants.TimeZoneUTCOffset,
                    LogoPath = data?.Merchant.LogoPath,
                    DateCreated = Utils.GetTranDate(merchants.DateCreated),
                    DateModified = Utils.GetTranDate(merchants.DateModified),
                    UserNameModified = merchants.UserNameModified,
                    DateCloseMerchant = merchants.DateCloseMerchant,
                    FPrivacyPolicy = merchants.FPrivacyPolicy,
                    FTermConditions = merchants.FTermConditions,
                    LogoLocalPath = keepCropedUri == null ? logolocalPath : keepCropedUri.ToString(),
                    RegMark = merchants.RegMark,
                    TaxID = merchants.TaxID
                };
                var result = await merchantManage.UpdateMerchant(merchant);
                this.Finish();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("EditMerchantConfig at Merchant");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        async Task<bool> UpdatetBranchDetail()
        {
            try
            {
                string phone = string.Empty;
                if (string.IsNullOrEmpty(txtPhoneNumber.Text))
                {
                    phone = null;
                }
                else
                {
                    if (txtPhoneNumber.Text.StartsWith("02") | txtPhoneNumber.Text.StartsWith("03") | txtPhoneNumber.Text.StartsWith("04") | txtPhoneNumber.Text.StartsWith("05") | txtPhoneNumber.Text.StartsWith("07"))
                    {
                        if (txtPhoneNumber.Text.Replace("-", "").Length < 9)
                        {
                            Toast.MakeText(this, GetString(Resource.String.telnotcomplete), ToastLength.Short).Show();
                            return false;
                        }
                        else
                        {
                            phone = txtPhoneNumber.Text.Replace("-", "");
                        }
                    }
                    else
                    {
                        if (txtPhoneNumber.Text.Replace("-", "").Length < 10)
                        {
                            Toast.MakeText(this, GetString(Resource.String.telnotcomplete), ToastLength.Short).Show();
                            return false;
                        }
                        else
                        {
                            phone = txtPhoneNumber.Text.Replace("-", "");
                        }
                    }
                }

                string address = txtaddress.Text;
                string comment = txtcomment.Text.Trim();
                string linkPromaxx = txtlinkpromaxx.Text.Trim();

                if (BranchDetail != null)
                {
                    UpdatetBranch = new ORM.Master.Branch();
                    UpdatetBranch.MerchantID = (int)BranchDetail.MerchantID;
                    UpdatetBranch.SysBranchID = (int)BranchDetail.SysBranchID;
                    UpdatetBranch.Ordinary = BranchDetail.Ordinary == null ? (int?)null : Convert.ToInt32(BranchDetail.Ordinary);
                    UpdatetBranch.BranchID = BranchDetail.BranchID;
                    UpdatetBranch.BranchName = BranchDetail.BranchName;
                    UpdatetBranch.Address = address;
                    if (ProvincesId == 0 & AmphuresId == 0 & DistrictsId == 0)
                    {
                        UpdatetBranch.ProvincesId = null;
                        UpdatetBranch.AmphuresId = null;
                        UpdatetBranch.DistrictsId = null;
                    }
                    else if (ProvincesId != 0 & AmphuresId == 0 | DistrictsId == 0)
                    {
                        if (AmphuresId == 0)
                        {
                            Toast.MakeText(this, GetString(Resource.String.cannotsave) +
                                                     GetString(Resource.String.selectdistict), ToastLength.Short).Show();
                            return false;
                        }
                        if (DistrictsId == 0)
                        {
                            Toast.MakeText(this, GetString(Resource.String.cannotsave) +
                                                     GetString(Resource.String.selectsubdistict), ToastLength.Short).Show();
                            return false;
                        }
                    }
                    else
                    {
                        UpdatetBranch.ProvincesId = ProvincesId;
                        UpdatetBranch.AmphuresId = AmphuresId;
                        UpdatetBranch.DistrictsId = DistrictsId;
                    }
                    UpdatetBranch.Status = BranchDetail.Status;
                    UpdatetBranch.DisplayLanguage = BranchDetail.DisplayLanguage;
                    UpdatetBranch.Lat = BranchDetail.Lat;
                    UpdatetBranch.Lng = BranchDetail.Lng;
                    UpdatetBranch.Email = BranchDetail.Email;
                    UpdatetBranch.Tel = phone;
                    UpdatetBranch.Line = BranchDetail.Line;
                    UpdatetBranch.Facebook = BranchDetail.Facebook;
                    UpdatetBranch.Instagram = BranchDetail.Instagram;
                    UpdatetBranch.TaxBranchName = BranchDetail.TaxBranchName == null ? BranchDetail.BranchName : BranchDetail.TaxBranchName;
                    UpdatetBranch.TaxBranchID = BranchDetail.TaxBranchID;
                    UpdatetBranch.LinkProMaxxID = linkPromaxx;
                    UpdatetBranch.Comments = comment;
                }
                var update = await GabanaAPI.PutDataBranch(UpdatetBranch); //Cloud
                if (update.Status)
                {
                    //insert local
                    var branch = JsonConvert.DeserializeObject<Gabana.ORM.Master.Branch>(update.Message);
                    if (branch != null)
                    {
                        //Branch
                        BranchManage branchManage = new BranchManage();
                        ORM.MerchantDB.Branch insertBranch = new ORM.MerchantDB.Branch();
                        insertBranch.MerchantID = branch.MerchantID;
                        insertBranch.SysBranchID = branch.SysBranchID;
                        insertBranch.Ordinary = branch.Ordinary;
                        insertBranch.BranchName = branch.BranchName;
                        insertBranch.BranchID = branch.BranchID;
                        insertBranch.Address = branch.Address;
                        insertBranch.ProvincesId = branch.ProvincesId;
                        insertBranch.AmphuresId = branch.AmphuresId;
                        insertBranch.DistrictsId = branch.DistrictsId;
                        insertBranch.Status = branch.Status;
                        insertBranch.DisplayLanguage = branch.DisplayLanguage;
                        insertBranch.Lat = branch.Lat;
                        insertBranch.Lng = branch.Lng;
                        insertBranch.Email = branch.Email;
                        insertBranch.Tel = branch.Tel;
                        insertBranch.Line = branch.Line;
                        insertBranch.Facebook = branch.Facebook;
                        insertBranch.Instagram = branch.Instagram;
                        insertBranch.TaxBranchName = branch.TaxBranchName == null ? branch.BranchName : branch.TaxBranchName;
                        insertBranch.TaxBranchID = branch.TaxBranchID;
                        insertBranch.LinkProMaxxID = branch.LinkProMaxxID;
                        insertBranch.Comments = branch.Comments;

                        var insert = await branchManage.UpdateBranch(insertBranch); //Local
                        var index = DataCashingAll.Merchant.Branch.FindIndex(x => x.SysBranchID == 1); //สาขาสำนักงานใหญ่
                        if (index != -1)
                        {
                            DataCashingAll.Merchant.Branch.RemoveAt(index);
                            DataCashingAll.Merchant.Branch.Add(UpdatetBranch);
                        }
                        return true;
                    }
                    return false;
                }
                else
                {
                    Toast.MakeText(this, update.Message, ToastLength.Short).Show();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("UpdatetBranchDetail at Merchant");
                return false;
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