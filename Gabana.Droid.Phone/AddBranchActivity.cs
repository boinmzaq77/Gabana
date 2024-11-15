using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AutoMapper;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ORM.PoolDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class AddBranchActivity : AppCompatActivity
    {
        public static AddBranchActivity addBranch;
        ImageButton btnShowDetail, btnprovinces, btnAmphures, btndistrict;
        EditText txtBranchName, txtBranchId, txtTaxBranchName, txtTaxBranchID, txtTax, txtRegistrationNo, txtPhoneNumber, txtaddress, txtcomment, txtlinkpromaxx;
        bool showdetail;
        FrameLayout lnShowDetail, btnDelete, lnDelete;
        LinearLayout lnDetails;
        internal Button btnAddBranch;
        Spinner spinnerProvince, spinneramphures, spinnerdistrict;
        TextView txtzipcode;
        BranchManage branchmanage = new BranchManage();
        PoolManage poolManage = new PoolManage();

        List<string> Provinces;
        List<string> Amphures;
        List<string> District;
        List<string> zipcode;
        int ProvincesId, AmphuresId, DistrictsId;

        public static Branch branchEdit;
        int sysBranchID;
        int BranchId;

        List<Province> GetProvinces = new List<Province>();
        List<Amphure> GetAmphures = new List<Amphure>();
        List<District> GetDistricts = new List<District>();
        string Phone = string.Empty;
        bool first = true, flagdatachange = false;
        string caseEvent = "";
        string LoginType;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.addbranch_activity);
                addBranch = this;
                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;
                TextView textTitle = FindViewById<TextView>(Resource.Id.textTitle);
                btnShowDetail = FindViewById<ImageButton>(Resource.Id.btnShowDetail);
                btnDelete = FindViewById<FrameLayout>(Resource.Id.btnDelete);

                txtBranchName = FindViewById<EditText>(Resource.Id.txtBranchName);
                txtBranchId = FindViewById<EditText>(Resource.Id.txtBranchId);
                txtTaxBranchName = FindViewById<EditText>(Resource.Id.txtTaxBranchName);
                txtTaxBranchID = FindViewById<EditText>(Resource.Id.txtTaxBranchID);
                txtTax = FindViewById<EditText>(Resource.Id.txtTax);
                txtRegistrationNo = FindViewById<EditText>(Resource.Id.txtRegistrationNo);
                txtPhoneNumber = FindViewById<EditText>(Resource.Id.txtPhoneNumber);
                txtaddress = FindViewById<EditText>(Resource.Id.txtaddress);
                spinnerProvince = FindViewById<Spinner>(Resource.Id.spinnerProvince);
                spinneramphures = FindViewById<Spinner>(Resource.Id.spinneramphures);
                spinnerdistrict = FindViewById<Spinner>(Resource.Id.spinnerdistrict);
                btnprovinces = FindViewById<ImageButton>(Resource.Id.btnprovinces);
                btnAmphures = FindViewById<ImageButton>(Resource.Id.btnAmphures);
                btndistrict = FindViewById<ImageButton>(Resource.Id.btndistrict);
                txtzipcode = FindViewById<TextView>(Resource.Id.txtzipcode);
                txtcomment = FindViewById<EditText>(Resource.Id.txtcomment);
                txtlinkpromaxx = FindViewById<EditText>(Resource.Id.txtlinkpromaxx);
                btnAddBranch = FindViewById<Button>(Resource.Id.btnAddBranch);

                lnDetails = FindViewById<LinearLayout>(Resource.Id.lnDetails);
                lnShowDetail = FindViewById<FrameLayout>(Resource.Id.lnShowDetail);
                lnDelete = FindViewById<FrameLayout>(Resource.Id.lnDelete);
                lnShowDetail.Click += LnShowDetail_Click;
                btnDelete.Click += BtnDelete_Click;
                lnDelete.Click += BtnDelete_Click;
                btnprovinces.Click += Btnprovinces_Click;
                btnAmphures.Click += Btndistrict_Click;
                btndistrict.Click += Btnsubdustrict_Click;
                txtPhoneNumber.TextChanged += TxtPhoneNumber_TextChanged;
                txtaddress.TextChanged += TxtBranchName_TextChanged;
                txtBranchName.TextChanged += TxtBranchName_TextChanged;
                txtBranchId.TextChanged += TxtBranchId_TextChanged;
                txtTaxBranchName.TextChanged += TxtTaxBranchName_TextChanged;
                txtTax.TextChanged += TxtTax_TextChanged;
                txtRegistrationNo.TextChanged += TxtRegistrationNo_TextChanged;
                txtcomment.TextChanged += Txtcomment_TextChanged;
                txtlinkpromaxx.TextChanged += Txtlinkpromaxx_TextChanged; ;
                txtzipcode.TextChanged += Txtzipcode_TextChanged;
                showdetail = false;

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

                CheckJwt();

                SetShowdetail();
                Task.Run(async () => {
                    SelectProvince();
                });

                LoginType = Preferences.Get("LoginType", "");

                if (branchEdit == null)
                {
                    textTitle.Text = GetString(Resource.String.branch_activity_addbranch);
                    btnDelete.Visibility = ViewStates.Gone;
                    sysBranchID = await branchmanage.GetLastBranch();
                    BranchId = sysBranchID + 1;
                    txtBranchId.Text = BranchId.ToString();
                    btnAddBranch.Click += BtnAddBranch_Click;                    
                    caseEvent = "insert";
                }
                else
                {
                    textTitle.Text = GetString(Resource.String.branch_activity_editbranch);
                    btnDelete.Visibility = ViewStates.Visible;
                    SetBranchDetail();
                    btnAddBranch.Click += BtnUpdateBranch_Click;                    
                    caseEvent = "update";
                }
                CheckDataChange();
                first = false;
                SetButtonAdd(false);
                SetUIFromMainRole(LoginType);

                _ = TinyInsights.TrackPageViewAsync("OnCreate : AddBranchActivity");
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("OnCreate at add Branch");
                return;
            }
        }

        private void SetUIFromMainRole(string typeLogin)
        {
            bool check = UtilsAll.CheckPermissionRoleUser(typeLogin, caseEvent, "branch");
            //case enable
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
                spinneramphures.Enabled = true;
                btnAmphures.Enabled = true;
                btnAmphures.SetBackgroundResource(Resource.Mipmap.Next);
                spinnerdistrict.Enabled = true;
                btndistrict.Enabled = true;
                btndistrict.SetBackgroundResource(Resource.Mipmap.Next);
                txtzipcode.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtzipcode.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                txtcomment.Enabled = true;
                txtcomment.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtcomment.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                txtlinkpromaxx.Enabled = true;
                txtlinkpromaxx.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtlinkpromaxx.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                btnAddBranch.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnAddBranch.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
            }
            else
            {
                txtBranchId.Enabled = false;
                txtBranchId.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtBranchId.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtBranchName.Enabled = false;
                txtBranchName.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtBranchName.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtTaxBranchID.Enabled = false;
                txtTaxBranchID.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtTaxBranchID.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
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
                btnprovinces.SetBackgroundResource(Resource.Mipmap.NextG);
                spinneramphures.Enabled = false;
                btnAmphures.Enabled = false;
                btnAmphures.SetBackgroundResource(Resource.Mipmap.NextG);
                spinnerdistrict.Enabled = false;
                btndistrict.Enabled = false;
                btndistrict.SetBackgroundResource(Resource.Mipmap.NextG);
                txtzipcode.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtzipcode.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                txtcomment.Enabled = false;
                txtcomment.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtcomment.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtlinkpromaxx.Enabled = false;
                txtlinkpromaxx.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtlinkpromaxx.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                btnAddBranch.SetBackgroundResource(Resource.Drawable.btnbordergray);
                btnAddBranch.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));

            }

            check = UtilsAll.CheckPermissionRoleUser(typeLogin, "delete", "branch");
            if (check)
            {
                btnDelete.Visibility = ViewStates.Visible;
            }
            else
            {
                btnDelete.Visibility = ViewStates.Gone;
            }
        }


        private void Txtzipcode_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
        }

        private void Txtlinkpromaxx_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
        }

        private void Txtcomment_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
        }

        private void TxtPhoneNumber_TextChanged1(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
        }

        private void TxtRegistrationNo_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
        }

        private void TxtTax_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
        }

        private void TxtTaxBranchName_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
        }

        private void TxtBranchId_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
        }

        private void TxtBranchName_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
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

        private void SetButtonAdd(bool enable)
        {
            if (enable)
            {
                btnAddBranch.SetBackgroundResource(Resource.Drawable.btnblue);
                btnAddBranch.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnAddBranch.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnAddBranch.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
            }
            btnAddBranch.Enabled = enable;
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
                _ = TinyInsights.TrackPageViewAsync("addTextTel at addbranch");
                return value;
            }
        }

        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                MainDialog dialog = new MainDialog();
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                bundle.PutString("message", myMessage);
                bundle.PutString("deleteType", "branch");
                bundle.PutInt("systemID", (int)branchEdit.SysBranchID);
                dialog.Arguments = bundle;
                dialog.Show(SupportFragmentManager, myMessage);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("BtnDelete_Click at add Branch");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void BtnUpdateBranch_Click(object sender, EventArgs e)
        {
            try
            {
                btnAddBranch.Enabled = false;
                if (!await GabanaAPI.CheckNetWork())
                {
                    Toast.MakeText(this, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    btnAddBranch.Enabled = true;
                    return;
                }

                if (!await GabanaAPI.CheckSpeedConnection())
                {
                    Toast.MakeText(this, GetString(Resource.String.unstableinternet), ToastLength.Long).Show(); 
                    btnAddBranch.Enabled = true;
                    return;
                }
                UpdatetBranch();
                btnAddBranch.Enabled = true;
            }
            catch (Exception ex)
            {
                btnAddBranch.Enabled = true;
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("BtnUpdateBranch_Click at add Branch");
                return;
            }
        }

        private async void BtnAddBranch_Click(object sender, EventArgs e)
        {
            try
            {
                btnAddBranch.Enabled = false;
                if (!await GabanaAPI.CheckNetWork())
                {
                    Toast.MakeText(this, GetString(Resource.String.nointernet), ToastLength.Short).Show(); btnAddBranch.Enabled = true;
                    return;
                }

                if (!await GabanaAPI.CheckSpeedConnection())
                {
                    Toast.MakeText(this, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                    btnAddBranch.Enabled = true;
                    return;
                }

                InsertBranch();
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

        public void DialogCheckBack()
        {
            base.OnBackPressed();
            flagdatachange = false;
            BranchActivity.flagLoadData = false;
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        public override void OnBackPressed()
        {
            try
            {
                if (branchEdit == null)
                {
                    if (!flagdatachange)
                    {
                        DialogCheckBack(); return;
                    }

                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.add_dialog_back.ToString();
                    bundle.PutString("message", myMessage);
                    bundle.PutString("fromPage", "branch");
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
                    bundle.PutString("fromPage", "branch");
                    dialog.Arguments = bundle;
                    dialog.Show(SupportFragmentManager, myMessage);
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Btnsubdustrict_Click(object sender, EventArgs e)
        {
            spinnerdistrict.PerformClick();
        }

        private void Btndistrict_Click(object sender, EventArgs e)
        {
            spinneramphures.PerformClick();
        }

        private void Btnprovinces_Click(object sender, EventArgs e)
        {
            spinnerProvince.PerformClick();
        }

        async void InsertBranch()
        {
            try
            {
                string branchName = txtBranchName.Text.Trim();
                if (string.IsNullOrEmpty(branchName))
                {
                    Toast.MakeText(this, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                    return;
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
                            Toast.MakeText(this, GetString(Resource.String.telnotcomplete), ToastLength.Short).Show();
                            return;
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
                            return;
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
                        Toast.MakeText(this, GetString(Resource.String.cannotsave) +
                                             GetString(Resource.String.selectdistict), ToastLength.Short).Show();
                        return;
                    }
                    if (DistrictsId == 0)
                    {
                        Toast.MakeText(this, GetString(Resource.String.cannotsave) +
                                             GetString(Resource.String.selectsubdistict), ToastLength.Short).Show();
                        return;
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
                    Toast.MakeText(this, result.Message, ToastLength.Short).Show();
                    return;
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
                    Toast.MakeText(this, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                    return;
                }

                Toast.MakeText(this, GetString(Resource.String.savesucess), ToastLength.Short).Show();
                BranchId = 0;
                //BranchActivity.flagLoadData = true;
                BranchActivity.SetFocusBranch(Branchlocal);
                this.Finish();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("InsertBranch at add Branch");
                return;
            }
        }

        async void SetBranchDetail()
        {
            try
            {
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
                var adapterProvince = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, province_array);
                adapterProvince.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinnerProvince.Adapter = adapterProvince;

                long? provinceid = branchEdit.ProvincesId;
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
                var adapterAmphure = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, amphure_array);
                adapterAmphure.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinneramphures.Adapter = adapterAmphure;

                long? amphureID = branchEdit.AmphuresId;

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
                var adapterDistrict = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, district_array);
                adapterDistrict.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinnerdistrict.Adapter = adapterDistrict;

                long? districtID = branchEdit.DistrictsId;

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


                var lstzipcode = new District();
                if (amphureID == 0 & districtID == 0)
                {
                    return;
                }

                lstzipcode = await poolManage.GetZipcode((int)amphureID, (int)districtID);
                if (lstzipcode.ZipCode != null)
                {
                    txtzipcode.Text = lstzipcode.ZipCode;
                }
                #endregion
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("BranchDetail at add Branch");
                return;
            }
        }

        async void UpdatetBranch()
        {
            try
            {
                string branchName = txtBranchName.Text.Trim();
                if (string.IsNullOrEmpty(branchName))
                {
                    Toast.MakeText(this, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                    return;
                }
                string branchid = txtBranchId.Text;
                if (string.IsNullOrEmpty(branchid))
                {
                    Toast.MakeText(this, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                    return;
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
                            Toast.MakeText(this, GetString(Resource.String.telnotcomplete), ToastLength.Short).Show();
                            return;
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
                            return;
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
                        Toast.MakeText(this, GetString(Resource.String.cannotsave) +
                                                 GetString(Resource.String.selectdistict), ToastLength.Short).Show();
                        return;
                    }
                    if (DistrictsId == 0)
                    {
                        Toast.MakeText(this, GetString(Resource.String.cannotsave) +
                                                 GetString(Resource.String.selectsubdistict), ToastLength.Short).Show();
                        return;
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
                    Toast.MakeText(this, result.Message, ToastLength.Short).Show();
                    branchEdit = new Branch();
                    return;
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
                    Toast.MakeText(this, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                    return;
                }

                Toast.MakeText(this, GetString(Resource.String.savesucess), ToastLength.Short).Show();
                //BranchActivity.flagLoadData = true;
                BranchActivity.SetFocusBranch(Branchlocal);
                this.Finish();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("UpdatetBranch at add Branch");
                return;
            }
        }

        //-------------------------------------------------------------------------
        //Spinner
        //--------------------------------------------------------------------------
        public async Task SelectProvince()
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
                var adapterspinnerProvince = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, items);
                adapterspinnerProvince.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinnerProvince.Adapter = adapterspinnerProvince;

                int position = GetProvinces.FindIndex(x => x.ProvincesId == ProvincesId);
                spinnerProvince.SetSelection(position);
                await SelectAmphures();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("SelectProvince at addbranch");
            }
        }

        public async Task SelectAmphures()
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
                var adapterspinneramphures = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, items);
                adapterspinneramphures.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinneramphures.Adapter = adapterspinneramphures;
                int position = GetAmphures.FindIndex(x => x.AmphuresId == AmphuresId);
                spinneramphures.SetSelection(position);
                await SelectDistrict();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("SelectAmphures at addbranch");
            }
        }

        public async Task SelectDistrict()
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
                var adapterspinnerdistrict = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, items);
                adapterspinnerdistrict.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinnerdistrict.Adapter = adapterspinnerdistrict;

                int position = GetDistricts.FindIndex(x => x.DistrictsId == DistrictsId);
                spinnerdistrict.SetSelection(position);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SelectDistrict at addbranch");
            }
        }

        private async void spinnerProvince_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
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
                await SelectAmphures();
                CheckDataChange();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private async void spinneramphures_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            try
            {
                string select = spinneramphures.SelectedItem.ToString();
                if (DataCashing.Amphures == null)
                {
                    return;
                }

                //if (SetDetailBranch?.ProvincesId == ProvincesId)
                //{
                //    AmphuresId = (int)GetAmphures.Find(x => x.AmphuresId == AmphuresId).AmphuresId;
                //}
                //else
                //{
                //    AmphuresId = (int)GetAmphures[e.Position].AmphuresId;
                //}

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
                spinneramphures.SetSelection(position);
                await SelectDistrict();
                CheckDataChange();

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

                if (branchEdit?.AmphuresId == AmphuresId)
                {
                    DistrictsId = (int)GetDistricts.Find(x => x.DistrictsId == DistrictsId).DistrictsId;
                }
                else
                {
                    DistrictsId = (int)GetDistricts[e.Position].DistrictsId;
                }

                if (GetDistricts[e.Position].DistrictsNameTH == select & select != "ตำบล")
                {
                    DistrictsId = (int)GetDistricts[e.Position].DistrictsId;
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
                CheckDataChange();


            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        async Task CheckJwt()
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