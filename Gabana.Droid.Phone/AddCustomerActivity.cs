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
using Gabana.ORM.PoolDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class AddCustomerActivity : AppCompatActivity
    {
        public static AddCustomerActivity addCustomer;
        EditText txtCustomerName, txtcustomerid, txtemail, txtLine, txtnationalID, txtaddress, txtcomment, txtlinkpromaxx, txtphonenumber, txtShortName;
        internal Button btnAdd;
        TextView txtbirthdate, txtzipcode, txtNameView, txtTitle;
        ImageView imgProfile;
        LinearLayout lnDetails, lnShowDetail;
        FrameLayout lnBack, lnDelete, lnShowDelete;
        ImageButton btnAddImage, imggender, imgbirthdate, btnShowDetail, btnprovinces, btnAmphure, btnDistrict, btnback;
        Spinner spinnergender, spinnerProvince, spinneramphures, spinnerDistrict, spinnerCustomerType;
        Android.App.DatePickerDialog dialogBirthdate;
        Android.Net.Uri keepCropedUri = null;
        Android.Graphics.Bitmap bitmap;

        int RESULT_OK = -1;
        int CAMERA_REQUEST = 0;
        int CROP_REQUEST = 1;
        int GALLERY_PICTURE = 2;
        CustomerManage CustomerManage = new CustomerManage();
        DeviceSystemSeqNo deviceSystemSeq = new DeviceSystemSeqNo();
        DeviceSystemSeqNoManage deviceSystemSeqNoManage = new DeviceSystemSeqNoManage();
        PoolManage poolManage = new PoolManage();
        string CustomerName, path, CustomerID;
        Android.Net.Uri cameraTakePictureUri;

        char Gender;
        int ProvincesId, AmphuresId, DistrictsId, CustomerTypeNo;
        DateTime? birthdate;
        List<string> Provinces;
        List<string> Amphures;
        List<string> District;
        List<string> zipcode;
        List<string> CustomerType;
        private bool showdetail;
        Customer customerEdit;
        public static long iSysCustomerID;
        static CultureInfo cultureUS = new CultureInfo("en-US");
        string pathThumnailFolder, Language, pathFolderPicture;
        public static bool CurrentActivity;
        List<Province> GetProvinces = new List<Province>();
        List<Amphure> GetAmphures = new List<Amphure>();
        List<District> GetDistricts = new List<District>();
        string Phone = string.Empty, idcard = string.Empty;
        TextView txtNoCustomerType;
        string usernamelogin, LoginType;
        bool first = true, flagdatachange = false;
        LinearLayout lnSelectType;
        private string shortName;
        ImageButton btnCustomertype;
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.addcustomer_activity_main);

                addCustomer = this;

                lnDetails = FindViewById<LinearLayout>(Resource.Id.lnDetails);
                txtbirthdate = FindViewById<TextView>(Resource.Id.txtbirthdate);
                txtzipcode = FindViewById<TextView>(Resource.Id.txtzipcode);
                txtCustomerName = FindViewById<EditText>(Resource.Id.txtCustomerName);
                btnAdd = FindViewById<Button>(Resource.Id.btnAdd);
                lnBack = FindViewById<FrameLayout>(Resource.Id.lnBack);
                lnShowDetail = FindViewById<LinearLayout>(Resource.Id.lnShowDetail);
                lnDelete = FindViewById<FrameLayout>(Resource.Id.lnDelete);
                lnShowDelete = FindViewById<FrameLayout>(Resource.Id.lnShowDelete);
                btnback = FindViewById<ImageButton>(Resource.Id.btnback);
                imggender = FindViewById<ImageButton>(Resource.Id.imggender);
                imgProfile = FindViewById<ImageView>(Resource.Id.imgProfile);
                btnAddImage = FindViewById<ImageButton>(Resource.Id.btnAddImage);
                imgbirthdate = FindViewById<ImageButton>(Resource.Id.imgbirthdate);
                btnShowDetail = FindViewById<ImageButton>(Resource.Id.btnShowDetail);
                btnprovinces = FindViewById<ImageButton>(Resource.Id.btnprovinces);
                btnAmphure = FindViewById<ImageButton>(Resource.Id.btnAmphure);
                btnDistrict = FindViewById<ImageButton>(Resource.Id.btnDistrict);
                txtcustomerid = FindViewById<EditText>(Resource.Id.txtcustomerid);
                txtphonenumber = FindViewById<EditText>(Resource.Id.txtphonenumber);
                txtemail = FindViewById<EditText>(Resource.Id.txtemail);
                txtLine = FindViewById<EditText>(Resource.Id.txtLine);
                txtnationalID = FindViewById<EditText>(Resource.Id.txtnationalID);
                txtaddress = FindViewById<EditText>(Resource.Id.txtaddress);
                txtcomment = FindViewById<EditText>(Resource.Id.txtcomment);
                txtlinkpromaxx = FindViewById<EditText>(Resource.Id.txtlinkpromaxx);
                txtShortName = FindViewById<EditText>(Resource.Id.txtShortName);
                txtNameView = FindViewById<TextView>(Resource.Id.txtNameView);
                txtTitle = FindViewById<TextView>(Resource.Id.txtTitle);
                lnSelectType = FindViewById<LinearLayout>(Resource.Id.lnSelectType);
                spinnergender = FindViewById<Spinner>(Resource.Id.spinnergender);
                spinnerProvince = FindViewById<Spinner>(Resource.Id.spinnerProvince);
                spinneramphures = FindViewById<Spinner>(Resource.Id.spinneramphures);
                spinnerDistrict = FindViewById<Spinner>(Resource.Id.spinnerDistrict);
                spinnerCustomerType = FindViewById<Spinner>(Resource.Id.spinnerCustomerType);
                btnCustomertype = FindViewById<ImageButton>(Resource.Id.btnCustomertype);
                lnSelectType.Click += LnSelectType_Click;
                btnback.Click += Btnback_Click;
                lnBack.Click += Btnback_Click;
                imgProfile.Click += ImgProfile_Click;
                btnAddImage.Click += BtnAddImage_Click;
                txtCustomerName.TextChanged += TxtCustomerName_TextChanged;
                //txtCustomerName.KeyPress += TxtCustomerName_KeyPress;
                txtcustomerid.TextChanged += Txtcustomerid_TextChanged;
                txtbirthdate.Click += Txtbirthdate_Click;
                imgbirthdate.Click += Imgbirthdate_Click;
                btnShowDetail.Click += BtnShowDetail_Click;
                lnShowDetail.Click += BtnShowDetail_Click;
                btnprovinces.Click += Btnprovinces_Click;
                btnAmphure.Click += BtnAmphure_Click;
                btnDistrict.Click += Btnsubdustrict_Click;
                imggender.Click += Imggender_Click;
                txtphonenumber.TextChanged += Txtphonenumber_TextChanged;
                txtnationalID.TextChanged += TxtnationalID_TextChanged1;
                txtNoCustomerType = FindViewById<TextView>(Resource.Id.txtNoCustomerType);
                txtNoCustomerType.Click += TxtNoCustomerType_Click;

                CheckPermission();
                CheckJwt();

                usernamelogin = Preferences.Get("User", "");
                Language = Preferences.Get("Language", "");
                CurrentActivity = true;
                showdetail = false;
                first = true;
                ShowDetail();

                //Birthdate
                DateTime dateTime = DateTime.Today;

                txtbirthdate.Visibility = ViewStates.Gone;
                txtbirthdate.Text = dateTime.ToString("dd/MM/yyyy", new CultureInfo("en-US"));
#pragma warning disable CS0618 // Type or member is obsolete
                dialogBirthdate = new DatePickerDialog(this, Android.Resource.Style.ThemeHoloLightDialogMinWidth, DatePickerDialog_BirthDate,
                                                         dateTime.Year,
                                                         dateTime.Month - 1,
                                                         dateTime.Day);
