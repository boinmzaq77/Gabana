
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.CardView.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Droid.Tablet.Fragments.More;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ORM.PoolDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using LinqToDB.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet.Fragments.Setting
{
    public class Setting_Fragment_Merchant : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static Setting_Fragment_Merchant NewInstance()
        {
            Setting_Fragment_Merchant frag = new Setting_Fragment_Merchant();
            return frag;
        }

        View view;
        public static Setting_Fragment_Merchant fragment_main;

        private bool showdetail;
        Gabana3.JAM.Merchant.Merchants MerchantDetail;
        Merchant MerchantLocal;
        int ProvincesId, AmphuresId, DistrictsId;
        List<string> Provinces;
        List<string> Amphures;
        List<string> District;
        List<string> zipcode;

        List<Province> GetProvinces = new List<Province>();
        List<Amphure> GetAmphures = new List<Amphure>();
        List<District> GetDistricts = new List<District>();

        internal static Android.Net.Uri keepCropedUri = null;
        Android.Graphics.Bitmap bitmap;
        Android.Net.Uri cameraTakePictureUri;
        bool  flagdatachange = false;
        string usernamelogin, LoginType;

        public override  View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.setting_fragment_merchant, container, false);
            try
            {
                fragment_main = this;
                ComBineUI();
                LoginType = Preferences.Get("LoginType", "");
                keepCropedUri = null;
                SetShowdetail();
                return view;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackPageViewAsync("OnCreate at Merchant");
                _ = TinyInsights.TrackErrorAsync(ex);
                return view;
            }
        }

        
        async void SetValue()
        {
            DialogLoading dialogLoading = new DialogLoading();
            try
            {
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(MainActivity.main_activity.SupportFragmentManager, nameof(DialogLoading));
                }

                await SetData();
                await ShowBranchDetail();
                await ShowDetailMerchant();
                SetButtonAdd(false);
                SetUIFromMainRole(LoginType);
                CheckDataChange();

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity,ex.Message,ToastLength.Short).Show();
                dialogLoading.Dismiss();
            }
        }

        public override  void OnResume()
        {
            try
            {
                base.OnResume();

                //if (!IsVisible)
                //{
                //    return;
                //}

                //เพิ่ม flag สำหรับตรวจจับว่ามีการกดเลือกรูปหรือไม่ เพราะ ตอนนี้จะเข้า Onresume ตลอดทำให้ข้อมูลที่เคยกรอกไว้หายไป
                if (DataCashing.flagChooseMedia)
                {
                    SetShowdetail();
                    SetImgProfile();
                    return;
                }

                showdetail = false;
                SetShowdetail();
                SetValue();
                view.ClearFocus();
                MainActivity.main_activity.CloseKeyboard(view);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnResume at Merchant");
            }
        }

        LinearLayout lnBack, lnDetails;
        internal static ImageButton imgProfile;
        ImageButton btnAddImage, btnShowDetail, btnprovinces, btnAmphures, btnDistrict;
        FrameLayout lnShowDetail;
        EditText txtMerchantName, txtTax, txtRegistrationNo, txtPhoneNumber, txtaddress , txtcomment, txtlinkpromaxx;
        Spinner spinnerProvince, spinneramphures, spinnerdistrict;
        TextView txtzipcode;
        internal Button btnSave;
        private void ComBineUI()
        {
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            lnDetails = view.FindViewById<LinearLayout> (Resource.Id.lnDetails);
            imgProfile = view.FindViewById<ImageButton>(Resource.Id.imgProfile);
            btnAddImage = view.FindViewById<ImageButton> (Resource.Id.btnAddImage);
            btnShowDetail =view.FindViewById<ImageButton>(Resource.Id.btnShowDetail);
            lnShowDetail = view.FindViewById<FrameLayout>(Resource.Id.lnShowDetail);
            txtMerchantName = view.FindViewById<EditText>(Resource.Id.txtMerchantName);
            txtTax = view.FindViewById<EditText>(Resource.Id.txtTax);
            txtRegistrationNo = view.FindViewById<EditText>(Resource.Id.txtRegistrationNo);
            txtPhoneNumber = view.FindViewById<EditText>(Resource.Id.txtPhoneNumber);
            txtaddress = view.FindViewById<EditText>(Resource.Id.txtaddress);
            spinnerProvince = view.FindViewById<Spinner>(Resource.Id.spinnerProvince);
            btnprovinces = view.FindViewById<ImageButton>(Resource.Id.btnprovinces);
            spinneramphures = view.FindViewById<Spinner>(Resource.Id.spinneramphures);
            btnAmphures = view.FindViewById<ImageButton>(Resource.Id.btnAmphures);
            spinnerdistrict = view.FindViewById<Spinner>(Resource.Id.spinnerdistrict);
            btnDistrict = view.FindViewById<ImageButton>(Resource.Id.btnDistrict);
            txtzipcode = view.FindViewById<TextView>(Resource.Id.txtzipcode);
            txtcomment = view.FindViewById<EditText>(Resource.Id.txtcomment);
            txtlinkpromaxx = view.FindViewById<EditText>(Resource.Id.txtlinkpromaxx);
            btnSave = view.FindViewById<Button>(Resource.Id.btnSave);

            txtMerchantName.TextChanged += TextChanged;
            txtTax.TextChanged += TextChanged;
            txtRegistrationNo.TextChanged += TextChanged;
            txtPhoneNumber.TextChanged += TxtPhoneNumber_TextChanged; ;
            txtaddress.TextChanged += TextChanged;
            txtcomment.TextChanged += TextChanged;
            txtlinkpromaxx.TextChanged += TextChanged;
            lnBack.Click += LnBack_Click;
            lnShowDetail.Click += LnShowDetail_Click;
            btnSave.Click += BtnSave_Click;
            btnAddImage.Click += BtnAddImage_Click;
            btnprovinces.Click += Btnprovinces_Click;
            btnAmphures.Click += BtnAmphures_Click;
            btnDistrict.Click += BtnDistrict_Click;
            imgProfile.Click += BtnAddImage_Click;
            txtzipcode.TextChanged += TextChanged;
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

        private async Task SetData()
        {
            try
            {
                BranchManage branchManage = new BranchManage();
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
                    Toast.MakeText(this.Activity, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    MerchantLocal = DataCashingAll.MerchantLocal;
                }
                else if (!await GabanaAPI.CheckSpeedConnection())
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
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
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context,ex.Message,ToastLength.Short).Show();
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

        private void BtnAddImage_Click(object sender, EventArgs e)
        {
            try
            {
                DataCashing.flagChooseMedia = true;
                var fragment = new Merchant_Dialog_ChooseMedia();
                fragment.Show(this.Activity.SupportFragmentManager, nameof(Merchant_Dialog_ChooseMedia));
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (!await GabanaAPI.CheckNetWork())
            {
                Toast.MakeText(Application.Context, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                return;
            }

            if (!await GabanaAPI.CheckSpeedConnection())
            {
                Toast.MakeText(Application.Context, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                return;
            }

            bool checkpass =  await EditMerchantConfig();
            if (checkpass)
            {
                this.OnResume();
                MainActivity.main_activity.more_fragment_main.OnResume();
            }
        }

        byte[] imageByte;
        async Task<bool> EditMerchantConfig()
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
                    Toast.MakeText(this.Activity, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                    return false;
                }
                merchants.TaxID = txtTax.Text.Trim();
                merchants.RegMark = txtRegistrationNo.Text.Trim();

                //Update Branch
                var resultBranch = await UpdatetBranchDetail();
                if (!resultBranch)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                    return false;
                }

                PutMerchant.Merchant = merchants;

                var updatemerchant = await GabanaAPI.PutMerchant(PutMerchant, imageByte);
                if (!updatemerchant.Status)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                    return false;
                }
                Toast.MakeText(this.Activity, GetString(Resource.String.savesucess), ToastLength.Short).Show();
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
                Merchant merchant = new Merchant()
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
                if (result)
                {
                    showdetail = false;
                    flagdatachange = false;
                    DataCashing.flagChooseMedia = false;
                    keepCropedUri = null;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("EditMerchantConfig at Merchant");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return false;
            }
        }


        ORM.Master.Branch UpdatetBranch;
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
                            Toast.MakeText(this.Activity, GetString(Resource.String.telnotcomplete), ToastLength.Short).Show();
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
                            Toast.MakeText(this.Activity, GetString(Resource.String.telnotcomplete), ToastLength.Short).Show();
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
                            Toast.MakeText(this.Activity, GetString(Resource.String.cannotsave) +
                                                     GetString(Resource.String.selectdistict), ToastLength.Short).Show();
                            return false;
                        }
                        if (DistrictsId == 0)
                        {
                            Toast.MakeText(this.Activity, GetString(Resource.String.cannotsave) +
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
                if (!update.Status)
                {
                    Toast.MakeText(this.Activity, update.Message, ToastLength.Short).Show();
                    return false;
                }

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
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("UpdatetBranchDetail at Merchant");
                return false;
            }
        }

        private void SetButtonAdd(bool enable)
        {
            if (enable)
            {
                btnSave.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnSave.SetTextColor(Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnSave.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnSave.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
            }
            btnSave.Enabled = enable;

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
                btnSave.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
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
                txtMerchantName.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                txtMerchantName.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtTax.Enabled = false;
                txtTax.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                txtTax.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtRegistrationNo.Enabled = false;
                txtRegistrationNo.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                txtRegistrationNo.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtPhoneNumber.Enabled = false;
                txtPhoneNumber.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                txtPhoneNumber.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtaddress.Enabled = false;
                txtaddress.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                txtaddress.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                spinnerProvince.Enabled = false;
                btnprovinces.Enabled = false;
                spinneramphures.Enabled = false;
                btnAmphures.Enabled = false;
                spinnerdistrict.Enabled = false;
                btnDistrict.Enabled = false;
                txtzipcode.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                txtzipcode.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtcomment.Enabled = false;
                txtcomment.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                txtcomment.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                btnSave.Enabled = false;
                btnSave.SetBackgroundResource(Resource.Drawable.btnWhiteBorderGray);
                btnSave.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                txtlinkpromaxx.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                txtlinkpromaxx.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                btnprovinces.SetBackgroundResource(Resource.Mipmap.NextG);
                btnAmphures.SetBackgroundResource(Resource.Mipmap.NextG);
                btnDistrict.SetBackgroundResource(Resource.Mipmap.NextG);
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
        Branch BranchDetail = new Branch();
        async Task ShowBranchDetail()
        {
            try
            {
                BranchManage branchManage = new BranchManage();
                BranchDetail = await branchManage.GetBranch(DataCashingAll.MerchantId, 1); //สาขาสำนักงานใหญ่
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
                _= TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ShowBranchDetail at Merchant");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        MerchantManage merchantManage = new MerchantManage();
        PoolManage poolManage = new PoolManage();
        string Phone = string.Empty;
        string Name = string.Empty;
        string TaxID = string.Empty;
        string RegMark = string.Empty;

        async Task ShowDetailMerchant()
        {
            try
            {
                btnSave.Text = GetString(Resource.String.textsave);

                #region Local Picture               
                var merchant = await merchantManage.GetMerchant(DataCashingAll.MerchantId);
                var cloudpath = merchant.LogoPath == null ? string.Empty : merchant.LogoPath;
                var localpath = merchant.LogoLocalPath == null ? string.Empty : merchant.LogoLocalPath;

                if (await GabanaAPI.CheckNetWork())
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
                    //var merchants = await GabanaAPI.GetMerchantDetail(DataCashingAll.DevicePlatform, DataCashingAll.DeviceUDID);
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
                Province province = new Province();
                var lstprovince = new List<Province>();
                var getallprovince = new List<Province>();

                Android.Content.Res.Resources res = this.Resources;
                string select = res.GetString(Resource.String.addcustomer_activity_selectprovince);

                province = new Province()
                {
                    ProvincesId = 0,
                    ProvincesNameEN = "Province",
                    ProvincesNameTH = select
                };
                lstprovince.Add(province);
                getallprovince = await poolManage.GetProvinces();
                lstprovince.AddRange(getallprovince);

                string[] province_array = lstprovince.Select(i => i.ProvincesNameTH.ToString()).ToArray();
                var adapterProvince = new ArrayAdapter<string>(this.Activity, Resource.Layout.spinner_item, province_array);
                adapterProvince.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinnerProvince.Adapter = adapterProvince;

                long? provinceid = BranchDetail.ProvincesId;
                if (provinceid != null)
                {
                    var data = getallprovince.Where(x => x.ProvincesId == provinceid).FirstOrDefault();
                    if (data != null)
                    {
                        int position;
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
                Amphure amphure = new Amphure();
                var lstamphures = new List<Amphure>();
                var getallamphures = new List<Amphure>();

                Android.Content.Res.Resources resamphur = this.Resources;
                string selectamphure = resamphur.GetString(Resource.String.addcustomer_activity_selectdistrict);

                amphure = new Amphure()
                {
                    AmphuresId = 0,
                    AmphuresNameEN = "Amphure",
                    AmphuresNameTH = selectamphure
                };
                lstamphures.Add(amphure);
                getallamphures = await poolManage.GetAmphures((int)provinceid);
                lstamphures.AddRange(getallamphures);

                string[] amphure_array = lstamphures.Select(i => i.AmphuresNameTH.ToString()).ToArray();
                var adapterAmphure = new ArrayAdapter<string>(this.Activity, Resource.Layout.spinner_item, amphure_array);
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
                District district = new District();
                var lstdistrict = new List<District>();
                var getalldistrict = new List<District>();

                Android.Content.Res.Resources resdistrict = this.Resources;
                string selectdistrict = resdistrict.GetString(Resource.String.addcustomer_activity_selectsubdistrict);

                district = new District()
                {
                    DistrictsId = 0,
                    DistrictsNameEN = "District",
                    DistrictsNameTH = selectdistrict
                };
                lstdistrict.Add(district);
                getalldistrict = await poolManage.GetDistricts((int)amphureID);
                lstdistrict.AddRange(getalldistrict);


                string[] district_array = lstdistrict.Select(i => i.DistrictsNameTH.ToString()).ToArray();
                var adapterDistrict = new ArrayAdapter<string>(this.Activity, Resource.Layout.spinner_item, district_array);
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

                var lstzipcode = new District();
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

                flagdatachange = false;
                #endregion
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("UpdateDetailCustomer at Merchant");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
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
                Province province = new Province();
                var lstprovince = new List<Province>();
                var getallprovince = new List<Province>();

                Android.Content.Res.Resources res = this.Resources;
                string select = res.GetString(Resource.String.addcustomer_activity_selectprovince);

                province = new Province()
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
                var adapterspinnerProvince = new ArrayAdapter<string>(this.Activity, Resource.Layout.spinner_item, items);
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
                Amphure amphure = new Amphure();
                var lstamphures = new List<Amphure>();
                var getallamphures = new List<Amphure>();

                Android.Content.Res.Resources res = this.Resources;
                string select = res.GetString(Resource.String.addcustomer_activity_selectdistrict);

                amphure = new Amphure()
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
                var adapterspinneramphures = new ArrayAdapter<string>(this.Activity, Resource.Layout.spinner_item, items);
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
                District district = new District();
                var lstdistrict = new List<District>();
                var getalldistrict = new List<District>();

                Android.Content.Res.Resources res = this.Resources;
                string select = res.GetString(Resource.String.addcustomer_activity_selectsubdistrict);

                district = new District()
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
                var adapterspinnerdistrict = new ArrayAdapter<string>(this.Activity, Resource.Layout.spinner_item, items);
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
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
                var lstzipcode = new District();
                lstzipcode = await poolManage.GetZipcode(AmphuresId, DistrictsId);
                if (lstzipcode.ZipCode != null)
                {
                    txtzipcode.Text = lstzipcode.ZipCode;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
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
        private void LnBack_Click(object sender, EventArgs e)
        {
            try
            {
                if (!flagdatachange)
                {
                    SetClearData(); return;
                }

                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.edit_dialog_back.ToString();
                bundle.PutString("message", myMessage);
                Edit_Dialog_Back.SetPage("merchant");
                Edit_Dialog_Back edit_Dialog = Edit_Dialog_Back.NewInstance();
                edit_Dialog.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
                return;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("LnBack_Click at add Customer");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public void SetClearData()
        {
            MainActivity.main_activity.SupportFragmentManager.BeginTransaction().Remove(MainActivity.main_activity.activeR).Commit();
        }

        private void TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
        }

        public void CheckDataChange()
        {
            try
            {
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

                //max lenght = 50
                var address = BranchDetail.Address == null ? string.Empty : BranchDetail.Address;

                int indexaddress = 0, indextxtaddress = 0;
                indexaddress = address.Length;
                if (indexaddress > 50)
                {
                   address =  address.Substring(0, 49);
                }

                indextxtaddress = txtaddress.Text.Length;
                if (indextxtaddress > 50)
                {
                    txtaddress.Text = txtaddress.Text.Substring(0, 49);
                }
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

        private void SetImgProfile()
        {
            try
            {
                if (keepCropedUri != null)
                {
                    //Clear รูปภาพก่อนทำอะไรใหม่
                    string setpathnull = string.Empty;
                    Android.Net.Uri urisetpathnull = Android.Net.Uri.Parse(setpathnull);
                    imgProfile.SetImageURI(urisetpathnull);

                    Android.Net.Uri cropImageURI = keepCropedUri;
                    bitmap = Android.Provider.MediaStore.Images.Media.GetBitmap(Application.Context.ContentResolver, cropImageURI);
                    imgProfile.SetImageBitmap(bitmap);
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
    }
}