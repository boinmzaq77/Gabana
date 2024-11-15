
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.CardView.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.SwipeRefreshLayout.Widget;
using AutoMapper;
using Gabana.Droid.Tablet.Dialog;
using Gabana.ORM.MerchantDB;
using Gabana.ORM.PoolDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using iTextSharp.text;
using Java.Security.Cert;
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
    public class Setting_Fragment_BranchDetail : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Setting_Fragment_BranchDetail NewInstance()
        {
            Setting_Fragment_BranchDetail frag = new Setting_Fragment_BranchDetail();
            return frag;
        }

        View view;
        SwipeRefreshLayout refreshlayout;
        public static Setting_Fragment_BranchDetail fragment_main;

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
        bool first = true, flagdatachange = false;
        string LoginType;
        public static Branch branchEdit;
        BranchManage branchmanage = new BranchManage();
        int BranchId;
        string caseEvent = "";

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.setting_fragment_branchdetail, container, false);
            try
            {
                fragment_main = this;
                ComBineUI();
                LoginType = Preferences.Get("LoginType", "");

                Android.Content.Res.Resources res = this.Resources;
                string selectzipcode = res.GetString(Resource.String.addcustomer_activity_selectzipcode);
                txtzipcode.Text = selectzipcode;

                Provinces = new List<string>();
                Amphures = new List<string>();
                District = new List<string>();

               
                SetButtonAdd(false);
                SetUIFromMainRole(LoginType);
                first = false;
                return view;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackPageViewAsync("OnCreate at Merchant");
                _ = TinyInsights.TrackErrorAsync(ex);
                return view;
            }
        }

        async Task<bool> UpdatetBranch()
        {
            try
            {
                string branchName = txtBranchName.Text.Trim();
                if (string.IsNullOrEmpty(branchName))
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                    return false;
                }
                string branchid = txtBranchId.Text;
                if (string.IsNullOrEmpty(branchid))
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                    return false;
                }

                string TAXbranchName = txtTaxBranchName.Text.Trim();
                string TAXbranchID = txtTaxBranchID.Text.Trim();
                string TAXID = txtTax.Text.Trim();
                string RegistrationNo = txtRegistrationNo.Text.Trim();
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

                Branch UpdatetBranch = new Branch();
                UpdatetBranch.MerchantID = branchEdit.MerchantID;
                UpdatetBranch.SysBranchID = Convert.ToInt64(branchid);
                UpdatetBranch.Ordinary = branchEdit.Ordinary;
                UpdatetBranch.BranchName = branchName;
                UpdatetBranch.BranchID = branchid;
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
                UpdatetBranch.DisplayLanguage = branchEdit.DisplayLanguage;
                UpdatetBranch.Lat = branchEdit.Lat;
                UpdatetBranch.Lng = branchEdit.Lng;
                UpdatetBranch.Email = branchEdit.Email;
                UpdatetBranch.Tel = phone;
                UpdatetBranch.Line = branchEdit.Line;
                UpdatetBranch.Facebook = branchEdit.Facebook;
                UpdatetBranch.Instagram = branchEdit.Instagram;
                UpdatetBranch.TaxBranchName = TAXbranchName;
                UpdatetBranch.TaxBranchID = TAXbranchID;
                UpdatetBranch.Status = branchEdit.Status;
                UpdatetBranch.LinkProMaxxID = linkPromaxx;
                UpdatetBranch.Comments = comment;

                //Update Online
                //Mapping 
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ORM.MerchantDB.Branch, Gabana.ORM.Master.Branch>();
                });

                var Imapper = config.CreateMapper();
                var MasterBranch = Imapper.Map<ORM.MerchantDB.Branch, Gabana.ORM.Master.Branch>(UpdatetBranch);

                //Branch
                var result = await GabanaAPI.PutDataBranch(MasterBranch);
                if (!result.Status)
                {
                    Toast.MakeText(this.Activity, result.Message, ToastLength.Short).Show();
                    branchEdit = new Branch();
                    return false;
                }

                //Update local
                var branch = JsonConvert.DeserializeObject<Gabana.ORM.Master.Branch>(result.Message);

                //Mapping 
                var configlocal = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Gabana.ORM.Master.Branch, ORM.MerchantDB.Branch>();
                });

                var Imapperlocal = configlocal.CreateMapper();
                var Branchlocal = Imapperlocal.Map<Gabana.ORM.Master.Branch, ORM.MerchantDB.Branch>(branch);

                //Update local
                var update = await branchmanage.UpdateBranch(Branchlocal);
                if (!update)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                    return false;
                }

                Toast.MakeText(this.Activity, GetString(Resource.String.savesucess), ToastLength.Short).Show();
                Setting_Fragment_Branch.fragment_main.ReloadBranch(Branchlocal);
                return true;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("UpdatetBranch at add Branch");
                return false;
            }
        }

        private async void BtnAddBranch_Click(object sender, EventArgs e)
        {
            try
            {                
                btnAddBranch.Enabled = false;
                if (!DataCashing.CheckNet)
                {
                    btnAddBranch.Enabled = true;
                    Toast.MakeText(this.Activity, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    return;
                }

                bool Check;
                if (branchEdit == null)
                {
                    Check =  await InsertBranch();
                    if (!Check) return;
                }
                else
                {
                    Check = await  UpdatetBranch();
                    if (!Check) 
                    Setting_Fragment_Main.SetEnableBtnBranch();                   
                }
                SetClearData();
                btnAddBranch.Enabled = true;
            }
            catch (Exception ex)
            {
                btnAddBranch.Enabled = true;
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("BtnAddBranch_Click at add Branch");
                return;
            }
        }

        async Task<bool> InsertBranch()
        {
            try
            {
                string branchName = txtBranchName.Text.Trim();
                if (string.IsNullOrEmpty(branchName))
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                    return false;
                }

                string TAXbranchName = branchName;
                string TAXbranchID = branchName;
                string TAXID = txtTax.Text.Trim();
                string RegistrationNo = txtRegistrationNo.Text.Trim();
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

                Branch insertBranch = new Branch();
                insertBranch.MerchantID = DataCashingAll.MerchantId;
                insertBranch.SysBranchID = BranchId;
                insertBranch.Ordinary = null;
                insertBranch.BranchName = branchName;
                insertBranch.BranchID = null;
                insertBranch.Address = address;
                if (ProvincesId == 0 & AmphuresId == 0 & DistrictsId == 0)
                {
                    insertBranch.ProvincesId = null;
                    insertBranch.AmphuresId = null;
                    insertBranch.DistrictsId = null;
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
                    insertBranch.ProvincesId = ProvincesId;
                    insertBranch.AmphuresId = AmphuresId;
                    insertBranch.DistrictsId = DistrictsId;
                }
                insertBranch.DisplayLanguage = 'L';
                insertBranch.Lat = null;
                insertBranch.Lng = null;
                insertBranch.Email = null;
                insertBranch.Tel = phone;
                insertBranch.Line = null;
                insertBranch.Facebook = null;
                insertBranch.Instagram = null;
                insertBranch.TaxBranchName = TAXbranchName;
                insertBranch.TaxBranchID = TAXbranchID;
                //insertBranch.TaxID = TAXID;
                //insertBranch.RegMark = null;
                insertBranch.LinkProMaxxID = linkPromaxx;
                insertBranch.Comments = comment;
                insertBranch.Status = 'A';

                //Insert Online
                //Mapping 
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<ORM.MerchantDB.Branch, Gabana.ORM.Master.Branch>();
                });

                var Imapper = config.CreateMapper();
                var MasterBranch = Imapper.Map<ORM.MerchantDB.Branch, Gabana.ORM.Master.Branch>(insertBranch);

                //Branch
                var result = await GabanaAPI.PostDataBranch(MasterBranch);
                if (!result.Status)
                {
                    Toast.MakeText(this.Activity, result.Message, ToastLength.Short).Show();
                    return false;
                }

                //insert local
                var branch = JsonConvert.DeserializeObject<Gabana.ORM.Master.Branch>(result.Message);

                //Mapping 
                var configlocal = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Gabana.ORM.Master.Branch, ORM.MerchantDB.Branch>();
                });

                var Imapperlocal = configlocal.CreateMapper();
                var Branchlocal = Imapperlocal.Map<Gabana.ORM.Master.Branch, ORM.MerchantDB.Branch>(branch);

                var insert = await branchmanage.InsertBranch(Branchlocal);
                if (!insert)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                    return false;
                }

                Toast.MakeText(this.Activity, GetString(Resource.String.savesucess), ToastLength.Short).Show();
                BranchId = 0;
                Setting_Fragment_Branch.fragment_main.ReloadBranch(Branchlocal);
                return true;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("InsertBranch at add Branch");
                return false;
            }
        }

        int sysBranchID;
        private async void GetMaxBranch()
        {
            sysBranchID = await branchmanage.GetLastBranch();
        }

        public override void OnResume()
        {
            base.OnResume();

            //if (!IsVisible)
            //{
            //    return;
            //}

            SelectProvince();
            showdetail = false;
            SetShowdetail();
            first = true;
            UINewBranch();
            if (DataCashing.EditBranch == null)
            {
                textTitle.Text = GetString(Resource.String.branch_activity_addbranch);                
                GetMaxBranch();
                BranchId = sysBranchID + 1;
                txtBranchId.Text = BranchId.ToString();
                caseEvent = "insert";                
            }
            else
            {
                textTitle.Text = GetString(Resource.String.branch_activity_editbranch);                
                SetBranchDetail();
                caseEvent = "update";
            }
            SetUIFromMainRole(LoginType);

            bool check = UtilsAll.CheckPermissionRoleUser(Preferences.Get("LoginType", ""), "delete", "branch");
            if (check && DataCashing.EditBranch != null)
            {
                lnDelete.Visibility = ViewStates.Visible;
                btnDelete.Visibility = ViewStates.Visible;
            }
            else
            {
                lnDelete.Visibility = ViewStates.Gone;
                btnDelete.Visibility = ViewStates.Gone;
            }
            view.ClearFocus();
            first = false; 
            flagdatachange = false;
            SetButtonAdd(false);
        }

        LinearLayout lnBack, lnDetails;
        TextView textTitle, txtzipcode;
        internal Button btnAddBranch;
        EditText txtBranchId, txtBranchName, txtTaxBranchName, txtTaxBranchID, txtTax,
                    txtRegistrationNo, txtPhoneNumber, txtaddress, txtcomment, txtlinkpromaxx;
        ImageButton btnShowDetail, btnprovinces, btnAmphures, btnDistrict;
        FrameLayout lnShowDetail, btnDelete, lnDelete;
        Spinner spinnerProvince, spinnerAmphures, spinnerDistrict;

        private void ComBineUI()
        {
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            textTitle = view.FindViewById<TextView>(Resource.Id.textTitle);
            txtBranchId = view.FindViewById<EditText>(Resource.Id.txtBranchId);
            txtBranchName = view.FindViewById<EditText>(Resource.Id.txtBranchName);
            btnShowDetail = view.FindViewById<ImageButton>(Resource.Id.btnShowDetail);
            lnShowDetail = view.FindViewById<FrameLayout>(Resource.Id.lnShowDetail);
            lnDetails = view.FindViewById<LinearLayout>(Resource.Id.lnDetails);
            txtTaxBranchName = view.FindViewById<EditText>(Resource.Id.txtTaxBranchName);
            txtTaxBranchID = view.FindViewById<EditText>(Resource.Id.txtTaxBranchID);
            txtTax = view.FindViewById<EditText>(Resource.Id.txtTax);
            txtRegistrationNo = view.FindViewById<EditText>(Resource.Id.txtRegistrationNo);
            txtPhoneNumber = view.FindViewById<EditText>(Resource.Id.txtPhoneNumber);
            txtaddress = view.FindViewById<EditText>(Resource.Id.txtaddress);
            spinnerProvince = view.FindViewById<Spinner>(Resource.Id.spinnerProvince);
            btnprovinces = view.FindViewById<ImageButton>(Resource.Id.btnprovinces);
            spinnerAmphures = view.FindViewById<Spinner>(Resource.Id.spinneramphures);
            btnAmphures = view.FindViewById<ImageButton>(Resource.Id.btnAmphures);
            spinnerDistrict = view.FindViewById<Spinner>(Resource.Id.spinnerdistrict);
            btnDistrict = view.FindViewById<ImageButton>(Resource.Id.btnDistrict);
            txtzipcode = view.FindViewById<TextView>(Resource.Id.txtzipcode);
            txtcomment = view.FindViewById<EditText>(Resource.Id.txtcomment);
            txtlinkpromaxx = view.FindViewById<EditText>(Resource.Id.txtlinkpromaxx);
            btnDelete = view.FindViewById<FrameLayout>(Resource.Id.btnDelete);
            lnDelete = view.FindViewById<FrameLayout>(Resource.Id.lnDelete);
            btnAddBranch = view.FindViewById<Button>(Resource.Id.btnAddBranch);

            txtBranchId.TextChanged += TextChanged;
            txtBranchName.TextChanged += TextChanged;
            txtTaxBranchName.TextChanged += TextChanged;
            txtTaxBranchID.TextChanged += TextChanged;
            txtTax.TextChanged += TextChanged;
            txtRegistrationNo.TextChanged += TextChanged;
            txtPhoneNumber.TextChanged += TxtPhoneNumber_TextChanged;
            txtaddress.TextChanged += TextChanged;
            txtcomment.TextChanged += TextChanged;
            txtlinkpromaxx.TextChanged += TextChanged;
            txtzipcode.TextChanged += TextChanged;
            lnBack.Click += LnBack_Click;
            lnShowDetail.Click += LnShowDetail_Click;
            lnDelete.Click += LnDelete_Click;
            btnDelete.Click += LnDelete_Click;
            btnprovinces.Click += Btnprovinces_Click;
            btnAmphures.Click += Btndistrict_Click;
            btnDistrict.Click += Btnsubdustrict_Click;
            btnAddBranch.Click += BtnAddBranch_Click;
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

        private void Btnsubdustrict_Click(object sender, EventArgs e)
        {
            spinnerDistrict.PerformClick();
        }

        private void Btndistrict_Click(object sender, EventArgs e)
        {
            spinnerAmphures.PerformClick();
        }

        private void Btnprovinces_Click(object sender, EventArgs e)
        {
            spinnerProvince.PerformClick();
        }

        private async void LnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                DataCashing.EditBranch = branchEdit;
                var fragment = new Branch_Dialog_Delete();
                fragment.Show(MainActivity.main_activity.SupportFragmentManager, nameof(Branch_Dialog_Delete));
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("BtnDelete_Click at add Branch");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void SetButtonAdd(bool enable)
        {
            if (enable)
            {
                btnAddBranch.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnAddBranch.SetTextColor(Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnAddBranch.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnAddBranch.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
            }
            btnAddBranch.Enabled = enable;

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
            bool check = UtilsAll.CheckPermissionRoleUser(typeLogin, caseEvent, "branch ");
            if (check)
            {
                txtBranchId.Enabled = true;
                txtBranchId.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtBranchId.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                txtBranchName.Enabled = true;
                txtBranchName.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtBranchName.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                txtTaxBranchID.Enabled = true;
                txtTaxBranchID.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtTaxBranchID.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
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
                btnprovinces.SetBackgroundResource(Resource.Mipmap.Next);
                spinnerAmphures.Enabled = true;
                btnAmphures.Enabled = true;
                btnAmphures.SetBackgroundResource(Resource.Mipmap.Next);
                spinnerDistrict.Enabled = true;
                btnDistrict.Enabled = true;
                btnDistrict.SetBackgroundResource(Resource.Mipmap.Next);
                txtzipcode.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtzipcode.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                txtcomment.Enabled = true;
                txtcomment.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtcomment.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                txtlinkpromaxx.Enabled = true;
                txtlinkpromaxx.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtlinkpromaxx.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                btnAddBranch.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnAddBranch.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
            }
            else
            {
                txtBranchId.Enabled = false;
                txtBranchId.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                txtBranchId.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtBranchName.Enabled = false;
                txtBranchName.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                txtBranchName.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtTaxBranchID.Enabled = false;
                txtTaxBranchID.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                txtTaxBranchID.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
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
                btnprovinces.SetBackgroundResource(Resource.Mipmap.NextG);
                spinnerAmphures.Enabled = false;
                btnAmphures.Enabled = false;
                btnAmphures.SetBackgroundResource(Resource.Mipmap.NextG);
                spinnerDistrict.Enabled = false;
                btnDistrict.Enabled = false;
                btnDistrict.SetBackgroundResource(Resource.Mipmap.NextG);
                txtzipcode.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                txtzipcode.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                txtcomment.Enabled = false;
                txtcomment.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                txtcomment.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtlinkpromaxx.Enabled = false;
                txtlinkpromaxx.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                txtlinkpromaxx.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                btnAddBranch.SetBackgroundResource(Resource.Drawable.btnWhiteBorderGray);
                btnAddBranch.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
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

        MerchantManage merchantManage = new MerchantManage();
        PoolManage poolManage = new PoolManage();
        string Phone = string.Empty;
        string Name = string.Empty;
        string TaxID = string.Empty;
        string RegMark = string.Empty;
        async void SetBranchDetail()
        {
            try
            {
                if (DataCashing.EditBranch != null)
                {
                    first = true;
                    branchEdit = DataCashing.EditBranch;
                    txtBranchName.Text = branchEdit.BranchName;
                    txtBranchId.Text = branchEdit.BranchID;
                    txtTaxBranchName.Text = branchEdit.TaxBranchName;
                    txtTaxBranchID.Text = branchEdit.TaxBranchID;
                    txtPhoneNumber.Text = addTextTel(branchEdit.Tel);
                    txtaddress.Text = branchEdit.Address;
                    txtcomment.Text = branchEdit.Comments;
                    txtlinkpromaxx.Text = branchEdit.LinkProMaxxID;

                    #region Set Select Spinner
                    //Set Select Spinner
                    //Province
                    List<string> items = new List<string>();
                    Provinces = new List<string>();
                    Province province = new Province();
                    List<Province> lstprovince = new List<Province>();
                    List<Province> getallprovince = new List<Province>();

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

                    long? provinceid = branchEdit.ProvincesId;
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
                        return;
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
                    spinnerAmphures.Adapter = adapterAmphure;

                    long? amphureID = branchEdit.AmphuresId;

                    if (amphureID != null)
                    {
                        var data = getallamphures.Where(x => x.AmphuresId == amphureID).FirstOrDefault();
                        if (data != null)
                        {
                            int position;
                            position = adapterAmphure.GetPosition(data.AmphuresNameTH);
                            spinnerAmphures.SetSelection(position);
                            AmphuresId = amphureID == 0 ? 0 : (int)amphureID;
                        }
                    }
                    else
                    {
                        int position = adapterAmphure.GetPosition(selectamphure);
                        spinnerAmphures.SetSelection(position);

                    }

                    if (amphureID == null)
                    {
                        return;
                    }
                    //District
                    District = new List<string>();
                    District district = new District();
                    List<District> lstdistrict = new List<District>();
                    List<District> getalldistrict = new List<District>();

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
                    spinnerDistrict.Adapter = adapterDistrict;

                    long? districtID = branchEdit.DistrictsId;

                    if (districtID != null)
                    {
                        var data = getalldistrict.Where(x => x.DistrictsId == districtID).FirstOrDefault();
                        if (data != null)
                        {
                            int position;
                            position = adapterDistrict.GetPosition(data.DistrictsNameTH);
                            spinnerDistrict.SetSelection(position);
                            DistrictsId = districtID == 0 ? 0 : (int)districtID;
                        }
                    }
                    else
                    {
                        int position = adapterDistrict.GetPosition(selectdistrict);
                        spinnerDistrict.SetSelection(position);
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
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("BranchDetail at add Branch");
                return;
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
                spinnerAmphures.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinneramphures_ItemSelected);
                var adapterspinneramphures = new ArrayAdapter<string>(this.Activity, Resource.Layout.spinner_item, items);
                adapterspinneramphures.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinnerAmphures.Adapter = adapterspinneramphures;

                int position = GetAmphures.FindIndex(x => x.AmphuresId == AmphuresId);
                spinnerAmphures.SetSelection(position);
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
                spinnerDistrict.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinnerdistrict_ItemSelected);
                var adapterspinnerdistrict = new ArrayAdapter<string>(this.Activity, Resource.Layout.spinner_item, items);
                adapterspinnerdistrict.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinnerDistrict.Adapter = adapterspinnerdistrict;

                int position = GetDistricts.FindIndex(x => x.DistrictsId == DistrictsId);
                spinnerDistrict.SetSelection(position);
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

                if (branchEdit?.ProvincesId == ProvincesId)
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
                }


                int position = GetProvinces.FindIndex(x => x.ProvincesId == ProvincesId);
                spinnerProvince.SetSelection(position);
                SelectAmphures();
                CheckDataChange();
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
                string select = spinnerAmphures.SelectedItem.ToString();
                if (DataCashing.Amphures == null)
                {
                    return;
                }

                if (branchEdit?.ProvincesId == ProvincesId)
                {
                    AmphuresId = (int)GetAmphures.Find(x => x.AmphuresId == AmphuresId).AmphuresId;
                }
                else
                {
                    AmphuresId = (int)GetAmphures[e.Position].AmphuresId;

                }

                if (GetAmphures[e.Position].AmphuresNameTH == select & select != "อำเภอ")
                {
                    AmphuresId = (int)GetAmphures[e.Position].AmphuresId;
                }

                int position = GetAmphures.FindIndex(x => x.AmphuresId == AmphuresId);
                spinnerAmphures.SetSelection(position);
                SelectDistrict();
                CheckDataChange();

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
                string select = spinnerDistrict.SelectedItem.ToString();
                if (DataCashing.Districts == null)
                {
                    return;
                }
                //DistrictsId = int.Parse(DataCashing.Districts[e.Position]);
                if (branchEdit?.DistrictsId == DistrictsId)
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
                spinnerDistrict.SetSelection(position);


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
            if (!flagdatachange)
            {
                SetClearData(); return;
            }

            if (DataCashing.EditBranch == null)
            {
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.add_dialog_back.ToString();
                bundle.PutString("message", myMessage);
                Add_Dialog_Back.SetPage("branch");
                Add_Dialog_Back add_Dialog = Add_Dialog_Back.NewInstance();
                add_Dialog.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
                return;
            }
            else
            {
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.edit_dialog_back.ToString();
                bundle.PutString("message", myMessage);
                Edit_Dialog_Back.SetPage("branch");
                Edit_Dialog_Back edit_Dialog = Edit_Dialog_Back.NewInstance();
                edit_Dialog.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
                return;
            }
        }

        private void TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
        }

        private void CheckDataChange()
        {
            if (first)
            {
                SetButtonAdd(false);
                return;
            }
            if (branchEdit == null)
            {
                if (!string.IsNullOrEmpty(txtBranchName.Text))
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                SetButtonAdd(false);
            }
            else
            {
                if (txtBranchName.Text != branchEdit.BranchName)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                if (txtBranchId.Text != branchEdit.BranchID)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                if (txtTaxBranchName.Text != branchEdit.TaxBranchName)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                if (txtPhoneNumber.Text != addTextTel(branchEdit.Tel))
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                if (txtaddress.Text != branchEdit.Address)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                if (txtcomment.Text != branchEdit.Comments)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                int.TryParse(branchEdit.ProvincesId?.ToString(), out int provintid);
                if (ProvincesId != provintid)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                int.TryParse(branchEdit.AmphuresId?.ToString(), out int amphureid);
                if (AmphuresId != amphureid)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                int.TryParse(branchEdit.DistrictsId?.ToString(), out int districtsid);
                if (DistrictsId != districtsid)
                {
                    SetButtonAdd(true);
                    flagdatachange = true;
                    return;
                }
                SetButtonAdd(false);
                SetUIFromMainRole(LoginType);
            }
        }

        void UINewBranch()
        {
            try
            {
                textTitle.Text = string.Empty;
                txtBranchId.Text = string.Empty;
                txtBranchName.Text = string.Empty;
                txtTaxBranchName.Text = string.Empty;
                txtTaxBranchID.Text = string.Empty;
                txtTax.Text = string.Empty;
                txtRegistrationNo.Text = string.Empty;
                txtPhoneNumber.Text = string.Empty;
                txtaddress.Text = string.Empty;
                txtzipcode.Text = string.Empty;
                txtcomment.Text = string.Empty;
                txtlinkpromaxx.Text = string.Empty;

                Android.Content.Res.Resources res = this.Resources;
                string selectzipcode = res.GetString(Resource.String.addcustomer_activity_selectzipcode);
                txtzipcode.Text = selectzipcode;

                Provinces = new List<string>();
                Amphures = new List<string>();
                District = new List<string>();

                SelectProvince();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("UINewCustomer at addBranch");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public void SetClearData()
        {
            UINewBranch();
            DataCashing.EditBranch = null;
            branchEdit = null;
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "branch");
        }



    }
}