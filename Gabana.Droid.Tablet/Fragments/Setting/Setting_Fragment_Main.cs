
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
using Gabana.Droid.Tablet.Dialog;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using LinqToDB.Data;
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
    public class Setting_Fragment_Main : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
        }
        public static Setting_Fragment_Main NewInstance()
        {
            Setting_Fragment_Main frag = new Setting_Fragment_Main();
            return frag;
        }

        View view;
        public static List<Item> AllItem = new List<Item>();
        public static List<Item> AllItemStatusD { get; internal set; }
        public static Setting_Fragment_Main fragment_main;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.setting_fragment_main, container, false);
            try
            {
                fragment_main = this;
                ComBineUI();
                return view;
            }
            catch (Exception)
            {
                return view;

            }
        }
        FrameLayout btnMerchant, btnBranch, btnEmpManage, btnNoteSetting, btnMembertype, btnVat,
                    btnDecimal, btnCurrency, btnServiceCharge, btnCash, btnGiftvoucher, btnMYQRSetting,
                    btnDevice, btnPrinter, btnPackage,btnCashdrawer;
        TextView txtVat, txtCurrency, txtServiceCharge;
        private void ComBineUI()
        {
            btnMerchant = view.FindViewById<FrameLayout>(Resource.Id.btnMerchant);
            btnMerchant.Click += BtnMerchant_Click;
            btnBranch = view.FindViewById<FrameLayout>(Resource.Id.btnBranch);
            btnBranch.Click += BtnBranch_Click;
            btnEmpManage = view.FindViewById<FrameLayout>(Resource.Id.btnEmpManage);
            btnEmpManage.Click += BtnEmpManage_Click;
            btnPackage = view.FindViewById<FrameLayout>(Resource.Id.btnPackage);
            btnPackage.Click += BtnPackage_Click;
            btnNoteSetting = view.FindViewById<FrameLayout>(Resource.Id.btnNoteSetting);
            btnNoteSetting.Click += BtnNoteSetting_Click;
            btnMembertype = view.FindViewById<FrameLayout>(Resource.Id.btnMembertype);
            btnMembertype.Click += BtnMembertype_Click;
            btnVat = view.FindViewById<FrameLayout>(Resource.Id.btnVat);
            btnVat.Click += BtnVat_Click;
            txtVat = view.FindViewById<TextView>(Resource.Id.txtVat);
            txtCurrency = view.FindViewById<TextView>(Resource.Id.txtCurrency);
            btnCurrency = view.FindViewById<FrameLayout>(Resource.Id.btnCurrency);
            btnCurrency.Click += BtnCurrency_Click;
            btnDecimal = view.FindViewById<FrameLayout>(Resource.Id.btnDecimal);
            btnDecimal.Click += BtnDecimal_Click;
            txtServiceCharge = view.FindViewById<TextView>(Resource.Id.txtServiceCharge);
            btnServiceCharge = view.FindViewById<FrameLayout>(Resource.Id.btnServiceCharge);
            btnServiceCharge.Click += BtnServiceCharge_Click;
            btnCash = view.FindViewById<FrameLayout>(Resource.Id.btnCash);
            btnCash.Click += BtnCash_Click;
            btnGiftvoucher = view.FindViewById<FrameLayout>(Resource.Id.btnGiftvoucher);
            btnGiftvoucher.Click += BtnGiftvoucher_Click;
            btnMYQRSetting = view.FindViewById<FrameLayout>(Resource.Id.btnMYQRSetting);
            btnMYQRSetting.Click += BtnMYQRSetting_Click;
            btnDevice = view.FindViewById<FrameLayout>(Resource.Id.btnDevice);
            btnDevice.Click += BtnDevice_Click;
            btnPrinter = view.FindViewById<FrameLayout>(Resource.Id.btnPrinter);
            btnPrinter.Click += BtnPrinter_Click;
            btnCashdrawer = view.FindViewById<FrameLayout>(Resource.Id.btnCashdrawer);
            btnCashdrawer.Click += BtnCashdrawer_Click;
        }

        private void BtnCashdrawer_Click(object sender, EventArgs e)
        {
            try
            {
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "cashdrawer");
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }

        private void BtnPackage_Click(object sender, EventArgs e)
        {
            try
            {
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "package");
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }

        private void BtnMYQRSetting_Click(object sender, EventArgs e)
        {
            try
            {
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "myqr");
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }

        private async void BtnPrinter_Click(object sender, EventArgs e)
        {
            try
            {
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "printer");
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }

        private async void BtnDevice_Click(object sender, EventArgs e)
        {
            try
            {
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "device");
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }

        private void BtnGiftvoucher_Click(object sender, EventArgs e)
        {
            try
            {
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "giftvoucher");
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }

        private void BtnCash_Click(object sender, EventArgs e)
        {
            try
            {
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "cashguild");
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }

        private void BtnServiceCharge_Click(object sender, EventArgs e)
        {
            try
            {
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "servicecharge");
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }

        private void BtnDecimal_Click(object sender, EventArgs e)
        {
            try
            {
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "decimal");
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }

        private async void BtnCurrency_Click(object sender, EventArgs e)
        {
            try
            {
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "currency");
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }

        private void BtnVat_Click(object sender, EventArgs e)
        {
            try
            {
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "vat");
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }

        private void BtnMembertype_Click(object sender, EventArgs e)
        {
            try
            {
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "membertype");
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }

        private  void BtnNoteSetting_Click(object sender, EventArgs e)
        {
            try
            {
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "note");
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }

        private void BtnEmpManage_Click(object sender, EventArgs e)
        {
            try
            {
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "empmanage");
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }

        }
        private async void BtnBranch_Click(object sender, EventArgs e)
        {
            try
            {
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "branch");
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }

        private async void BtnMerchant_Click(object sender, EventArgs e)
        {
            try
            {
                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "merchant");
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
            }
        }

        internal static void SetEnableBtnMerchant()
        {
            fragment_main.btnMerchant.Enabled = true;
        }
        internal static void SetEnableBtnBranch()
        {
            fragment_main.btnBranch.Enabled = true;
        }

        private void SetDataShow()
        {
            DialogLoading dialogLoading = new DialogLoading();
            try
            {
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(MainActivity.main_activity.SupportFragmentManager, nameof(DialogLoading));
                }

                var Vat = DataCashingAll.setmerchantConfig.TAXRATE;
                var TAXTYPE = DataCashingAll.setmerchantConfig.TAXTYPE;
                var SERVICECHARGERATE = string.Empty;
                if (string.IsNullOrEmpty(DataCashingAll.setmerchantConfig.SERVICECHARGE_RATE))
                {
                    SERVICECHARGERATE = "";
                }
                else
                {
                    SERVICECHARGERATE = DataCashingAll.setmerchantConfig.SERVICECHARGE_RATE; //อัตราการคิด service charge
                }
                var SERVICECHARGETYPE = DataCashingAll.setmerchantConfig.SERVICECHARGE_TYPE;
                var CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;

                decimal servicecharge = 0;
                if (string.IsNullOrEmpty(SERVICECHARGERATE))
                {
                    servicecharge = 0;
                }
                else
                {
                    var check = SERVICECHARGERATE.IndexOf('%');
                    if (check == -1)
                    {
                        servicecharge = Convert.ToDecimal(SERVICECHARGERATE);
                    }
                    else
                    {
                        servicecharge = Convert.ToDecimal(SERVICECHARGERATE.Remove(check)); //% ที่ SERVICECHARGE_RATE
                    }
                }

                string showService;
                if (SERVICECHARGERATE.Contains('%'))
                {
                    showService = servicecharge == 0 ? GetString(Resource.String.nonservicecharge) : Utils.DisplayDouble(servicecharge) + "%";
                }
                else
                {
                    showService = servicecharge == 0 ? GetString(Resource.String.nonservicecharge) : CURRENCYSYMBOLS + Utils.DisplayDouble(servicecharge);
                }
                if (SERVICECHARGETYPE == "B")
                {
                    txtServiceCharge.Text = showService + " , " + GetString(Resource.String.servicecharge_activity_before);
                }
                else
                {
                    txtServiceCharge.Text = showService + " , " + GetString(Resource.String.servicecharge_activity_after);
                }

                if (servicecharge == 0)
                {
                    txtServiceCharge.Text = showService;
                }

                decimal GetVat = 0;
                if (string.IsNullOrEmpty(Vat))
                {
                    GetVat = 0;
                }
                else
                {
                    GetVat = Convert.ToDecimal(Vat);
                }
                var vat = GetVat;

                if (TAXTYPE == "I")
                {
                    txtVat.Text = vat == 0 ? GetString(Resource.String.nonvat) : Utils.DisplayDouble(vat) + "%" + " , " +
                                                GetString(Resource.String.vat_activity_include);
                }
                else
                {
                    txtVat.Text = vat == 0 ? GetString(Resource.String.nonvat) : Utils.DisplayDouble(vat) + "%" + " , " +
                                               GetString(Resource.String.vat_activity_exclude);
                }

                string currencyTH = "";
                string currencyEn = "";
                switch (CURRENCYSYMBOLS)
                {
                    case "$":
                        currencyEn = "US dollar";
                        currencyTH = "ดอลลาร์สหรัฐ";
                        break;
                    case "฿":
                        currencyEn = "Thai Baht";
                        currencyTH = "บาท";
                        break;
                    case "€":
                        currencyEn = "Euro";
                        currencyTH = "ยูโร";
                        break;
                    case "¥":
                        currencyEn = "Yen";
                        currencyTH = "เยน";
                        break;
                    default:
                        currencyEn = "";
                        currencyTH = "";
                        break;
                }

                if (DataCashing.Language == "th")
                {
                    txtCurrency.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " - " + currencyTH;
                }
                else
                {
                    txtCurrency.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " - " + currencyEn;
                }

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                    dialogLoading = new DialogLoading();
                }
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                dialogLoading = new DialogLoading();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnResume at Stting");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        public static void SetShowNewData(string text)
        {            
            switch (text)
            {
                case "vat":
                    var TAXTYPE = DataCashingAll.setmerchantConfig.TAXTYPE;
                    decimal.TryParse(DataCashingAll.setmerchantConfig.TAXRATE, out decimal value);

                    if (TAXTYPE == "I")
                    {
                        fragment_main.txtVat.Text = value == 0 ? fragment_main.GetString(Resource.String.nonvat) : Utils.DisplayDouble(value) + "%" + " , " +
                                                    fragment_main.GetString(Resource.String.vat_activity_include);
                    }
                    else
                    {
                        fragment_main.txtVat.Text = value == 0 ? fragment_main.GetString(Resource.String.nonvat) : Utils.DisplayDouble(value) + "%" + " , " +
                                                   fragment_main.GetString(Resource.String.vat_activity_exclude);
                    }
                    break;
                case "servicechange":
                    var SERVICECHARGERATE = string.Empty;
                    if (string.IsNullOrEmpty(DataCashingAll.setmerchantConfig.SERVICECHARGE_RATE))
                    {
                        SERVICECHARGERATE = "";
                    }
                    else
                    {
                        SERVICECHARGERATE = DataCashingAll.setmerchantConfig.SERVICECHARGE_RATE; //อัตราการคิด service charge
                    }

                    var CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
                    var SERVICECHARGETYPE = DataCashingAll.setmerchantConfig.SERVICECHARGE_TYPE;
                    decimal servicecharge = 0;
                    if (string.IsNullOrEmpty(SERVICECHARGERATE))
                    {
                        servicecharge = 0;
                    }
                    else
                    {
                        var check = SERVICECHARGERATE.IndexOf('%');
                        if (check == -1)
                        {
                            servicecharge = Convert.ToDecimal(SERVICECHARGERATE);
                        }
                        else
                        {
                            servicecharge = Convert.ToDecimal(SERVICECHARGERATE.Remove(check)); //% ที่ SERVICECHARGE_RATE
                        }
                    }

                    string showService;
                    if (SERVICECHARGERATE.Contains('%'))
                    {
                        showService = servicecharge == 0 ? fragment_main.GetString(Resource.String.nonservicecharge) : Utils.DisplayDouble(servicecharge) + "%";
                    }
                    else
                    {
                        showService = servicecharge == 0 ? fragment_main.GetString(Resource.String.nonservicecharge) : CURRENCYSYMBOLS + Utils.DisplayDouble(servicecharge);
                    }
                    if (SERVICECHARGETYPE == "B")
                    {
                        fragment_main.txtServiceCharge.Text = showService + " , " + fragment_main.GetString(Resource.String.servicecharge_activity_before);
                    }
                    else
                    {
                        fragment_main.txtServiceCharge.Text = showService + " , " + fragment_main.GetString(Resource.String.servicecharge_activity_after);
                    }

                    if (servicecharge == 0)
                    {
                        fragment_main.txtServiceCharge.Text = showService;
                    }
                    break;
                case "currency":
                    var CURRENCYSYMBOL = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
                    string currencyTH = "";
                    string currencyEn = "";
                    switch (CURRENCYSYMBOL)
                    {
                        case "$":
                            currencyEn = "US dollar";
                            currencyTH = "ดอลลาร์สหรัฐ";
                            break;
                        case "฿":
                            currencyEn = "Thai Baht";
                            currencyTH = "บาท";
                            break;
                        case "€":
                            currencyEn = "Euro";
                            currencyTH = "ยูโร";
                            break;
                        case "¥":
                            currencyEn = "Yen";
                            currencyTH = "เยน";
                            break;
                        default:
                            currencyEn = "";
                            currencyTH = "";
                            break;
                    }

                    if (DataCashing.Language == "th")
                    {
                        fragment_main.txtCurrency.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " - " + currencyTH;
                    }
                    else
                    {
                        fragment_main.txtCurrency.Text = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS + " - " + currencyEn;
                    }
                    break;

                default:
                    break;
            }

        }


        public override void OnResume()
        {
            try
            {
                base.OnResume();

                //if (!IsVisible)
                //{
                //    return;
                //}

                SetDataShow();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }
    }
}