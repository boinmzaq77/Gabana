 using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Gabana.Model;
using Gabana.ShareSource;
using System;
using TinyInsightsLib;
using Xamarin.Essentials;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class SettingActivity : AppCompatActivity
    {
        SettingActivity activity;
        TextView txtServiceCharge, txtVat, txtCurrency;
        DialogLoading dialogLoading = new DialogLoading();
        string LoginType;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.setting_activity);
                activity = this;

                FrameLayout btnMerchant = FindViewById<FrameLayout>(Resource.Id.btnMerchant);
                FrameLayout btnEmployee = FindViewById<FrameLayout>(Resource.Id.btnEmployee);
                FrameLayout btnBranch = FindViewById<FrameLayout>(Resource.Id.btnBranch);
                FrameLayout btnLinkProMaxx = FindViewById<FrameLayout>(Resource.Id.btnLinkProMaxx);
                FrameLayout btnPackage = FindViewById<FrameLayout>(Resource.Id.btnPackage);
                FrameLayout btnNoteSetting = FindViewById<FrameLayout>(Resource.Id.btnNoteSetting);
                FrameLayout btnMemberType = FindViewById<FrameLayout>(Resource.Id.btnMembertype);
                FrameLayout btnVat = FindViewById<FrameLayout>(Resource.Id.btnVat);
                FrameLayout btnCurrency = FindViewById<FrameLayout>(Resource.Id.btnCurrency);
                FrameLayout btnDecimal = FindViewById<FrameLayout>(Resource.Id.btnDecimal);
                FrameLayout btnServiceCharge = FindViewById<FrameLayout>(Resource.Id.btnServiceCharge);
                FrameLayout btnCash = FindViewById<FrameLayout>(Resource.Id.btnCash);
                FrameLayout btnGiftvoucher = FindViewById<FrameLayout>(Resource.Id.btnGiftvoucher);
                FrameLayout btnDevice = FindViewById<FrameLayout>(Resource.Id.btnDevice);
                FrameLayout btnPrinter = FindViewById<FrameLayout>(Resource.Id.btnPrinter);
                FrameLayout btnMYQRSetting = FindViewById<FrameLayout>(Resource.Id.btnMYQRSetting);
                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                ImageButton btnBack = FindViewById<ImageButton>(Resource.Id.btnBack);

                txtVat = FindViewById<TextView>(Resource.Id.txtVat);
                txtServiceCharge = FindViewById<TextView>(Resource.Id.txtServiceCharge);
                txtCurrency = FindViewById<TextView>(Resource.Id.txtCurrency);
                CheckJwt();
                LoginType = Preferences.Get("LoginType", "");

                lnBack.Click += LnBack_Click;
                btnBack.Click += LnBack_Click;

                btnBranch.Click += BtnBranch_Click;
                btnEmployee.Click += BtnEmployee_Click;
                //btnDiscount.Click += BtnDiscount_Click;
                btnNoteSetting.Click += BtnNoteSetting_Click;
                btnServiceCharge.Click += BtnServiceCharge_Click;
                btnCash.Click += BtnCash_Click;
                btnMemberType.Click += BtnMemberType_Click;
                btnMerchant.Click += BtnMerchant_Click;
                btnVat.Click += BtnVat_Click;
                btnCurrency.Click += BtnCurrency_Click;
                btnGiftvoucher.Click += BtnGiftvoucher_Click;
                btnMYQRSetting.Click += BtnMYQRSetting_Click;
                btnDevice.Click += BtnDevice_Click;
                btnPrinter.Click += BtnPrinter_Click;
                btnDecimal.Click += BtnDecimal_Click;
                btnPackage.Click += BtnPackage_Click;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at Setting");
            }
        }

        private void BtnPackage_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(SettingPackageActivity)));
        }

        private void BtnCash_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(CashGuideActivity)));
        }

        private void BtnMYQRSetting_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(SettingmyQRActivity)));
        }

        private void BtnPrinter_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(PrinterActivity)));
        }

        private void BtnDevice_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(DeviceActivity)));
        }

        private void BtnEmployee_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(EmpManageActivity)));
        }

        private void BtnMemberType_Click(object sender, EventArgs e)
        {
            StartActivity(new Android.Content.Intent(Application.Context, typeof(MemberTypeActivity)));
        }

        private void BtnDecimal_Click(object sender, EventArgs e)
        {
            bool check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "decimal");
            if (check)
            {
                StartActivity(new Android.Content.Intent(Application.Context, typeof(DecimalActivity)));
            }
            else
            {
                Toast.MakeText(this, GetString(Resource.String.notperm), ToastLength.Short).Show();
            }
        }        

        private void BtnServiceCharge_Click(object sender, EventArgs e)
        {
            bool check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "servicecharge");
            if (check)
            {
                StartActivity(new Intent(Application.Context, typeof(ServiceChargeActivity)));
            }
            else
            {
                Toast.MakeText(this, GetString(Resource.String.notperm), ToastLength.Short).Show();
            }
        }

        private void BtnGiftvoucher_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(GiftvoucherActivity)));
        }

        private void BtnCurrency_Click(object sender, EventArgs e)
        {
            bool check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "currency");
            if (check)
            {
                StartActivity(new Intent(Application.Context, typeof(CurrencyActivity)));
            }
            else
            {
                Toast.MakeText(this, GetString(Resource.String.notperm), ToastLength.Short).Show();
            }
        }

        private void BtnVat_Click(object sender, EventArgs e)
        {
            bool check = UtilsAll.CheckPermissionRoleUser(LoginType, "insert", "vat");
            if (check)
            {
                StartActivity(new Intent(Application.Context, typeof(VatActivity)));
            }
            else
            {
                Toast.MakeText(this, GetString(Resource.String.notperm), ToastLength.Short).Show();
            }
        }

        private void BtnBranch_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(BranchActivity)));
        }

        private void BtnMerchant_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(MerchantActivity)));
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }

        private void BtnNoteSetting_Click(object sender, EventArgs e)
        {
            StartActivity(new Intent(Application.Context, typeof(NoteActivity)));
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                CheckJwt();
                SetDataShow();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("OnResume at Stting");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                Utils.RestartApplication(this.activity, "main", 1);
            }
        }

        private void SetDataShow()
        {
            try
            {
                if (dialogLoading.Cancelable != false)
                {
                    dialogLoading.Cancelable = false;
                    dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
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
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'SettingActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'SettingActivity.openPage' is assigned but its value is never used
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