using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Adapter.Setting;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Fragments.Setting
{
    public class Setting_Fragment_Currency : AndroidX.Fragment.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Setting_Fragment_Currency NewInstance()
        {
            Setting_Fragment_Currency frag = new Setting_Fragment_Currency();
            return frag;
        }
        View view;
        public static Setting_Fragment_Currency fragment_currency;
        List<Currency> Currency { get; set; }
        string CURRENCYSYMBOLS;
        public static string currencySelec;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            view = inflater.Inflate(Resource.Layout.setting_fragment_currency, container, false);
            try
            {
                fragment_currency = this;
                ComBineUI();
                return view;
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackPageViewAsync("OnCreate at Merchant");
                _ = TinyInsights.TrackErrorAsync(ex);
                return view;
            }
        }

        public override void OnResume()
        {
            base.OnResume();

            //if (!IsVisible)
            //{
            //    return;
            //}

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

            SetBtnSave();
        }

        private void SetBtnSave()
        {
            if (currencySelec != DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS)
            {
                btnSave.Enabled = true;
                btnSave.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnSave.Enabled = true;
                btnSave.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));

            }
        }

        ListCurrency listCurrency;
        private void ShowCurrency()
        {
            listCurrency = new ListCurrency();
            Setting_Adapter_Currency adapter_currency = new Setting_Adapter_Currency(listCurrency);
            LinearLayoutManager mLayoutManager = new LinearLayoutManager(this.Activity, LinearLayoutManager.Vertical, false);
            rcvCurrency.SetLayoutManager(mLayoutManager);
            rcvCurrency.SetAdapter(adapter_currency);
            rcvCurrency.HasFixedSize = true;
            adapter_currency.ItemClick += Adapter_currency_ItemClick; ;
        }

        private void Adapter_currency_ItemClick(object sender, int e)
        {
            currencySelec = listCurrency[e].CurrencyType;
            ShowCurrency();
            SetBtnSave();
        }

        LinearLayout lnBack;
        RecyclerView rcvCurrency ;
        Button btnSave;
        private void ComBineUI()
        {
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            rcvCurrency = view.FindViewById<RecyclerView>(Resource.Id.rcvCurrency);
            btnSave = view.FindViewById<Button>(Resource.Id.btnSave);
            lnBack.Click += LnBack_Click;
            btnSave.Click += BtnSave_Click;
        }

        List<Gabana.ORM.Master.MerchantConfig> lstmerchantConfig = new List<Gabana.ORM.Master.MerchantConfig>();
        Gabana.ORM.Master.MerchantConfig merchantConfig = new ORM.Master.MerchantConfig();
        MerchantConfigManage configManage = new MerchantConfigManage();
        private async void BtnSave_Click(object sender, EventArgs e)
        {
            DialogLoading dialogLoading = new DialogLoading();
            try
            {
                dialogLoading = new DialogLoading();
                dialogLoading.Cancelable = false;
                dialogLoading?.Show(MainActivity.main_activity.SupportFragmentManager, nameof(DialogLoading));
                if (!DataCashing.CheckNet)
                {
                    dialogLoading.Dismiss();
                    Toast.MakeText(this.Activity, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                    return;
                }

                lstmerchantConfig = new List<Gabana.ORM.Master.MerchantConfig>();
                merchantConfig = new ORM.Master.MerchantConfig()
                {
                    MerchantID = DataCashingAll.MerchantId,
                    CfgKey = "CURRENCY_SYMBOLS",
                    CfgString = currencySelec
                };
                lstmerchantConfig.Add(merchantConfig);

                var update = await GabanaAPI.PutDataMerchantConfig(lstmerchantConfig, DataCashingAll.DeviceNo);
                if (!update.Status)
                {
                    dialogLoading.Dismiss();
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                    return;
                }

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

                    Toast.MakeText(this.Activity, GetString(Resource.String.savesucess), ToastLength.Short).Show();
                }

                MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "default");
                Setting_Fragment_Main.SetShowNewData("currency");

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


        private async void LnBack_Click(object sender, EventArgs e)
        {
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "default");
        }
    }
    public class ListCurrency
    {
        public List<Currency> currency;
        static List<Currency> builitem;
        public ListCurrency()
        {
            builitem = GabanaModel.gabanaMain.currency;
            this.currency = builitem;
        }
        public int Count
        {
            get
            {
                return currency == null ? 0 : currency.Count;
            }
        }
        public Currency this[int i]
        {
            get { return currency == null ? null : currency[i]; }
        }
    }

}