#pragma warning restore CS0618 // Type or member is obsolete

                //gender
                spinnergender.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinnergender_ItemSelected);
                var adapter = ArrayAdapter.CreateFromResource(this, Resource.Array.spinGender, Resource.Layout.spinner_item);
                adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinnergender.Adapter = adapter;

                Android.Content.Res.Resources res = this.Resources;
                string select = res.GetString(Resource.String.addcustomer_activity_selectzipcode);

                List<string> items = new List<string>();
                zipcode = new List<string>();
                string temp = "0";
                string temp2 = select;
                items.Add(temp2);
                zipcode.Add(temp);

                Gender = 'N'; //default none

                Provinces = new List<string>();
                Amphures = new List<string>();
                District = new List<string>();
                CustomerType = new List<string>();

                pathThumnailFolder = DataCashingAll.PathThumnailFolderImage;
                pathFolderPicture = DataCashingAll.PathFolderImage;

                txtemail.TextChanged += Txtemail_TextChanged;
                txtLine.TextChanged += TxtLine_TextChanged;
                txtnationalID.TextChanged += TxtnationalID_TextChanged;
                txtaddress.TextChanged += Txtaddress_TextChanged;
                txtcomment.TextChanged += Txtcomment_TextChanged;
                txtlinkpromaxx.TextChanged += Txtlinkpromaxx_TextChanged;
                txtShortName.TextChanged += TxtShortName_TextChanged;

                SelectCutomerType();
                SelectProvince();

                LoginType = Preferences.Get("LoginType", "");               

                if (iSysCustomerID == 0)
                {
                    txtTitle.Text = GetString(Resource.String.addcustomer_activity_addcustomer);
                    btnAdd.Text = GetString(Resource.String.addcustomer_activity_addcustomer);
                    imgProfile.SetBackgroundResource(Resource.Mipmap.defaultcustL);
                    btnAdd.Click += BtnAdd_Click;
                    lnShowDelete.Visibility = ViewStates.Gone;                    
                }
                else
                {
                    txtTitle.Text = GetString(Resource.String.addcustomer_activity_editcustomer);
                    btnAdd.Text = GetString(Resource.String.textsave);
                    await GetDetailCustomer();
                    btnAdd.Click += BtnEdit_Click;
                    lnShowDelete.Visibility = ViewStates.Visible;
                    lnDelete.Click += LnDelete_Click;                    
                }

                CheckDataChange();
                SetButtonAdd(false);
                SetUIFromMainRole(LoginType);
                first = false;
                _ = TinyInsights.TrackPageViewAsync("OnCreate : AddCustomerActivity");

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("OnCreate at add Customer");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private void SetUIFromMainRole(string loginType)
        {
            bool check = UtilsAll.CheckPermissionRoleUser(loginType, "insert", "customer");
            if (check)
            {
                imgProfile.Enabled = true;
                btnAddImage.Enabled = true;
                btnAddImage.Visibility = ViewStates.Visible;
                txtCustomerName.Enabled = true;
                txtCustomerName.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtCustomerName.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                txtphonenumber.Enabled = true;
                txtphonenumber.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtphonenumber.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                spinnerCustomerType.Enabled = true;
                lnSelectType.Enabled = true;
                btnCustomertype.Enabled = true;
                btnCustomertype.SetBackgroundResource(Resource.Mipmap.Next);
                txtcustomerid.Enabled = true;
                txtcustomerid.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtcustomerid.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                txtShortName.Enabled = true;
                txtShortName.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtShortName.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                txtnationalID.Enabled = true;
                txtnationalID.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtnationalID.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                txtbirthdate.Enabled = true;
                txtbirthdate.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtbirthdate.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                imgbirthdate.Enabled = true;
                imgbirthdate.SetBackgroundResource(Resource.Mipmap.Next);
                spinnergender.Enabled = true;
                imggender.Enabled = true;
                imggender.SetBackgroundResource(Resource.Mipmap.Next);
                txtemail.Enabled = true;
                txtemail.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtemail.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                txtLine.Enabled = true;
                txtLine.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtLine.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                txtaddress.Enabled = true;
                txtaddress.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtaddress.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                spinnerProvince.Enabled = true;
                btnprovinces.Enabled = true;
                btnprovinces.SetBackgroundResource(Resource.Mipmap.Next);
                spinneramphures.Enabled = true;
                btnAmphure.Enabled = true;
                btnAmphure.SetBackgroundResource(Resource.Mipmap.Next);
                spinnerDistrict.Enabled = true;
                btnDistrict.Enabled = true;
                btnDistrict.SetBackgroundResource(Resource.Mipmap.Next);
                txtzipcode.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtzipcode.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                txtcomment.Enabled = true;
                txtcomment.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtcomment.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
                btnAdd.Enabled = true;
                btnAdd.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnAdd.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtlinkpromaxx.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
                txtlinkpromaxx.SetHintTextColor(Resources.GetColor(Resource.Color.texthintcolor, null));
            }
            else
            {
                imgProfile.Enabled = false;
                btnAddImage.Enabled = false;
                btnAddImage.Visibility = ViewStates.Invisible;
                txtCustomerName.Enabled = false;
                txtCustomerName.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtCustomerName.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtphonenumber.Enabled = false;
                txtphonenumber.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtphonenumber.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                spinnerCustomerType.Enabled = false;
                lnSelectType.Enabled = false;
                btnCustomertype.Enabled = false;
                btnCustomertype.SetBackgroundResource(Resource.Mipmap.NextG);
                txtcustomerid.Enabled = false;
                txtcustomerid.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtcustomerid.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtShortName.Enabled = false;
                txtShortName.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtShortName.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtnationalID.Enabled = false;
                txtnationalID.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtnationalID.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtbirthdate.Enabled = false;
                txtbirthdate.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtbirthdate.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                imgbirthdate.Enabled = false;
                imgbirthdate.SetBackgroundResource(Resource.Mipmap.NextG);
                spinnergender.Enabled = false;
                imggender.Enabled = false;
                imggender.SetBackgroundResource(Resource.Mipmap.NextG);
                txtemail.Enabled = false;
                txtemail.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtemail.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtLine.Enabled = false;
                txtLine.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtLine.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtaddress.Enabled = false;
                txtaddress.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtaddress.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                spinnerProvince.Enabled = false;
                btnprovinces.Enabled = false;
                btnprovinces.SetBackgroundResource(Resource.Mipmap.NextG);
                spinneramphures.Enabled = false;
                btnAmphure.Enabled = false;
                btnAmphure.SetBackgroundResource(Resource.Mipmap.NextG);
                spinnerDistrict.Enabled = false;
                btnDistrict.Enabled = false;
                btnDistrict.SetBackgroundResource(Resource.Mipmap.NextG);
                txtzipcode.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtzipcode.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtcomment.Enabled = false;
                txtcomment.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtcomment.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                btnAdd.Enabled = false;
                btnAdd.SetBackgroundResource(Resource.Drawable.btnbordergray);
                btnAdd.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtlinkpromaxx.SetTextColor(Resources.GetColor(Resource.Color.textblacklightcolor, null));
                txtlinkpromaxx.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
            }
            check = UtilsAll.CheckPermissionRoleUser(loginType, "delete", "customer");
            if (check && iSysCustomerID > 0)
            {
                lnShowDelete.Visibility = ViewStates.Visible;
            }
            else
            {
                lnShowDelete.Visibility = ViewStates.Gone;
            }
        }

        //private void TxtCustomerName_KeyPress(object sender, View.KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keycode.Del)
        //        {
        //            e.Handled = true;
        //            if (string.IsNullOrEmpty(txtCustomerName.Text) && txtShortName.Text.Length == 1)
        //            {
        //                txtShortName.Text = string.Empty;
        //            }
        //        }
        //        else
        //        {
        //            e.Handled = false; //if you want that character appeared on the screen
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _ = TinyInsights.TrackErrorAsync(ex);
        //        _ = TinyInsights.TrackPageViewAsync("TxtCustomerName_KeyPress at add Customer");
        //        Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
        //    }
        //}

        private void LnSelectType_Click(object sender, EventArgs e)
        {
            try
            {
                if (DataCashing.Membertype == null || DataCashing.Membertype.Count > 1)
                {
                    spinnerCustomerType.PerformClick();
                }
                else
                {
                    txtNoCustomerType.PerformClick();
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("LnSelectType_Click at add Customer");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private void CheckDataChange()
        {
            try
            {
                if (first)
                {
                    SetButtonAdd(false);
                    return;
                }
                if (customerEdit == null)
                {
                    if (!string.IsNullOrEmpty(txtCustomerName.Text))
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }

                    SetButtonAdd(false);
                    flagdatachange = false;
                    return;
                }
                else
                {
                    if (txtCustomerName.Text != customerEdit.CustomerName)
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }                    

                    string CusEdit =  string.IsNullOrEmpty(customerEdit.CustomerID) ?  "" : customerEdit.CustomerID;
                    string CusTxt = string.IsNullOrEmpty(txtcustomerid.Text) ? "" : txtcustomerid.Text;
                    if (CusEdit != CusTxt)
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }
                    if (txtphonenumber.Text != addTextTel(customerEdit.Mobile))
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }
                    if (txtemail.Text != customerEdit.EMail)
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }
                    if (txtLine.Text != customerEdit.LineID)
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }
                    var tnationalID = txtnationalID.Text.Replace(" ", "");
                    var cusid = "";
                    if (!string.IsNullOrEmpty(customerEdit.IDCard))
                    {
                        cusid = customerEdit.IDCard?.ToString();
                        cusid = cusid.Replace(" ", "");
                    }
                    if (tnationalID != cusid)
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }
                    if (txtaddress.Text != customerEdit.Address)
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }
                    if (txtcomment.Text != customerEdit.Comments)
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }
                    if (txtlinkpromaxx.Text != customerEdit.LinkProMaxxID)
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }
                    if (txtShortName.Text != customerEdit.ShortName)
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }
                    if (Gender != customerEdit.Gender)
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }
                    int.TryParse(customerEdit.ProvincesId?.ToString(), out int provintid);
                    if (ProvincesId != provintid)
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }
                    int.TryParse(customerEdit.AmphuresId?.ToString(), out int amphureid);
                    if (AmphuresId != amphureid)
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }
                    int.TryParse(customerEdit.DistrictsId?.ToString(), out int districtsid);
                    if (DistrictsId != districtsid)
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

                    int.TryParse(customerEdit.MemberTypeNo?.ToString(), out int memberTypeNo);
                    if (CustomerTypeNo != memberTypeNo)
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }


                    DateTime? Birth = Utils.BirthDayBE(customerEdit.BirthDate ?? DateTime.UtcNow);
                    if (txtbirthdate.Text != string.Format("{0:00}/{1:00}/{2:0000}", Birth.Value.Day, Birth.Value.Month, Birth.Value.Year))
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }

                    flagdatachange = false;
                    SetButtonAdd(false);
                    SetUIFromMainRole(LoginType);


                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CheckDataChange at add Customer");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private void TxtShortName_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                if (shortName.Length == 1 && string.IsNullOrEmpty(txtShortName.Text.Trim()))
                {
                    shortName = "";
                    txtNameView.Text = CustomerName;
                    return;
                }
                shortName = txtShortName.Text.Trim();
                if (string.IsNullOrEmpty(shortName))
                {
                    if (txtCustomerName.Text.Length > 7)
                    {
                        txtShortName.Text = txtCustomerName.Text.Trim().Substring(0, 7);
                    }
                    else
                    {
                        txtShortName.Text = txtCustomerName.Text.Trim();
                    }
                }
                else
                {
                    var textName = txtCustomerName.Text;
                    if (textName.Length > 7)
                    {
                        textName = txtCustomerName.Text.Trim().Substring(0, 7);
                    }
                    else
                    {
                        textName = txtCustomerName.Text.Trim();
                    }
                    if (shortName != textName)
                    {
                        txtNameView.Text = shortName;
                    }
                }
                CheckDataChange();
            }
            catch (Exception)
            {
                return;
            }

        }

        private void Txtlinkpromaxx_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
        }

        private void TxtnationalID_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
        }

        private void Txtcomment_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
        }

        private void Txtaddress_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
        }

        private void TxtLine_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
        }

        private void Txtemail_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckDataChange();
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

        private async void TxtNoCustomerType_Click(object sender, EventArgs e)
        {
            try
            {
                MainDialog dialog = new MainDialog();
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                bundle.PutString("message", myMessage);
                bundle.PutString("NoCustomerType", "CustomerType");
                dialog.Arguments = bundle;
                dialog.Show(SupportFragmentManager, myMessage);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("TxtNoCustomerType_Click at add Customer");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void LnDelete_Click(object sender, EventArgs e)
        {
            try
            {

                bool check = UtilsAll.CheckPermissionRoleUser(LoginType, "delete", "customer");
                if (!check)
                {
                    Toast.MakeText(this, GetString(Resource.String.notperm), ToastLength.Short).Show();
                    return;
                }

                MainDialog dialog = new MainDialog();
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                bundle.PutString("message", myMessage);
                bundle.PutInt("systemID", (int)iSysCustomerID);
                bundle.PutString("deleteType", "customer");
                dialog.Arguments = bundle;
                dialog.Show(SupportFragmentManager, myMessage);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("LnDelete_Click at add Customer");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public static void setCustomer(long SysCustomerID)
        {
            iSysCustomerID = SysCustomerID;
        }

        protected async override void OnResume()
        {
            try
            {
                base.OnResume();                
                TokenResult res = await TokenServiceBase.GetToken();
                if (!res.status)
                {
                    StartActivity(new Android.Content.Intent(Application.Context, typeof(LoginActivity)));
                    this.Finish();
                    return;
                }
            }
            catch (Exception)
            {
                base.OnRestart();
            }
        }

        public override void OnBackPressed()
        {
            try
            {
                if (iSysCustomerID == 0)
                {
                    if (!flagdatachange)
                    {
                        DialogCheckBack(); return;
                    }

                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.add_dialog_back.ToString();
                    bundle.PutString("message", myMessage);
                    bundle.PutString("fromPage", "customer");
                    dialog.Arguments = bundle;
                    dialog.Show(SupportFragmentManager, myMessage);
                    return;
                }
                else
                {
                    if (!flagdatachange)
                    {
                        DialogCheckBack(); 
                        return;
                    }

                    MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.edit_dialog_back.ToString();
                    bundle.PutString("message", myMessage);
                    bundle.PutString("fromPage", "customer");
                    bundle.PutString("PassValue", iSysCustomerID.ToString());
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

        private void Btnback_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        public void DialogCheckBack()
        {
            base.OnBackPressed();
            flagdatachange = false;
            iSysCustomerID = 0;
        }

        private void Txtphonenumber_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                Phone = txtphonenumber.Text;
                int textlength = txtphonenumber.Text.Length;

                if (Phone.EndsWith(" "))
                    return;

                if (Phone.StartsWith("02"))
                {
                    if (Phone.Length == 12)
                    {
                        Phone = Phone.Remove(Phone.Length - 1);
                        txtphonenumber.Text = Phone;
                        txtphonenumber.SetSelection(txtphonenumber.Text.Length);
                    }
                    if (textlength == 3)
                    {
                        var index = txtphonenumber.Text.LastIndexOf("-");
                        if (textlength == 3 & index == 2)
                        {
                            Phone.Remove(2, 1);
                        }
                        else
                        {
                            txtphonenumber.Text = Phone.Insert(Phone.Length - 1, "-").ToString();
                            txtphonenumber.SetSelection(txtphonenumber.Text.Length);
                        }
                    }
                    else if (textlength == 7)
                    {
                        var index = txtphonenumber.Text.LastIndexOf("-");
                        if (textlength == 7 & index == 6)
                        {
                            Phone.Remove(6, 1);
                        }
                        else
                        {
                            txtphonenumber.Text = Phone.Insert(Phone.Length - 1, "-").ToString();
                            txtphonenumber.SetSelection(txtphonenumber.Text.Length);
                        }
                    }
                }
                else if (Phone.StartsWith("03") | Phone.StartsWith("04") | Phone.StartsWith("05") | Phone.StartsWith("07"))
                {
                    if (Phone.Length == 12)
                    {
                        Phone = Phone.Remove(Phone.Length - 1);
                        txtphonenumber.Text = Phone;
                        txtphonenumber.SetSelection(txtphonenumber.Text.Length);
                    }
                    if (textlength == 4)
                    {
                        var index = txtphonenumber.Text.LastIndexOf("-");
                        if (textlength == 4 & index == 3)
                        {
                            Phone.Remove(3, 1);
                        }
                        else
                        {
                            txtphonenumber.Text = Phone.Insert(Phone.Length - 1, "-").ToString();
                            txtphonenumber.SetSelection(txtphonenumber.Text.Length);
                        }
                    }
                    else if (textlength == 8)
                    {
                        var index = txtphonenumber.Text.LastIndexOf("-");
                        if (textlength == 8 & index == 7)
                        {
                            Phone.Remove(7, 1);
                        }
                        else
                        {
                            txtphonenumber.Text = Phone.Insert(Phone.Length - 1, "-").ToString();
                            txtphonenumber.SetSelection(txtphonenumber.Text.Length);
                        }
                    }
                }
                else
                {
                    if (textlength == 4)
                    {
                        var index = txtphonenumber.Text.LastIndexOf("-");
                        if (textlength == 4 & index == 3)
                        {
                            Phone.Remove(3, 1);
                        }
                        else
                        {
                            txtphonenumber.Text = Phone.Insert(Phone.Length - 1, "-").ToString();
                            txtphonenumber.SetSelection(txtphonenumber.Text.Length);
                        }
                    }
                    else if (textlength == 8)
                    {
                        var index = txtphonenumber.Text.LastIndexOf("-");
                        if (textlength == 8 & index == 7)
                        {
                            Phone.Remove(7, 1);
                        }
                        else
                        {
                            txtphonenumber.Text = Phone.Insert(Phone.Length - 1, "-").ToString();
                            txtphonenumber.SetSelection(txtphonenumber.Text.Length);
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

        private void TxtnationalID_TextChanged1(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                idcard = txtnationalID.Text;
                int textlength = txtnationalID.Text.Length;

                if (Phone.EndsWith(" "))
                    return;

                if (textlength == 2)
                {
                    var index = txtnationalID.Text.LastIndexOf(" ");
                    if (textlength == 2 & index == 1)
                    {
                        idcard.Remove(1, 1);
                    }
                    else
                    {
                        txtnationalID.Text = idcard.Insert(idcard.Length - 1, " ").ToString();
                        txtnationalID.SetSelection(txtnationalID.Text.Length);
                    }
                }
                else if (textlength == 7)
                {
                    var index = txtnationalID.Text.LastIndexOf(" ");
                    if (textlength == 7 & index == 6)
                    {
                        idcard.Remove(6, 1);
                    }
                    else
                    {
                        txtnationalID.Text = idcard.Insert(idcard.Length - 1, " ").ToString();
                        txtnationalID.SetSelection(txtnationalID.Text.Length);
                    }
                }
                else if (textlength == 13)
                {
                    var index = txtnationalID.Text.LastIndexOf(" ");
                    if (textlength == 13 & index == 12)
                    {
                        idcard.Remove(12, 1);
                    }
                    else
                    {
                        txtnationalID.Text = idcard.Insert(idcard.Length - 1, " ").ToString();
                        txtnationalID.SetSelection(txtnationalID.Text.Length);
                    }
                }
                else if (textlength == 16)
                {
                    var index = txtnationalID.Text.LastIndexOf(" ");
                    if (textlength == 16 & index == 15)
                    {
                        idcard.Remove(15, 1);
                    }
                    else
                    {
                        txtnationalID.Text = idcard.Insert(idcard.Length - 1, " ").ToString();
                        txtnationalID.SetSelection(txtnationalID.Text.Length);
                    }
                }
                CheckDataChange();
            }
            catch (Exception)
            {
                return;
            }
        }

        private void Txtcustomerid_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CustomerID = txtcustomerid.Text.Trim();
            CheckDataChange();
        }

        private void Imggender_Click(object sender, EventArgs e)
        {
            spinnergender.PerformClick();
        }

        private void Btnsubdustrict_Click(object sender, EventArgs e)
        {
            spinnerDistrict.PerformClick();
        }

        private void BtnAmphure_Click(object sender, EventArgs e)
        {
            spinneramphures.PerformClick();
        }

        private void Btnprovinces_Click(object sender, EventArgs e)
        {
            spinnerProvince.PerformClick();
        }

        private void BtnShowDetail_Click(object sender, EventArgs e)
        {
            if (showdetail)
            {
                showdetail = false;
            }
            else
            {
                showdetail = true;
            }
            ShowDetail();
            SetUIFromMainRole(LoginType);
        }

        private void ShowDetail()
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

        private void Imgbirthdate_Click(object sender, EventArgs e)
        {
            dialogBirthdate.Show();
        }

        private void Txtbirthdate_Click(object sender, EventArgs e)
        {
            dialogBirthdate.Show();
        }
        string cusName = "", temp;

        private void TxtCustomerName_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                CustomerName = txtCustomerName.Text.Trim();
                if (string.IsNullOrEmpty(txtCustomerName.Text))
                {
                    if (cusName == txtShortName.Text)
                    {
                        txtShortName.Text = String.Empty;
                    }
                    CheckDataChange();
                    return;
                }
                if (!string.IsNullOrEmpty(txtCustomerName.Text))
                {
                    //Show Picture Customer 
                    if (customerEdit != null)
                    {
                        var path = customerEdit.ThumbnailLocalPath;
                        if (!string.IsNullOrEmpty(path))
                        {
                            txtNameView.Visibility = ViewStates.Invisible;
                            Android.Net.Uri uri = Android.Net.Uri.Parse(path);
                            imgProfile.SetImageURI(uri);
                            //txtTitle.Text = getcustomer.CustomerName;
                        }
                        else
                        {
                            txtNameView.Visibility = ViewStates.Visible;
                            imgProfile.SetBackgroundColor(Android.Graphics.Color.ParseColor("#E2E2E2"));
                        }
                    }
                    else
                    {
                        txtNameView.Visibility = ViewStates.Visible;
                        imgProfile.SetBackgroundColor(Android.Graphics.Color.ParseColor("#E2E2E2"));
                    }
                }
                shortName = txtShortName.Text;
                if (txtCustomerName.Text.Length > 7)
                {
                    cusName = txtCustomerName.Text.Substring(0, 7);
                }
                else
                {
                    cusName = txtCustomerName.Text;
                }

                temp = cusName;
                if (cusName.Length > shortName.Length)
                {
                    cusName = cusName.Substring(0, shortName.Length);
                }
                else
                {
                    shortName = shortName.Substring(0, cusName.Length);
                }
                if (cusName == shortName)
                {
                    txtShortName.Text = temp;
                }


                txtNameView.Text = txtShortName.Text?.ToString();

                CheckDataChange();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("TxtCustomerName_TextChanged at add Customer");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void ImgProfile_Click(object sender, EventArgs e)
        {
            try
            {
                string path = "";

                //การแก้ไขข้อมูล , พาธรูปไม่ว่าง , มีอินเตอร์เน็ต
                if (iSysCustomerID != 0 && !string.IsNullOrEmpty(customerEdit.PicturePath) && await GabanaAPI.CheckSpeedConnection())
                {
                    //MainDialog dialog = new MainDialog();
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.dialog_item.ToString();
                    bundle.PutString("message", myMessage);
                    if (!string.IsNullOrEmpty(customerEdit.PicturePath))
                    {
                        bundle.PutString("OpenCloudPicture", customerEdit.PicturePath);
                        path = customerEdit.PicturePath;
                    }
                    //dialog.Arguments = bundle;
                    //dialog.Show(SupportFragmentManager, myMessage);

                    Show_Dialog_Customer dialog_Item = Show_Dialog_Customer.NewInstance(path);
                    dialog_Item.Show(SupportFragmentManager, myMessage);

                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ImgProfile_Click at add Customer");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void BtnAddImage_Click(object sender, EventArgs e)
        {
            try
            {
                MainDialog dialog = new MainDialog();
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.addcustomer_dialog_addimage.ToString();
                bundle.PutString("message", myMessage);
                bundle.PutString("OpenPicture", "customer");
                dialog.Arguments = bundle;
                dialog.Show(SupportFragmentManager, myMessage);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ImgProfile_Click at add Customer");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void spinnergender_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            string select = spinnergender.SelectedItem.ToString();
            if (select == "None" | select == "ไม่ระบุ")
            {
                Gender = 'N';
            }
            else if (select == "Male" | select == "ชาย")
            {
                Gender = 'M';
            }
            else if (select == "Female" | select == "หญิง")
            {
                Gender = 'F';
            }
            CheckDataChange();
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

                if (customerEdit?.ProvincesId == ProvincesId)
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
                    if (ProvincesId != customerEdit?.ProvincesId)
                    {
                        AmphuresId = 0;
                        DistrictsId = 0;
                    }
                }

                int position = GetProvinces.FindIndex(x => x.ProvincesId == ProvincesId);
                spinnerProvince.SetSelection(position);
                SelectAmphures();

                CheckDataChange();
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

                if (customerEdit?.AmphuresId == AmphuresId)
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
                    if (AmphuresId != customerEdit?.AmphuresId)
                    {
                        DistrictsId = 0;
                    }
                }

                int position = GetAmphures.FindIndex(x => x.AmphuresId == AmphuresId);
                spinneramphures.SetSelection(position);
                SelectDistrict();
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
                string select = spinnerDistrict.SelectedItem.ToString();
                if (DataCashing.Districts == null)
                {
                    return;
                }

                if (customerEdit?.DistrictsId == DistrictsId)
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
                else
                {
                    txtzipcode.Text = string.Empty;
                }

                CheckDataChange();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private void spinnerCustomerType_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            try
            {
                string select = spinnerCustomerType.SelectedItem.ToString();

                if (DataCashing.Membertype == null)
                {
                    return;
                }
                CustomerTypeNo = int.Parse(DataCashing.Membertype[e.Position]);

                CheckDataChange();

            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private void DatePickerDialog_BirthDate(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            txtbirthdate.Visibility = ViewStates.Visible;
            txtbirthdate.Text = e.Date.ToString("dd/MM/yyyy", new CultureInfo("en-US"));
            CheckDataChange();
        }

        #region Spinner
        //-------------------------------------------------------------------------
        //Spinner
        //--------------------------------------------------------------------------
        public async void SelectProvince()
        {
            try
            {
                string temp = "";
                string temp2 = "";
                List<string> items = new List<string>();
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
                SelectAmphures();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SelectProvince at add Customer");
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
                List<Amphure> lstamphures = new List<Amphure>();
                List<Amphure> getallamphures = new List<Amphure>();

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
                SelectDistrict();


            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SelectAmphures at add Customer");
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
                var adapterspinnerdistrict = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, items);
                adapterspinnerdistrict.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinnerDistrict.Adapter = adapterspinnerdistrict;
                int position = GetDistricts.FindIndex(x => x.DistrictsId == DistrictsId);
                spinnerDistrict.SetSelection(position);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SelectDistrict at add Customer");
                Log.Debug("error", ex.Message);
            }
        }

        public async void SelectCutomerType()
        {
            try
            {
                string temp = "";
                string temp2 = "";
                List<string> items = new List<string>();
                CustomerType = new List<string>();
                MemberType memberType = new MemberType();
                var lstmembertype = new List<MemberType>();
                var getallmembertype = new List<MemberType>();

                MemberTypeManage memberTypeManage = new MemberTypeManage();
                getallmembertype = await memberTypeManage.GetAllMemberType(DataCashingAll.MerchantId);
                //getallmembertype = new List<MemberType>();
                if (getallmembertype?.Count == 0)
                {
                    memberType = new MemberType()
                    {
                        MemberTypeNo = 0,
                        MemberTypeName = GetString(Resource.String.addcustomer_activity_nocustomertype)
                    };
                    txtNoCustomerType.Visibility = ViewStates.Visible;
                    spinnerCustomerType.Visibility = ViewStates.Gone;
                }
                else
                {
                    txtNoCustomerType.Visibility = ViewStates.Gone;
                    spinnerCustomerType.Visibility = ViewStates.Visible;
                    memberType = new MemberType()
                    {
                        MemberTypeNo = 0,
                        MemberTypeName = GetString(Resource.String.addcustomer_activity_customertype)
                    };
                }

                lstmembertype.Add(memberType);
                lstmembertype.AddRange(getallmembertype);

                for (int i = 0; i < lstmembertype.Count; i++)
                {
                    temp = lstmembertype[i].MemberTypeNo.ToString();
                    temp2 = lstmembertype[i].MemberTypeName.ToString();
                    items.Add(temp2);
                    CustomerType.Add(temp);
                }

                DataCashing.Membertype = CustomerType;
                //subdistrict
                spinnerCustomerType.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinnerCustomerType_ItemSelected);
                var adapterspinnercustomertype = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, items);
                adapterspinnercustomertype.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinnerCustomerType.Adapter = adapterspinnercustomertype;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SelectCutomerType at add Customer");
                Log.Debug("error", ex.Message);
            }
        }
        #endregion

        #region Picture
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
                GalIntent.SetType("image/*");
                StartActivityForResult(Intent.CreateChooser(GalIntent, "Select image from gallery"), GALLERY_PICTURE);
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("GalleryOpen at add Customer");
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
                CropIntent.PutExtra("outputX", 400);
                CropIntent.PutExtra("outputY", 400);
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

                CropIntent.PutExtra(MediaStore.ExtraOutput, cropedUri);
                StartActivityForResult(CropIntent, CROP_REQUEST);
                CheckDataChange();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("CropImage at add Customer");
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
                _ = TinyInsights.TrackPageViewAsync("CameraTakePicture at add Customer");
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

                        //if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
                        //{
                        //    keepCropedUri = selectPictureUri;
                        //    CheckDataChange();

                        //    Android.Net.Uri cropImageURI = keepCropedUri;
                        //    bitmap = Android.Provider.MediaStore.Images.Media.GetBitmap(Application.Context.ContentResolver, cropImageURI);
                        //    imgProfile.SetImageBitmap(bitmap);
                        //    txtNameView.Visibility = ViewStates.Gone;
                        //}
                        //else
                        //{
                            CropImage(selectPictureUri);
                        //}
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
                        bitmap = Android.Provider.MediaStore.Images.Media.GetBitmap(Application.Context.ContentResolver, cropImageURI);

                        // Solution 3 : อ่าน BitMap จากไฟล์ โดยเอาค่าไฟล์จาก bundle.GetParcelable(MediaStore.ExtraOutput) : จะ error กับ Andord ที่ต่ำกว่า Android.N
                        //Android.Net.Uri cropImageURI = (Android.Net.Uri)bundle.GetParcelable(MediaStore.ExtraOutput); // ใช้กับ Andord ที่ต่ำกว่า Android.N ไม่ได้ เพราะจะมีค่าเป็น Null
                        //Bitmap bitmap = Android.Provider.MediaStore.Images.Media.GetBitmap(Application.Context.ContentResolver, cropImageURI);

                        imgProfile.SetImageBitmap(bitmap);
                        txtNameView.Visibility = ViewStates.Gone;

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
                _ = TinyInsights.TrackPageViewAsync("OnActivityResult at add Customer");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        #endregion

        //-----------------------------------------------------------------------------
        //Insert and Update
        //-----------------------------------------------------------------------------

        private async void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                btnAdd.Enabled = false;
                var birth = txtbirthdate.Text;
                //Name
                if (string.IsNullOrEmpty(CustomerName))
                {
                    Toast.MakeText(this, GetString(Resource.String.usernamenotcomplete), ToastLength.Short).Show();
                    btnAdd.Enabled = true;
                    return;
                }

                CultureInfo culture = new CultureInfo("en-US");

                DateTime dateTime = new DateTime();
                dateTime = DateTime.UtcNow;
                string datenow = dateTime.ToString("dd/MM/yyyy", new CultureInfo("en-US"));
                birthdate = DateTime.ParseExact(birth, "dd/MM/yyyy", culture);

                if (birthdate > dateTime)
                {
                    Toast.MakeText(this, GetString(Resource.String.birthdaynotcomplete), ToastLength.Short).Show();
                    btnAdd.Enabled = true;
                    return;
                }

                if (birthdate.Value.Year == dateTime.Year & birthdate.Value.Month == dateTime.Month & birthdate.Value.Day == dateTime.Day)
                {
                    birthdate = null;
                }

                //get local SystemSeqNo
                int systemSeqNo = await deviceSystemSeqNoManage.GetLastDeviceSystemSeqNo(DataCashingAll.MerchantId, DataCashingAll.DeviceNo, 50);
                var sys = DataCashingAll.DeviceNo + (systemSeqNo + 1).ToString("D6");

                Gabana.ORM.MerchantDB.Customer customer = new Customer();

                if (keepCropedUri != null)
                {
                    path = Utils.SplitPath(keepCropedUri.ToString());
                    var checkResult = await Utils.InsertImageToThumbnail(path, bitmap, "customer");
                    if (checkResult)
                    {
                        customer.ThumbnailLocalPath = pathThumnailFolder + path;
                    }

                    var checkResultPicture = await Utils.InsertImageToPicture(path, bitmap);
                    if (checkResultPicture)
                    {
                        customer.PictureLocalPath = pathFolderPicture + path;
                    }

                    //Utils.streamImage(bitmap);
                    customer.PicturePath = keepCropedUri.ToString();
                }
                else
                {
                    customer.ThumbnailLocalPath = null;
                    customer.PictureLocalPath = null;
                    customer.PicturePath = null;
                }

                customer.MerchantID = DataCashingAll.MerchantId;
                customer.SysCustomerID = long.Parse(sys);
                customer.CustomerName = CustomerName;
                customer.Ordinary = 1;
                customer.CustomerID = CustomerID == string.Empty ? null : CustomerID;
                if (CustomerName.Length > 5)
                {
                    customer.ShortName = CustomerName.Substring(0, 5);
                }
                else
                {
                    customer.ShortName = CustomerName;
                }

                if (string.IsNullOrEmpty(txtemail.Text.Trim()))
                {
                    customer.EMail = txtemail.Text.Trim();
                }
                else
                {
                    if (Utils.IsEmail(txtemail.Text.Trim()))
                    {
                        customer.EMail = txtemail.Text.Trim();
                    }
                    else
                    {
                        Toast.MakeText(this, GetString(Resource.String.emailnotcomplete), ToastLength.Short).Show();
                        btnAdd.Enabled = true;
                        return;
                    }
                }

                customer.LineID = txtLine.Text.Trim();

                if (string.IsNullOrEmpty(txtphonenumber.Text))
                {
                    customer.Mobile = null;
                }
                else
                {
                    if (txtphonenumber.Text.StartsWith("02") | txtphonenumber.Text.StartsWith("03") | txtphonenumber.Text.StartsWith("04") | txtphonenumber.Text.StartsWith("05") | txtphonenumber.Text.StartsWith("07"))
                    {
                        if (txtphonenumber.Text.Replace("-", "").Length < 9)
                        {
                            Toast.MakeText(this, GetString(Resource.String.telnotcomplete), ToastLength.Short).Show();
                            btnAdd.Enabled = true;
                            return;
                        }
                        else
                        {
                            customer.Mobile = txtphonenumber.Text.Replace("-", "");
                        }
                    }
                    else
                    {
                        if (txtphonenumber.Text.Replace("-", "").Length < 10)
                        {
                            Toast.MakeText(this, GetString(Resource.String.telnotcomplete), ToastLength.Short).Show();
                            btnAdd.Enabled = true;
                            return;
                        }
                        else
                        {
                            customer.Mobile = txtphonenumber.Text.Replace("-", "");
                        }
                    }
                }

                customer.Gender = Gender;
                customer.BirthDate = birthdate;
                customer.Address = txtaddress.Text;
                if (ProvincesId == 0 & AmphuresId == 0 & DistrictsId == 0)
                {
                    customer.ProvincesId = null;
                    customer.AmphuresId = null;
                    customer.DistrictsId = null;
                }
                else if (ProvincesId != 0 & AmphuresId == 0 | DistrictsId == 0)
                {
                    if (AmphuresId == 0)
                    {
                        Toast.MakeText(this, GetString(Resource.String.cannotsave) +
                                                GetString(Resource.String.selectdistict), ToastLength.Short).Show();
                        btnAdd.Enabled = true;
                        return;
                    }
                    if (DistrictsId == 0)
                    {
                        Toast.MakeText(this, GetString(Resource.String.cannotsave) +
                                                GetString(Resource.String.selectsubdistict), ToastLength.Short).Show();
                        btnAdd.Enabled = true;
                        return;
                    }
                }
                else
                {
                    customer.ProvincesId = ProvincesId;
                    customer.AmphuresId = AmphuresId;
                    customer.DistrictsId = DistrictsId;
                }
                if (string.IsNullOrEmpty(txtnationalID.Text))
                {
                    customer.IDCard = null;
                }
                else
                {
                    var count = Removespaces(txtnationalID.Text);
                    if (count.Length == 13)
                    {
                        customer.IDCard = count;
                    }
                    else
                    {
                        Toast.MakeText(this, GetString(Resource.String.idcardnotcomplete), ToastLength.Short).Show();
                        btnAdd.Enabled = true;
                        return;
                    }
                }
                customer.Comments = txtcomment.Text;
                customer.LastDateModified = Utils.GetTranDate(DateTime.UtcNow);
                customer.UserLastModified = usernamelogin;
                customer.LinkProMaxxID = txtlinkpromaxx.Text.Trim();
                customer.DataStatus = 'I';
                customer.FWaitSending = 2;
                customer.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);
                customer.MemberTypeNo = CustomerTypeNo == 0 ? (long?)null : CustomerTypeNo;
                customer.ShortName = txtShortName.Text;

                var checkName = await CustomerManage.CheckCustomerName(customer.CustomerName);
                if (checkName)
                {
                    try
                    {
                        btnAdd.Enabled = true;
                        //เพิ่ม json สำหรับไปบันทึกข้อมูลที่ dialog                    
                        var json = JsonConvert.SerializeObject(customer);

                        MainDialog dialog = new MainDialog();
                        Bundle bundle = new Bundle();
                        String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                        bundle.PutString("message", myMessage);
                        bundle.PutString("insertRepeat", "insertcustomer");
                        bundle.PutString("detailnnsert", json);
                        bundle.PutString("event", "insert");
                        bundle.PutString("detailitem", customer.CustomerName);
                        dialog.Arguments = bundle;
                        dialog.Show(SupportFragmentManager, myMessage);
                        return;
                    }
                    catch (Exception ex)
                    {
                        btnAdd.Enabled = true;
                        await TinyInsights.TrackErrorAsync(ex);
                        _ = TinyInsights.TrackPageViewAsync("InsertItem at add customer");
                        Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                        return;
                    }
                }

                var insertCustomer = await CustomerManage.InsertCustomer(customer);
                if (!insertCustomer)
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                    btnAdd.Enabled = true;
                    return;
                }

                Toast.MakeText(this, GetString(Resource.String.savesucess), ToastLength.Short).Show();

                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendCustomer((int)customer.MerchantID, (int)customer.SysCustomerID);
                }
                else
                {
                    customer.FWaitSending = 2;
                    await CustomerManage.UpdateCustomer(customer);
                }

                //CustomerActivity.SetFocusCustomer(customer.SysCustomerID);
                //DataCashingAll.flagCustomerChange = true;

                CustomerActivity.SetFocusCustomer(customer);
                btnAdd.Enabled = true;
                this.Finish();
            }
            catch (Exception ex)
            {
                btnAdd.Enabled = true;
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnAdd_Click at add Customer");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private async void BtnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                btnAdd.Enabled = false;
                var birth = txtbirthdate.Text;
                //Name
                if (string.IsNullOrEmpty(CustomerName))
                {
                    Toast.MakeText(this, GetString(Resource.String.usernamenotcomplete), ToastLength.Short).Show();
                    btnAdd.Enabled = true;
                    return;
                }

                CultureInfo culture = new CultureInfo("en-US");

                DateTime dateTime = new DateTime();
                dateTime = DateTime.UtcNow;
                string datenow = dateTime.ToString("dd/MM/yyyy", new CultureInfo("en-US"));
                var data = DateTime.ParseExact(birth, "dd/MM/yyyy", culture);
                DateTime? birthdate = Utils.GetTranDate(data);
                if (birthdate > dateTime)
                {
                    Toast.MakeText(this, GetString(Resource.String.birthdaynotcomplete), ToastLength.Short).Show();
                    btnAdd.Enabled = true;
                    return;
                }

                if (birthdate == dateTime)
                {
                    birthdate = null;
                }

                Gabana.ORM.MerchantDB.Customer customer = new Customer();

                if (keepCropedUri != null)
                {
                    path = Utils.SplitPath(keepCropedUri.ToString());
                    var checkResult = await Utils.InsertImageToThumbnail(path, bitmap, "customer");
                    if (checkResult)
                    {
                        customer.ThumbnailLocalPath = pathThumnailFolder + path;
                    }

                    var checkResultPicture = await Utils.InsertImageToPicture(path, bitmap);
                    if (checkResultPicture)
                    {
                        customer.PictureLocalPath = pathFolderPicture + path;
                    }

                    if (!string.IsNullOrEmpty(customerEdit.ThumbnailLocalPath))
                    {
                        Java.IO.File imgFile = new Java.IO.File(customerEdit.ThumbnailLocalPath);
                        if (System.IO.File.Exists(imgFile.AbsolutePath))
                        {
                            System.IO.File.Delete(imgFile.AbsolutePath);
                        }
                    }
                    customer.PicturePath = customerEdit.PicturePath;
                }
                else
                {
                    customer.PictureLocalPath = customerEdit.PictureLocalPath;
                    customer.PicturePath = customerEdit.PicturePath;
                    customer.ThumbnailLocalPath = customerEdit.ThumbnailLocalPath;
                    customer.ThumbnailPath = customerEdit.ThumbnailPath;
                }

                customer.MerchantID = customerEdit.MerchantID;
                customer.SysCustomerID = customerEdit.SysCustomerID;
                customer.CustomerName = CustomerName;
                customer.Ordinary = customerEdit.Ordinary;
                customer.CustomerID = CustomerID == string.Empty ? null : CustomerID;
                if (CustomerName.Length > 5)
                {
                    customer.ShortName = CustomerName.Substring(0, 5);
                }
                else
                {
                    customer.ShortName = CustomerName;
                }
                if (string.IsNullOrEmpty(path))
                {
                    customer.PicturePath = customerEdit.PicturePath;
                }
                else
                {
                    customer.PicturePath = path;
                }

                if (string.IsNullOrEmpty(txtemail.Text.Trim()))
                {
                    customer.EMail = txtemail.Text.Trim();
                }
                else
                {
                    if (Utils.IsEmail(txtemail.Text.Trim()))
                    {
                        customer.EMail = txtemail.Text.Trim();
                    }
                    else
                    {
                        Toast.MakeText(this, GetString(Resource.String.emailnotcomplete), ToastLength.Short).Show();
                        btnAdd.Enabled = true;
                        return;
                    }
                }
                customer.LineID = txtLine.Text.Trim();

                if (string.IsNullOrEmpty(txtphonenumber.Text))
                {
                    customer.Mobile = null;
                }
                else
                {
                    if (txtphonenumber.Text.StartsWith("02") | txtphonenumber.Text.StartsWith("03") | txtphonenumber.Text.StartsWith("04") | txtphonenumber.Text.StartsWith("05") | txtphonenumber.Text.StartsWith("07"))
                    {
                        if (txtphonenumber.Text.Replace("-", "").Length < 9)
                        {
                            Toast.MakeText(this, GetString(Resource.String.telnotcomplete), ToastLength.Short).Show();
                            btnAdd.Enabled = true;
                            return;
                        }
                        else
                        {
                            customer.Mobile = txtphonenumber.Text.Replace("-", "");
                        }
                    }
                    else
                    {
                        if (txtphonenumber.Text.Replace("-", "").Length < 10)
                        {
                            Toast.MakeText(this, GetString(Resource.String.telnotcomplete), ToastLength.Short).Show();
                            btnAdd.Enabled = true;
                            return;
                        }
                        else
                        {
                            customer.Mobile = txtphonenumber.Text.Replace("-", "");
                        }
                    }
                }

                customer.Gender = Gender;
                customer.BirthDate = birthdate;
                customer.Address = txtaddress.Text;
                if (ProvincesId == 0 & AmphuresId == 0 & DistrictsId == 0)
                {
                    customer.ProvincesId = customerEdit.ProvincesId;
                    customer.AmphuresId = customerEdit.AmphuresId;
                    customer.DistrictsId = customerEdit.DistrictsId;
                }
                else if (ProvincesId != 0 & AmphuresId == 0 | DistrictsId == 0)
                {
                    if (AmphuresId == 0)
                    {
                        Toast.MakeText(this, GetString(Resource.String.cannotsave) +
                                                GetString(Resource.String.selectdistict), ToastLength.Short).Show();
                        btnAdd.Enabled = true;
                        return;
                    }
                    if (DistrictsId == 0)
                    {
                        Toast.MakeText(this, GetString(Resource.String.cannotsave) +
                                                GetString(Resource.String.selectsubdistict), ToastLength.Short).Show();
                        btnAdd.Enabled = true;
                        return;
                    }
                }
                else
                {
                    customer.ProvincesId = ProvincesId;
                    customer.AmphuresId = AmphuresId;
                    customer.DistrictsId = DistrictsId;
                }
                if (string.IsNullOrEmpty(txtnationalID.Text))
                {
                    customer.IDCard = null;
                }
                else
                {
                    var count = Removespaces(txtnationalID.Text);
                    if (count.Length == 13)
                    {
                        customer.IDCard = count;
                    }
                    else
                    {
                        Toast.MakeText(this, GetString(Resource.String.idcardnotcomplete), ToastLength.Short).Show();
                        btnAdd.Enabled = true;
                        return;
                    }
                }
                customer.Comments = txtcomment.Text;
                customer.LastDateModified = Utils.GetTranDate(DateTime.UtcNow);
                customer.UserLastModified = usernamelogin;
                customer.LinkProMaxxID = txtlinkpromaxx.Text.Trim();
                customer.DataStatus = 'M';
                customer.FWaitSending = 2;
                customer.WaitSendingTime = Utils.GetTranDate(DateTime.UtcNow);
                customer.MemberTypeNo = CustomerTypeNo == 0 ? (long?)null : CustomerTypeNo;
                customer.ShortName = txtShortName.Text;

                if (txtCustomerName.Text.Trim() != customerEdit.CustomerName && txtCustomerName.Text.Trim() != string.Empty)
                {
                    var checkName = await CustomerManage.CheckCustomerName(customer.CustomerName);
                    if (checkName)
                    {
                        try
                        {
                            btnAdd.Enabled = true;
                            //เพิ่ม json สำหรับไปบันทึกข้อมูลที่ dialog                    
                            var json = JsonConvert.SerializeObject(customer);

                            MainDialog dialog = new MainDialog();
                            Bundle bundle = new Bundle();
                            String myMessage = Resource.Layout.pos_dialog_deleteitem.ToString();
                            bundle.PutString("message", myMessage);
                            bundle.PutString("insertRepeat", "insertcustomer");
                            bundle.PutString("detailnnsert", json);
                            bundle.PutString("event", "update");
                            bundle.PutString("detailitem", customer.CustomerName);
                            dialog.Arguments = bundle;
                            dialog.Show(SupportFragmentManager, myMessage);
                            return;
                        }
                        catch (Exception ex)
                        {
                            btnAdd.Enabled = true;
                            await TinyInsights.TrackErrorAsync(ex);
                            _ = TinyInsights.TrackPageViewAsync("InsertItem at add customer");
                            Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                            return;
                        }
                    }
                }

                var updateCustomer = await CustomerManage.UpdateCustomer(customer);               
                if (!updateCustomer)
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                    btnAdd.Enabled = true;
                    return;
                }

                Toast.MakeText(this, GetString(Resource.String.savesucess), ToastLength.Short).Show();

                iSysCustomerID = 0;

                if (await GabanaAPI.CheckNetWork())
                {
                    JobQueue.Default.AddJobSendCustomer((int)customer.MerchantID, (int)customer.SysCustomerID);
                }
                else
                {
                    customer.FWaitSending = 2;
                    await CustomerManage.UpdateCustomer(customer);
                }

                CustomerActivity.SetFocusCustomer(customer);
                btnAdd.Enabled = true;
                this.Finish();
            }
            catch (Exception ex)
            {
                btnAdd.Enabled = true;
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnEdit_Click at add Customer");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        async Task GetDetailCustomer()
        {
            try
            {
                first = true;
                customerEdit = await CustomerManage.GetCustomer(DataCashingAll.MerchantId, Convert.ToInt32(iSysCustomerID));

                if (customerEdit.BirthDate == null)
                {
                    txtbirthdate.Visibility = ViewStates.Gone;
                }
                else
                {
                    txtbirthdate.Visibility = ViewStates.Visible;
                }
                DateTime? Birth = Utils.BirthDayBE(customerEdit.BirthDate ?? DateTime.UtcNow);
                txtCustomerName.Text = customerEdit.CustomerName;
                //var d = Birth;
                txtbirthdate.Text = string.Format("{0:00}/{1:00}/{2:0000}", Birth.Value.Day, Birth.Value.Month, Birth.Value.Year); //"dd /MM/yyyy", new CultureInfo("en-US")
                txtcustomerid.Text = customerEdit.CustomerID;
                txtphonenumber.Text = addTextTel(customerEdit.Mobile);
                txtemail.Text = customerEdit.EMail;
                txtLine.Text = customerEdit.LineID;
                var remove = Removespaces(customerEdit.IDCard);
                txtnationalID.Text = addTextIDCard(remove);
                txtaddress.Text = customerEdit.Address;
                txtcomment.Text = customerEdit.Comments;
                txtlinkpromaxx.Text = customerEdit.LinkProMaxxID;
                txtShortName.Text = customerEdit.ShortName;

                char selectGender = customerEdit.Gender;
                Gender = customerEdit.Gender;
                if (selectGender == 'N')
                {
                    spinnergender.SetSelection(0);
                }
                else if (selectGender == 'M')
                {
                    spinnergender.SetSelection(1);
                }
                else
                {
                    spinnergender.SetSelection(2);
                }

                //Show Picture Customer   
                var cloudpath = customerEdit.PicturePath == null ? string.Empty : customerEdit.PicturePath;
                var localpath = customerEdit.ThumbnailLocalPath == null ? string.Empty : customerEdit.ThumbnailLocalPath;

                if (await GabanaAPI.CheckSpeedConnection())
                {
                    if (string.IsNullOrEmpty(localpath))
                    {
                        if (string.IsNullOrEmpty(cloudpath))
                        {
                            //defalut
                            txtNameView.Visibility = ViewStates.Visible;
                            imgProfile.SetBackgroundColor(Android.Graphics.Color.ParseColor("#E2E2E2"));
                        }
                        else
                        {
                            //cloud
                            txtNameView.Visibility = ViewStates.Invisible;
                            Utils.SetImage(imgProfile, cloudpath);
                        }
                    }
                    else
                    {
                        //local
                        txtNameView.Visibility = ViewStates.Invisible;
                        Android.Net.Uri uri = Android.Net.Uri.Parse(localpath);
                        imgProfile.SetImageURI(uri);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(localpath))
                    {
                        txtNameView.Visibility = ViewStates.Invisible;
                        Android.Net.Uri uri = Android.Net.Uri.Parse(localpath);
                        imgProfile.SetImageURI(uri);
                    }
                    else
                    {
                        txtNameView.Visibility = ViewStates.Visible;
                        imgProfile.SetBackgroundColor(Android.Graphics.Color.ParseColor("#E2E2E2"));
                    }
                }

#pragma warning disable CS0618 // Type or member is obsolete
                dialogBirthdate = new DatePickerDialog(this, Android.Resource.Style.ThemeHoloLightDialogMinWidth, DatePickerDialog_BirthDate,
                                                         Birth.Value.Year,
                                                         Birth.Value.Month - 1,
                                                         Birth.Value.Day);
#pragma warning restore CS0618 // Type or member is obsolete

                //Set Select Spinner
                //Membertype
                MemberType memberType = new MemberType();
                var lstmembertype = new List<MemberType>();
                var getallmembertype = new List<MemberType>();
                memberType = new MemberType()
                {
                    MemberTypeNo = 0,
                    MemberTypeName = "ประเภทสมาชิก"
                };
                lstmembertype.Add(memberType);
                MemberTypeManage memberTypeManage = new MemberTypeManage();
                var getalldmemberType = new List<MemberType>();
                getalldmemberType = await memberTypeManage.GetAllMemberType(DataCashingAll.MerchantId);
                lstmembertype.AddRange(getalldmemberType);

                string[] member_array = lstmembertype.Select(i => i.MemberTypeName.ToString()).ToArray();
                var adaptermember = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, member_array);
                adaptermember.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinnerCustomerType.Adapter = adaptermember;

                long? cusmemberTypeNo = customerEdit.MemberTypeNo;

                if (cusmemberTypeNo != null)
                {
                    var data = lstmembertype.Where(x => x.MemberTypeNo == cusmemberTypeNo).FirstOrDefault();
                    if (data != null)
                    {
                        int position = adaptermember.GetPosition(data.MemberTypeName);
                        spinnerCustomerType.SetSelection(position);
                    }
                }

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
                var adapterProvince = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, province_array);
                adapterProvince.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinnerProvince.Adapter = adapterProvince;

                long? provinceid = customerEdit.ProvincesId;
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

                long? amphureID = customerEdit.AmphuresId;

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
                var adapterDistrict = new ArrayAdapter<string>(this, Resource.Layout.spinner_item, district_array);
                adapterDistrict.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinnerDistrict.Adapter = adapterDistrict;

                long? districtID = customerEdit.DistrictsId;

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
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("UpdateDetailCustomer at add Customer");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
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
                _ = TinyInsights.TrackPageViewAsync("addTextTel at add Customer");
                return value;
            }
        }

        string addTextIDCard(string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    return string.Empty;
                }
                if (value.Length > 0)
                {
                    var idcard = string.Empty;
                    for (int i = 0; i < value.Length; i++)
                    {
                        if (i == 1 | i == 5 | i == 10 | i == 12)
                        {
                            idcard += " " + value[i];
                        }
                        else
                        {
                            idcard += value[i];
                        }
                    }
                    return idcard;
                }
                return value;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("addTextIDCard at add Customer");
                return value;
            }
        }

        public static string Removespaces(string strValue)
        {
            try
            {
                string pattern = "[ ]";
                string replacement = "";

                System.Text.RegularExpressions.Regex regEx = new System.Text.RegularExpressions.Regex(pattern);
                var check = System.Text.RegularExpressions.Regex.Replace(regEx.Replace(strValue, replacement), @"\s+", "");
                return check;
            }
            catch (Exception)
            {
                return strValue;
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

