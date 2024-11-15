using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Widget;
using Gabana.Droid.Adapter;
using Gabana.Droid.ListData;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using TinyInsightsLib;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]

    public class CurrencyActivity : AppCompatActivity
    {
        RecyclerView recyclerview_listcurrency;
        CurrencyActivity currencyActivity;
        ListCurrency listCurrency;
        public static string currencySelec;
        List<Currency> Currency { get; set; }
        Button btnSave;
        List<Gabana.ORM.Master.MerchantConfig> lstmerchantConfig = new List<Gabana.ORM.Master.MerchantConfig>();
        Gabana.ORM.Master.MerchantConfig merchantConfig = new ORM.Master.MerchantConfig();
        MerchantConfigManage configManage = new MerchantConfigManage();
        string CURRENCYSYMBOLS;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.currency_activity);
                // Create your application here
                currencyActivity = this;

                btnSave = FindViewById<Button>(Resource.Id.btnSave);
                btnSave.Click += BtnSave_Click;

                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += LnBack_Click;

                CheckJwt();

                Currency = new List<Currency> {
                    new Currency(){ CurrencyType= "$", LogoCurrency = Resource.Mipmap.CurrencyUSDg ,LogoCurrency2 =  Resource.Mipmap.CurrencyUSD, CurrencyNameEn = "US Dollar" ,CurrencyNameTh = "ดอลลาร์สหรัฐ"},
                    new Currency(){ CurrencyType= "฿", LogoCurrency = Resource.Mipmap.CurrencyTHBg ,LogoCurrency2 =  Resource.Mipmap.CurrencyTHB, CurrencyNameEn = "Thai Baht" ,CurrencyNameTh = "บาท"},
                    new Currency(){ CurrencyType= "€", LogoCurrency = Resource.Mipmap.CurrencyEURg ,LogoCurrency2 =  Resource.Mipmap.CurrencyEUR, CurrencyNameEn = "Euro",CurrencyNameTh = "ยูโร" },
                    new Currency(){ CurrencyType= "¥", LogoCurrency = Resource.Mipmap.CurrencyJPYg ,LogoCurrency2 =  Resource.Mipmap.CurrencyJPY, CurrencyNameEn = "Japanese Yen" ,CurrencyNameTh = "เยน"},
                    new Currency(){ CurrencyType= "", LogoCurrency = Resource.Mipmap.CurrencyNoG ,LogoCurrency2 =  Resource.Mipmap.CurrencyNo, CurrencyNameEn = "Not Displayed" ,CurrencyNameTh = "ไม่แสดงสกุลเงิน"}
                };
                GabanaModel.gabanaMain.currency = Currency;

                CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
                if (CURRENCYSYMBOLS == null)
                {
                    currencySelec = "";
                }
                else
                {
                    currencySelec = CURRENCYSYMBOLS;
                }

                ShowCurrency();
                _ = TinyInsights.TrackPageViewAsync("OnCreate : CurrencyActivity");

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Currency");
                return;
            }
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            DialogLoading dialogLoading = new DialogLoading();
            if (dialogLoading.Cancelable != false)
            {
                dialogLoading.Cancelable = false;
                dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
            }
            try
            {               
                if (await GabanaAPI.CheckNetWork())
                {
                    if (await GabanaAPI.CheckSpeedConnection())
                    {
                        lstmerchantConfig = new List<Gabana.ORM.Master.MerchantConfig>();
                        merchantConfig = new ORM.Master.MerchantConfig()
                        {
                            MerchantID = DataCashingAll.MerchantId,
                            CfgKey = "CURRENCY_SYMBOLS",
                            CfgString = currencySelec
                        };
                        lstmerchantConfig.Add(merchantConfig);

                        var update = await GabanaAPI.PutDataMerchantConfig(lstmerchantConfig, DataCashingAll.DeviceNo);
                        if (update.Status)
                        {
                            //Insert to Local DB
                            MerchantConfig localConfig = new MerchantConfig()
                            {
                                MerchantID = DataCashingAll.MerchantId,
                                CfgKey = "CURRENCY_SYMBOLS",
                                CfgString = currencySelec
                            };
                            var localVAT = await configManage.InsertorReplacrMerchantConfig(localConfig);
                            if (localVAT)
                            {
                                CURRENCYSYMBOLS = currencySelec;
                                DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS = CURRENCYSYMBOLS;

                                Toast.MakeText(this, GetString(Resource.String.savesucess), ToastLength.Short).Show();
                            }
                        }
                        else
                        {
                            Toast.MakeText(this, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                        }

                        this.Finish();
                    }
                    else
                    {
                        Toast.MakeText(this, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                    }
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                }

                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }

            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnSave_Click at CurrencyActivity");
                return;
            }

        }

        async void UpdateMerchantConfig()
        {
            DialogLoading dialogLoading = new DialogLoading();
            if (dialogLoading.Cancelable != false)
            {
                dialogLoading.Cancelable = false;
                dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
            }
            try
            {          
                lstmerchantConfig = new List<Gabana.ORM.Master.MerchantConfig>();
                merchantConfig = new ORM.Master.MerchantConfig()
                {
                    MerchantID = DataCashingAll.MerchantId,
                    CfgKey = "CURRENCY_SYMBOLS",
                    CfgString = currencySelec
                };
                lstmerchantConfig.Add(merchantConfig);

                var update = await GabanaAPI.PutDataMerchantConfig(lstmerchantConfig, DataCashingAll.DeviceNo);
                if (update.Status)
                {
                    //Insert to Local DB
                    MerchantConfig localConfig = new MerchantConfig()
                    {
                        MerchantID = DataCashingAll.MerchantId,
                        CfgKey = "CURRENCY_SYMBOLS",
                        CfgString = currencySelec
                    };
                    var localVAT = await configManage.InsertorReplacrMerchantConfig(localConfig);
                    if (localVAT)
                    {
                        CURRENCYSYMBOLS = currencySelec;
                        DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS = CURRENCYSYMBOLS;

                        Toast.MakeText(this, GetString(Resource.String.savesucess), ToastLength.Short).Show();
                    }
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                }
                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
            }
            catch (Exception ex)
            {
                dialogLoading.Dismiss();
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("UpdateMerchantConfig at Currency");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }

        }

        private void ShowCurrency()
        {
            listCurrency = new ListCurrency();
            recyclerview_listcurrency = FindViewById<RecyclerView>(Resource.Id.recyclerview_listcurrency);
            Currency_Adapter_Main currency_Adapter_Main = new Currency_Adapter_Main(listCurrency);
            LinearLayoutManager mLayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Vertical, false);
            recyclerview_listcurrency.SetLayoutManager(mLayoutManager);
            recyclerview_listcurrency.SetAdapter(currency_Adapter_Main);
            recyclerview_listcurrency.HasFixedSize = true;
            currency_Adapter_Main.ItemClick += Currency_Adapter_Main_ItemClick;
        }

        private void Currency_Adapter_Main_ItemClick(object sender, int e)
        {

            currencySelec = listCurrency[e].CurrencyType;
            ShowCurrency();
            if (currencySelec != CURRENCYSYMBOLS)
            {
                btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                btnSave.SetBackgroundResource(Resource.Drawable.btnblue);
            }
            else
            {
                btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
                btnSave.SetBackgroundResource(Resource.Drawable.btnborderblue);
            }
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }
        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }

        bool deviceAsleep = false;
#pragma warning disable CS0414 // The field 'CurrencyActivity.openPage' is assigned but its value is never used
        bool openPage = false;
#pragma warning restore CS0414 // The field 'CurrencyActivity.openPage' is assigned but its value is never used
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