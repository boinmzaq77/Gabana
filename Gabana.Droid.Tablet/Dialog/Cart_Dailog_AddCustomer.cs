using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Adapter.Pos;
using Gabana.Droid.Tablet.Fragments.Customers;
using Gabana.Droid.Tablet.Fragments.PayMent;
using Gabana.Droid.Tablet.Fragments.POS;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ORM.PoolDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana3.JAM.Trans;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Cart_Dailog_AddCustomer : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public static Cart_Dailog_AddCustomer NewInstance()
        {
            var frag = new Cart_Dailog_AddCustomer { Arguments = new Bundle() };
            return frag;
        }

        View view;
        string usernamelogin, LoginType;
        internal static Android.Net.Uri keepCropedUri = null;
        public static Cart_Dailog_AddCustomer cart_dailog_adCustomer;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.addcustomer_fragment_main, container, false);
            try
            {
                cart_dailog_adCustomer = this;
                ComBineUI();
                showdetail = false;
                ShowDetail();
                SetUIFromMainRole(LoginType);
                first = false;
                return view;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackPageViewAsync("Cart_Dailog_AddCustomer");
                _ = TinyInsights.TrackErrorAsync(ex);
                return view;
            }
        }

        TextView txtTitle, txtNameView, txtbirthdate, txtzipcode, txtNoCustomerType;
        FrameLayout lnBack;
        internal static ImageView imgProfile;
        ImageButton btnAddImage, btnCustomertype;
        EditText txtCustomerName, txtphonenumber, txtcustomerid, txtShortName, txtnationalID, txtemail, txtLine,
                    txtaddress, txtcomment, txtlinkpromaxx;
        Spinner spinnerCustomerType, spinnergender, spinnerProvince, spinneramphures, spinnerDistrict;
        internal Button btnAdd;
        LinearLayout lnSelectType, lnDetails;
        ImageButton btnShowDetail, imgbirthdate, imggender, btnprovinces, btnAmphure, btnDistrict;
        FrameLayout lnShowDelete, lnDelete;
        Android.App.DatePickerDialog dialogBirthdate;
        List<string> Provinces;
        List<string> Amphures;
        List<string> District;
        List<string> zipcode;
        List<string> CustomerType;
        string pathThumnailFolder, Language, pathFolderPicture;
        internal static Customer customerEdit;
        string CustomerName, path, CustomerID;
        DateTime? birthdate;
        Android.Graphics.Bitmap bitmap;
        int ProvincesId = 0, AmphuresId = 0, DistrictsId = 0, CustomerTypeNo = 0;
        DeviceSystemSeqNoManage deviceSystemSeqNoManage = new DeviceSystemSeqNoManage();
        CustomerManage customerManage = new CustomerManage();
        PoolManage poolManage = new PoolManage();
        List<Province> GetProvinces = new List<Province>();
        List<Amphure> GetAmphures = new List<Amphure>();
        List<District> GetDistricts = new List<District>();
        string Phone = string.Empty, idcard = string.Empty;
        bool showdetail = false, first = true;
        public static bool flagdatachange = false;
        string shortName;

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
                btnAdd.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
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
                txtCustomerName.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                txtCustomerName.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtphonenumber.Enabled = false;
                txtphonenumber.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                txtphonenumber.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                spinnerCustomerType.Enabled = false;
                lnSelectType.Enabled = false;
                btnCustomertype.Enabled = false;
                btnCustomertype.SetBackgroundResource(Resource.Mipmap.NextG);
                txtcustomerid.Enabled = false;
                //txtcustomerid.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                txtcustomerid.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtShortName.Enabled = false;
                txtShortName.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                txtShortName.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtnationalID.Enabled = false;
                txtnationalID.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                txtnationalID.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtbirthdate.Enabled = false;
                txtbirthdate.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                txtbirthdate.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                imgbirthdate.Enabled = false;
                imgbirthdate.SetBackgroundResource(Resource.Mipmap.NextG);
                spinnergender.Enabled = false;
                imggender.Enabled = false;
                imggender.SetBackgroundResource(Resource.Mipmap.NextG);
                txtemail.Enabled = false;
                txtemail.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                txtemail.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtLine.Enabled = false;
                txtLine.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                txtLine.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtaddress.Enabled = false;
                txtaddress.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
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
                txtzipcode.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                txtzipcode.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                txtcomment.Enabled = false;
                txtcomment.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                txtcomment.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
                btnAdd.Enabled = false;
                btnAdd.SetBackgroundResource(Resource.Drawable.btnWhiteBorderGrayRD5);
                btnAdd.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                txtlinkpromaxx.SetTextColor(Resources.GetColor(Resource.Color.nobel, null));
                txtlinkpromaxx.SetHintTextColor(Resources.GetColor(Resource.Color.colorrule, null));
            }
            check = UtilsAll.CheckPermissionRoleUser(loginType, "delete", "customer");
            if (check && DataCashing.EditCus?.SysCustomerID > 0)
            {
                lnShowDelete.Visibility = ViewStates.Visible;
            }
            else
            {
                lnShowDelete.Visibility = ViewStates.Gone;
            }
        }

        public void CheckDataChange()
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

                    string CusEdit = string.IsNullOrEmpty(customerEdit.CustomerID) ? "" : customerEdit.CustomerID;
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
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
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

        private void ComBineUI()
        {
            try
            {
                txtTitle = view.FindViewById<TextView>(Resource.Id.txtTitle);
                lnBack = view.FindViewById<FrameLayout>(Resource.Id.lnBack);
                imgProfile = view.FindViewById<ImageView>(Resource.Id.imgProfile);
                txtNameView = view.FindViewById<TextView>(Resource.Id.txtNameView);
                btnAddImage = view.FindViewById<ImageButton>(Resource.Id.btnAddImage);
                txtCustomerName = view.FindViewById<EditText>(Resource.Id.txtCustomerName);
                txtphonenumber = view.FindViewById<EditText>(Resource.Id.txtphonenumber);
                spinnerCustomerType = view.FindViewById<Spinner>(Resource.Id.spinnerCustomerType);
                btnCustomertype = view.FindViewById<ImageButton>(Resource.Id.btnCustomertype);
                lnSelectType = view.FindViewById<LinearLayout>(Resource.Id.lnSelectType);
                btnShowDetail = view.FindViewById<ImageButton>(Resource.Id.btnShowDetail);
                lnDetails = view.FindViewById<LinearLayout>(Resource.Id.lnDetails);
                txtcustomerid = view.FindViewById<EditText>(Resource.Id.txtcustomerid);
                txtShortName = view.FindViewById<EditText>(Resource.Id.txtShortName);
                txtnationalID = view.FindViewById<EditText>(Resource.Id.txtnationalID);
                txtbirthdate = view.FindViewById<TextView>(Resource.Id.txtbirthdate);
                imgbirthdate = view.FindViewById<ImageButton>(Resource.Id.imgbirthdate);
                spinnergender = view.FindViewById<Spinner>(Resource.Id.spinnergender);
                imggender = view.FindViewById<ImageButton>(Resource.Id.imggender);
                txtemail = view.FindViewById<EditText>(Resource.Id.txtemail);
                txtLine = view.FindViewById<EditText>(Resource.Id.txtLine);
                txtaddress = view.FindViewById<EditText>(Resource.Id.txtaddress);
                spinnerProvince = view.FindViewById<Spinner>(Resource.Id.spinnerProvince);
                btnprovinces = view.FindViewById<ImageButton>(Resource.Id.btnprovinces);
                spinneramphures = view.FindViewById<Spinner>(Resource.Id.spinneramphures);
                btnAmphure = view.FindViewById<ImageButton>(Resource.Id.btnAmphure);
                spinnerDistrict = view.FindViewById<Spinner>(Resource.Id.spinnerDistrict);
                btnDistrict = view.FindViewById<ImageButton>(Resource.Id.btnDistrict);
                txtzipcode = view.FindViewById<TextView>(Resource.Id.txtzipcode);
                txtcomment = view.FindViewById<EditText>(Resource.Id.txtcomment);
                txtlinkpromaxx = view.FindViewById<EditText>(Resource.Id.txtlinkpromaxx);
                lnShowDelete = view.FindViewById<FrameLayout>(Resource.Id.lnShowDelete);
                lnDelete = view.FindViewById<FrameLayout>(Resource.Id.lnDelete);
                btnAdd = view.FindViewById<Button>(Resource.Id.btnAdd);
                txtNoCustomerType = view.FindViewById<TextView>(Resource.Id.txtNoCustomerType);

                lnBack.Click += LnBack_Click;
                lnSelectType.Click += LnSelectType_Click;
                btnShowDetail.Click += BtnShowDetail_Click;
                txtbirthdate.Click += Txtbirthdate_Click;
                imgbirthdate.Click += Txtbirthdate_Click;
                imggender.Click += Imggender_Click;
                btnprovinces.Click += Btnprovinces_Click;
                btnAmphure.Click += BtnAmphure_Click;
                btnDistrict.Click += BtnDistrict_Click;
                txtphonenumber.TextChanged += Txtphonenumber_TextChanged;
                txtnationalID.TextChanged += TxtnationalID_TextChanged;
                txtCustomerName.TextChanged += TxtCustomerName_TextChanged;
                txtShortName.TextChanged += TxtShortName_TextChanged;
                txtcustomerid.TextChanged += Txtcustomerid_TextChanged;
                imgProfile.Click += ImgProfile_Click;
                btnAddImage.Click += BtnAddImage_Click;
                btnAdd.Click += BtnAdd_Click;
                lnDelete.Click += LnDelete_Click;
                lnShowDelete.Click += LnDelete_Click;

                Language = Preferences.Get("Language", "");
                usernamelogin = Preferences.Get("User", "");
                LoginType = Preferences.Get("LoginType", "");
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("spinnergender_ItemSelected at add Customer");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void LnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                bool check = UtilsAll.CheckPermissionRoleUser(LoginType, "delete", "customer");
                if (!check)
                {
                    Toast.MakeText(Application.Context, GetString(Resource.String.notperm), ToastLength.Short).Show();
                    return;
                }

                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.customer_dialog_delete.ToString();
                bundle.PutString("message", myMessage);
                Customer_Dialog_Delete customer_Dialog = Customer_Dialog_Delete.NewInstance();
                customer_Dialog.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
            }
            catch (Exception ex)
            {
                _= TinyInsights.TrackErrorAsync(ex);
                _= TinyInsights.TrackPageViewAsync("LnDelete_Click at add Customer");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void BtnAddImage_Click(object sender, EventArgs e)
        {
            try
            {
                DataCashing.flagChooseMedia = true;
                var fragment = new Customer_Dialog_SelectMedia();
                fragment.Show(this.Activity.SupportFragmentManager, nameof(Customer_Dialog_SelectMedia));
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnAddImage_Click at add Customer");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void  ImgProfile_Click(object sender, EventArgs e)
        {
            try
            {
                //การแก้ไขข้อมูล , พาธรูปไม่ว่าง , มีอินเตอร์เน็ต
                if (DataCashing.EditCus?.SysCustomerID > 0 && !string.IsNullOrEmpty(customerEdit.PicturePath) && DataCashing.CheckNet)
                {
                    Customer_Dialog_ShowImage dialog = Customer_Dialog_ShowImage.NewInstance(customerEdit.PicturePath);
                    dialog.Show(MainActivity.main_activity.SupportFragmentManager, nameof(Customer_Dialog_ShowImage));
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ImgProfile_Click at add Customer");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void Txtcustomerid_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                CustomerID = txtcustomerid.Text.Trim();
                CheckDataChange();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Txtcustomerid_TextChanged at add Customer");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void TxtShortName_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(shortName))
                {
                    return;
                }

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
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("TxtShortName_TextChanged at add Customer");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        string cusName = "", temp;
        private void TxtCustomerName_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                CustomerName = txtCustomerName.Text.Trim();
                if (string.IsNullOrEmpty(CustomerName))
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
                SetImgProfile();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("TxtCustomerName_TextChanged at add Customer");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
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

                    txtNameView.Text = string.Empty;
                    txtNameView.Visibility = ViewStates.Gone;
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

        private void TxtnationalID_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
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

        private void BtnDistrict_Click(object sender, EventArgs e)
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

        private void Imggender_Click(object sender, EventArgs e)
        {
            spinnergender.PerformClick();
        }

        private void Txtbirthdate_Click(object sender, EventArgs e)
        {
            dialogBirthdate.Show();
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
                Console.WriteLine(ex.ToString());
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

                if (DataCashing.EditCus == null)
                {
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.add_dialog_back.ToString();
                    bundle.PutString("message", myMessage);
                    Add_Dialog_Back.SetPage("customer");
                    Add_Dialog_Back add_Dialog = Add_Dialog_Back.NewInstance();                    
                    add_Dialog.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
                    return;
                }
                else
                {
                    Bundle bundle = new Bundle();
                    String myMessage = Resource.Layout.edit_dialog_back.ToString();
                    bundle.PutString("message", myMessage);
                    Edit_Dialog_Back.SetPage("customer");
                    Edit_Dialog_Back edit_Dialog = Edit_Dialog_Back.NewInstance();
                    edit_Dialog.Show(MainActivity.main_activity.SupportFragmentManager, myMessage);
                    return;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("LnBack_Click at add Customer");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public async void SetClearData()
        {
            UINewCustomer();
            flagdatachange = false;
            DataCashing.flagChooseMedia = false;
            DataCashing.EditCus = null;
            customerEdit = null;
            Cart_Dailog_Customer.cart_dailog_addCustomer = null;
            this.Dialog.Dismiss();
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
                var adapterspinnerProvince = new ArrayAdapter<string>(this.Activity, Resource.Layout.spinner_item, items);
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
                var adapterspinnerdistrict = new ArrayAdapter<string>(this.Activity, Resource.Layout.spinner_item, items);
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
       
        private void DatePickerDialog_BirthDate(object sender, DatePickerDialog.DateSetEventArgs e)
        {
            txtbirthdate.Visibility = ViewStates.Visible;
            txtbirthdate.Text = e.Date.ToString("dd/MM/yyyy", new CultureInfo("en-US"));
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        char Gender;
        private void spinnergender_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("spinnergender_ItemSelected at add Customer");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        public async void SelectMermberType()
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
                var adapterspinnercustomertype = new ArrayAdapter<string>(this.Activity, Resource.Layout.spinner_item, items);
                adapterspinnercustomertype.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinnerCustomerType.Adapter = adapterspinnercustomertype;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SelectMermberType at add Customer");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        public override async void OnResume()
        {
            try
            {
                base.OnResume();

                //if (!IsVisible)
                //{
                //    return;
                //}

                showdetail = false;
                ShowDetail();

                //เพิ่ม flag สำหรับตรวจจับว่ามีการกดเลือกรูปหรือไม่ เพราะ ตอนนี้จะเข้า Onresume ตลอดทำให้ข้อมูลที่เคยกรอกไว้หายไป
                if (DataCashing.flagChooseMedia)
                {
                    SetImgProfile();
                    return;
                }
                first = true;
                UINewCustomer();
                SetDefaultData();
                if (DataCashing.EditCus == null)
                {                    
                    txtTitle.Text = GetString(Resource.String.addcustomer_activity_addcustomer);
                    btnAdd.Text = GetString(Resource.String.addcustomer_activity_addcustomer);                    
                    imgProfile.SetBackgroundResource(Resource.Mipmap.defaultcust);
                    lnShowDelete.Visibility = ViewStates.Gone;                    
                }
                else
                {
                    txtTitle.Text = GetString(Resource.String.addcustomer_activity_editcustomer);
                    btnAdd.Text = GetString(Resource.String.textsave);
                    lnShowDelete.Visibility = ViewStates.Visible;
                    await GetDetailCustomer();
                }
                first = false;
                flagdatachange = false;
                SetButtonAdd(false);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnResume at add Customer");
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void SetDefaultData()
        {
            try
            {
                //Birthdate
                DateTime dateTime = DateTime.Today;
                txtbirthdate.Visibility = ViewStates.Gone;
                txtbirthdate.Text = dateTime.ToString("dd/MM/yyyy", new CultureInfo("en-US"));
                dialogBirthdate = new DatePickerDialog(this.Activity, Android.Resource.Style.ThemeHoloLightDialogMinWidth, DatePickerDialog_BirthDate,
                                                         dateTime.Year,
                                                         dateTime.Month - 1,
                                                         dateTime.Day);

                //gender
                spinnergender.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinnergender_ItemSelected);
                var adapter = ArrayAdapter.CreateFromResource(this.Activity, Resource.Array.spinGender, Resource.Layout.spinner_item);
                adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinnergender.Adapter = adapter;

                Android.Content.Res.Resources res = this.Resources;
                string selectzipcode = res.GetString(Resource.String.addcustomer_activity_selectzipcode);
                txtzipcode.Text = selectzipcode;

                Gender = 'N'; //default none
                Provinces = new List<string>();
                Amphures = new List<string>();
                District = new List<string>();
                CustomerType = new List<string>();
                pathThumnailFolder = DataCashingAll.PathThumnailFolderImage;
                pathFolderPicture = DataCashingAll.PathFolderImage;

                SelectMermberType();
                SelectProvince();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity , "SetDefaultData : " + ex.Message, ToastLength.Short).Show();
            }

        }

        async Task GetDetailCustomer()
        {
            try
            {
                if (DataCashing.EditCus != null)
                {                    
                    customerEdit = DataCashing.EditCus;
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

                    if (DataCashing.CheckNet)
                    {
                        if (string.IsNullOrEmpty(localpath))
                        {
                            if (string.IsNullOrEmpty(cloudpath))
                            {
                                //defalut
                                txtNameView.Text = string.Empty;
                                txtNameView.Text = customerEdit.CustomerName;
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
                            txtNameView.Text = string.Empty;
                            txtNameView.Text = customerEdit.CustomerName;
                            txtNameView.Visibility = ViewStates.Visible;
                            imgProfile.SetBackgroundColor(Android.Graphics.Color.ParseColor("#E2E2E2"));
                        }
                    }

                    dialogBirthdate = new DatePickerDialog(this.Activity, Android.Resource.Style.ThemeHoloLightDialogMinWidth, DatePickerDialog_BirthDate,
                                                             Birth.Value.Year,
                                                             Birth.Value.Month - 1,
                                                             Birth.Value.Day);

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
                    var adaptermember = new ArrayAdapter<string>(this.Activity, Resource.Layout.spinner_item, member_array);
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

                    //hide addcustomer_activity_nocustomertype เมื่อไม่มีค่่า
                    if (lstmembertype.Count > 0) txtNoCustomerType.Visibility = ViewStates.Gone;

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

                    long? provinceid = customerEdit.ProvincesId;
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
                        ProvincesId = 0;
                        int position = adapterProvince.GetPosition(select);
                        spinnerProvince.SetSelection(position);
                        SelectProvince();
                    }

                    if (provinceid == null)
                    {
                        AmphuresId = 0;
                        DistrictsId = 0;
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
                        AmphuresId = 0;
                        int position = adapterAmphure.GetPosition(selectamphure);
                        spinneramphures.SetSelection(position);
                    }

                    if (amphureID == null)
                    {
                        DistrictsId = 0;
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
                    var adapterDistrict = new ArrayAdapter<string>(this.Activity, Resource.Layout.spinner_item, district_array);
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
                        DistrictsId = 0;
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
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("UpdateDetailCustomer at add Customer");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        public void UINewCustomer()
        {
            try
            {
                txtTitle.Text = string.Empty;
                txtNameView.Text = string.Empty;
                txtCustomerName.Text = string.Empty;
                txtphonenumber.Text = string.Empty;
                txtcustomerid.Text = string.Empty;
                txtShortName.Text = string.Empty;
                txtnationalID.Text = string.Empty;
                txtbirthdate.Text = string.Empty;
                txtemail.Text = string.Empty;
                txtLine.Text = string.Empty;
                txtaddress.Text = string.Empty;
                txtzipcode.Text = string.Empty;
                txtcomment.Text = string.Empty;                
                txtNameView.Visibility = ViewStates.Invisible;
                Android.Net.Uri uri = Android.Net.Uri.Parse(string.Empty);
                imgProfile.SetImageURI(uri);
                keepCropedUri = null;
                Gender = 'N'; //default none

                Provinces = new List<string>();
                Provinces = new List<string>();
                Amphures = new List<string>();
                District = new List<string>();
                CustomerType = new List<string>();
                ProvincesId = 0;
                AmphuresId = 0;
                DistrictsId = 0;

                pathThumnailFolder = DataCashingAll.PathThumnailFolderImage;
                pathFolderPicture = DataCashingAll.PathFolderImage;
                spinnergender.SetSelection(0);
                spinnerCustomerType.SetSelection(0);
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("UINewCustomer at add Customer");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private async void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                btnAdd.Enabled = false;
                bool CheckDup = await CheckDuplicateData();
                if (!CheckDup)
                {
                    btnAdd.Enabled = true;
                    var fragmenta = new AddCustomer_Dialog_Dubicate();
                    fragmenta.Show(MainActivity.main_activity.SupportFragmentManager, nameof(AddCustomer_Dialog_Dubicate));
                    AddCustomer_Dialog_Dubicate.SetCustomerName(CustomerName);
                    return;
                }
                ManageCustomer();
                btnAdd.Enabled = true;
            }
            catch (Exception ex)
            {
                btnAdd.Enabled = true;
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnAdd_Click at add Customer");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        public async void ManageCustomer()
        {
            try
            {
                bool check;
                if (DataCashing.EditCus == null)
                {
                    check = await InsertCustomer();
                    if (!check) return;
                }
                else
                {
                    check = await UpdateCustomer();
                    if (!check) return;
                }               
                SetClearData();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ManageCustomer at addCustomer");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private async Task<bool> CheckDuplicateData()
        {
            try
            {
                //Name
                if (string.IsNullOrEmpty(CustomerName))
                {
                    btnAdd.Enabled = true;
                    Toast.MakeText(this.Activity, GetString(Resource.String.usernamenotcomplete), ToastLength.Short).Show();
                    return false;
                }

                //Check Customer Name ซ้ำ
                bool checkName = false;                
                if (DataCashing.EditCus == null)
                {
                    checkName = await customerManage.CheckCustomerName(CustomerName);
                }
                else
                {
                    if (customerEdit.CustomerName != CustomerName)
                    {
                        checkName = await customerManage.CheckCustomerName(CustomerName);
                    }                    
                }

                if (checkName)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                btnAdd.Enabled = true;
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("CheckDuplicateData at add Customer");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return false;
            }
        }

        public async Task<bool> InsertCustomer()
        {
            try
            {                
                var birth = txtbirthdate.Text;
                CultureInfo culture = new CultureInfo("en-US");
                DateTime dateTime = new DateTime();
                dateTime = DateTime.UtcNow;
                string datenow = dateTime.ToString("dd/MM/yyyy", new CultureInfo("en-US"));
                birthdate = DateTime.ParseExact(birth, "dd/MM/yyyy", culture);

                if (birthdate > dateTime)
                {
                    btnAdd.Enabled = true;
                    Toast.MakeText(this.Activity, GetString(Resource.String.birthdaynotcomplete), ToastLength.Short).Show();
                    return false;
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
                        btnAdd.Enabled = true;
                        Toast.MakeText(this.Activity, GetString(Resource.String.emailnotcomplete), ToastLength.Short).Show();
                        return false;
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
                            btnAdd.Enabled = true;
                            Toast.MakeText(this.Activity, GetString(Resource.String.telnotcomplete), ToastLength.Short).Show();
                            return false;
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
                            btnAdd.Enabled = true;
                            Toast.MakeText(this.Activity, GetString(Resource.String.telnotcomplete), ToastLength.Short).Show();
                            return false;
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
                        btnAdd.Enabled = true;
                        Toast.MakeText(this.Activity, GetString(Resource.String.cannotsave) +
                                                GetString(Resource.String.selectdistict), ToastLength.Short).Show();
                        return false;
                    }
                    if (DistrictsId == 0)
                    {
                        btnAdd.Enabled = true;
                        Toast.MakeText(this.Activity, GetString(Resource.String.cannotsave) +
                                                GetString(Resource.String.selectsubdistict), ToastLength.Short).Show();
                        return false;
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
                        btnAdd.Enabled = true;
                        Toast.MakeText(this.Activity, GetString(Resource.String.idcardnotcomplete), ToastLength.Short).Show();
                        return false;
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

                var insertCustomer = await customerManage.InsertCustomer(customer);
                if (!insertCustomer)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                    return false;
                }

                Toast.MakeText(this.Activity, GetString(Resource.String.savesucess), ToastLength.Short).Show();

                if (DataCashing.CheckNet)
                {
                    JobQueue.Default.AddJobSendCustomer((int)customer.MerchantID, (int)customer.SysCustomerID);
                }
                else
                {
                    customer.FWaitSending = 2;
                    await customerManage.UpdateCustomer(customer);
                }

                Customer_Fragment_Main.fragment_main.ReloadCustomer(customer);
                if (Cart_Dailog_Customer.cart_dailog_customer != null)
                {
                    Cart_Dailog_Customer.cart_dailog_customer.ReloadCustomer(customer);
                }
                return true;
            }
            catch (Exception ex)
            {
                btnAdd.Enabled = true;
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnAdd_Click at add Customer");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return false;
            }
        }

        public async Task<bool> UpdateCustomer()
        {
            try
            {
                var birth = txtbirthdate.Text;
                //Name
                if (string.IsNullOrEmpty(CustomerName))
                {
                    btnAdd.Enabled = true;
                    Toast.MakeText(this.Activity, GetString(Resource.String.usernamenotcomplete), ToastLength.Short).Show();
                    return false;
                }

                CultureInfo culture = new CultureInfo("en-US");

                DateTime dateTime = new DateTime();
                dateTime = DateTime.UtcNow;
                string datenow = dateTime.ToString("dd/MM/yyyy", new CultureInfo("en-US"));
                var data = DateTime.ParseExact(birth, "dd/MM/yyyy", culture);
                DateTime? birthdate = Utils.GetTranDate(data);
                if (birthdate > dateTime)
                {
                    btnAdd.Enabled = true;
                    Toast.MakeText(this.Activity, GetString(Resource.String.birthdaynotcomplete), ToastLength.Short).Show();
                    return false;
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
                        btnAdd.Enabled = true;
                        Toast.MakeText(Application.Context, GetString(Resource.String.emailnotcomplete), ToastLength.Short).Show();
                        return false;
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
                            btnAdd.Enabled = true;
                            Toast.MakeText(Application.Context, GetString(Resource.String.telnotcomplete), ToastLength.Short).Show();
                            return false;
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
                            btnAdd.Enabled = true;
                            Toast.MakeText(Application.Context, GetString(Resource.String.telnotcomplete), ToastLength.Short).Show();
                            return false;
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
                        btnAdd.Enabled = true;
                        Toast.MakeText(Application.Context, GetString(Resource.String.cannotsave) +
                                                GetString(Resource.String.selectdistict), ToastLength.Short).Show();
                        return false;
                    }
                    if (DistrictsId == 0)
                    {
                        btnAdd.Enabled = true;
                        Toast.MakeText(Application.Context, GetString(Resource.String.cannotsave) +
                                                GetString(Resource.String.selectsubdistict), ToastLength.Short).Show();
                        return false;
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
                        btnAdd.Enabled = true;
                        Toast.MakeText(Application.Context, GetString(Resource.String.idcardnotcomplete), ToastLength.Short).Show();
                        return false;
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

                var updateCustomer = await customerManage.UpdateCustomer(customer);
                if (!updateCustomer)
                {
                    btnAdd.Enabled = true;
                    Toast.MakeText(Application.Context, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                    return false;
                }

                Toast.MakeText(Application.Context, GetString(Resource.String.savesucess), ToastLength.Short).Show();
                if (DataCashing.CheckNet)
                {
                    JobQueue.Default.AddJobSendCustomer((int)customer.MerchantID, (int)customer.SysCustomerID);
                }
                else
                {
                    customer.FWaitSending = 2;
                    await customerManage.UpdateCustomer(customer);
                }

                Customer_Fragment_Main.fragment_main.ReloadCustomer(customer);
                return true;
            }
            catch (Exception ex)
            {
                btnAdd.Enabled = true;
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnEdit_Click at add Customer");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return false;
            }
        }
    }
}