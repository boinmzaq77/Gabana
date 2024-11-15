using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Fragments.Setting
{
    public class Setting_Fragment_Vat : AndroidX.Fragment.App.Fragment
    {
        public static Setting_Fragment_Vat fragment_vat;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Setting_Fragment_Vat NewInstance()
        {
            Setting_Fragment_Vat frag = new Setting_Fragment_Vat();
            return frag;
        }
        View view;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            view = inflater.Inflate(Resource.Layout.setting_fragment_vat, container, false);
            try
            {
                fragment_vat = this;
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
            try
            {
                base.OnResume();

                //if (!IsVisible)
                //{
                //    return;
                //}

                txtVat.Hint = Convert.ToDecimal(vat).ToString("#,##0.00");
                VatType = DataCashingAll.setmerchantConfig.TAXTYPE;
                decimal Vat = 0;
                if (string.IsNullOrEmpty(DataCashingAll.setmerchantConfig.TAXRATE))
                {
                    Vat = 0;
                }
                else
                {
                    Vat = Convert.ToDecimal(DataCashingAll.setmerchantConfig.TAXRATE);
                }

                if (Vat == 0)
                {
                    txtVat.Text = Vat.ToString("#,##0.00");
                    strValue = "0";
                }
                else
                {
                    txtVat.Text = Vat.ToString("#,##0.00");
                    strValue = txtVat.Text;
                }

                txtVat.TextChanged += TxtVat_TextChanged;
                SetBtnClear();
                taxType = char.Parse(VatType);
                SetBtnTaxType();
                SetBtnSave();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private void TxtVat_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            SetBtnSave();
        }

        LinearLayout lnBack;
        Button btnClear, btnInclude, btnExclude,
                btnnumber1, btnnumber2, btnnumber3, btnnumber4, btnnumber5, btnnumber6, btnnumber7, btnnumber8, btnnumber9,
                btnpoint,btnnumber0,  btnSave;
        ImageButton btndeletenumber;
        TextView txtVat;
        string strValue;
        public static double vat;
        string Vat;
        string VatType;
        private char taxType;

        private void ComBineUI()
        {
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            btnClear = view.FindViewById<Button>(Resource.Id.btnClear);
            txtVat = view.FindViewById<TextView>(Resource.Id.txtVat);

            btnInclude = view.FindViewById<Button>(Resource.Id.btnInclude);
            btnExclude = view.FindViewById<Button>(Resource.Id.btnExclude);

            btnnumber0 = view.FindViewById<Button>(Resource.Id.btnnumber0);
            btnnumber1 = view.FindViewById<Button>(Resource.Id.btnnumber1);
            btnnumber2 = view.FindViewById<Button>(Resource.Id.btnnumber2);
            btnnumber3 = view.FindViewById<Button>(Resource.Id.btnnumber3);
            btnnumber4 = view.FindViewById<Button>(Resource.Id.btnnumber4);
            btnnumber5 = view.FindViewById<Button>(Resource.Id.btnnumber5);
            btnnumber6 = view.FindViewById<Button>(Resource.Id.btnnumber6);
            btnnumber7 = view.FindViewById<Button>(Resource.Id.btnnumber7);
            btnnumber8 = view.FindViewById<Button>(Resource.Id.btnnumber8);
            btnnumber9 = view.FindViewById<Button>(Resource.Id.btnnumber9);

            btnpoint = view.FindViewById<Button>(Resource.Id.btnpoint);
            btnpoint.Click += Btnpoint_Click;

            btndeletenumber = view.FindViewById<ImageButton>(Resource.Id.btndeletenumber);
            btnSave = view.FindViewById<Button>(Resource.Id.btnSave);

            btnnumber0.Click += Btnnumber0_Click;
            btnnumber1.Click += Btnnumber1_Click;
            btnnumber2.Click += Btnnumber2_Click;
            btnnumber3.Click += Btnnumber3_Click;
            btnnumber4.Click += Btnnumber4_Click;
            btnnumber5.Click += Btnnumber5_Click;
            btnnumber6.Click += Btnnumber6_Click;
            btnnumber7.Click += Btnnumber7_Click;
            btnnumber8.Click += Btnnumber8_Click;
            btnnumber9.Click += Btnnumber9_Click;

            lnBack.Click += LnBack_Click;
            btnClear.Click += BtnClear_Click;
            btnSave.Click += BtnSave_Click;
            btndeletenumber.Click += Btndeletenumber_Click;
            btnInclude.Click += BtnInclude_Click;
            btnExclude.Click += BtnExclude_Click;

        }

        bool clickPoint = false;
        private void Btnpoint_Click(object sender, EventArgs e)
        {
            clickPoint = true;
            SetValue(btnpoint);
        }

        private void Btnnumber9_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber9);
        }

        private void Btnnumber8_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber8);
        }

        private void Btnnumber7_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber7);
        }

        private void Btnnumber6_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber6);
        }

        private void Btnnumber5_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber5);
        }

        private void Btnnumber4_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber4);
        }

        private void Btnnumber3_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber3);
        }

        private void Btnnumber2_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber2);
        }

        private void Btnnumber1_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber1);
        }

        private void Btnnumber0_Click(object sender, EventArgs e)
        {
            SetValue(btnnumber0);
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (!await GabanaAPI.CheckNetWork())
            {
                Toast.MakeText(this.Activity, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                return;
            }

            if (!await GabanaAPI.CheckSpeedConnection())
            {
                Toast.MakeText(this.Activity, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                return;
            }

            await UpdateMerchantConfig();
        }

        List<Gabana.ORM.Master.MerchantConfig> lstmerchantConfig = new List<Gabana.ORM.Master.MerchantConfig>();
        Gabana.ORM.Master.MerchantConfig merchantConfig = new ORM.Master.MerchantConfig();
        MerchantConfigManage configManage = new MerchantConfigManage();
        async Task UpdateMerchantConfig()
        {
            try
            {
                string vat = "";
                if (string.IsNullOrEmpty(txtVat.Text))
                {
                    vat = Utils.DisplayDecimal(0);
                }
                else
                {
                    vat = txtVat.Text;
                }
                decimal TAXRATE = Convert.ToDecimal(vat);
                lstmerchantConfig = new List<Gabana.ORM.Master.MerchantConfig>();
                merchantConfig = new ORM.Master.MerchantConfig()
                {
                    MerchantID = DataCashingAll.MerchantId,
                    CfgKey = "TAXRATE",
                    CfgFloat = TAXRATE
                };
                lstmerchantConfig.Add(merchantConfig);

                merchantConfig = new ORM.Master.MerchantConfig()
                {
                    MerchantID = DataCashingAll.MerchantId,
                    CfgKey = "TAXTYPE",
                    CfgString = taxType.ToString()
                };
                lstmerchantConfig.Add(merchantConfig);

                var update = await GabanaAPI.PutDataMerchantConfig(lstmerchantConfig, DataCashingAll.DeviceNo);
                if (update.Status)
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.savesucess), ToastLength.Short).Show();

                    //Insert to Local DB
                    List<MerchantConfig> lstlocal = new List<MerchantConfig>();
                    MerchantConfig localConfig = new MerchantConfig()
                    {
                        MerchantID = DataCashingAll.MerchantId,
                        CfgKey = "TAXRATE",
                        CfgFloat = TAXRATE
                    };
                    lstlocal.Add(localConfig);

                    MerchantConfig localConfig2 = new MerchantConfig()
                    {
                        MerchantID = DataCashingAll.MerchantId,
                        CfgKey = "TAXTYPE",
                        CfgString = taxType.ToString()
                    };
                    lstlocal.Add(localConfig2);

                    var localVAT = await configManage.InsertorReplaceListMerchantConfig(lstlocal);
                    if (localVAT)
                    {
                        Vat = TAXRATE.ToString();
                        VatType = taxType.ToString();

                        DataCashingAll.setmerchantConfig.TAXRATE = Vat;
                        DataCashingAll.setmerchantConfig.TAXTYPE = VatType;

                        MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "default");
                        Setting_Fragment_Main.SetShowNewData("vat");
                    }
                }
                else
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("UpdateMerchantConfig at Vat");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }

        }

        private void Btndeletenumber_Click(object sender, EventArgs e)
        {
            try
            {
                if (strValue != string.Empty && strValue.Length > 1)
                {
                    strValue = strValue.Remove(strValue.Length - 1);
                    string str = "";
                    str = strValue;
                    txtVat.Text = Utils.DisplayComma(str);
                }
                else
                {
                    strValue = "0";
                    txtVat.Text = "";
                    txtVat.Hint = "0.00";
                    txtVat.Focusable = true;
                }
                SetBtnClear();
                SetBtnSave();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }        

        public void SetValue(Button btn)
        {
            try
            {
                string str = "";
                if (strValue == "0" && btn.Text != ".")
                {
                    strValue = string.Empty;
                    txtVat.Text = strValue;
                }

                if (clickPoint)
                {
                    strValue += ".";
                    var data = Utils.AllIndexOf(strValue, ".", StringComparison.Ordinal);
                    if (data.Count > 1)
                    {
                        strValue = strValue.Substring(0, strValue.Length - 1);
                        clickPoint = false;
                        return;
                    }
                    str = strValue;
                    clickPoint = false;
                }
                else
                {
                    if (strValue.Contains("."))
                    {
                        var val = strValue.Split(".");
                        var sp = val[1];
                        if (sp.Length == 2)
                        {
                            return;
                        }
                    }
                    str = strValue + btn.Text.ToString();
                }
                strValue = str;

                #region กดตัวเลขแสดงทศนิยมหลังค่าที่กรอก .00
                //txtVat.Text = (Convert.ToDouble(str)).ToString("#,##0.00");
                //txtVat.Text = Utils.DisplayComma(Convert.ToDecimal(str)); 
                #endregion

                txtVat.Text = Utils.DisplayComma(str);

                SetBtnClear();
                SetBtnSave();

                //// ค่าสูงสุด 99.99 %
                double maxValue = 99.99;
                if (maxValue < Convert.ToDouble(strValue))
                {
                    var checklenght = Utils.CheckLenghtValue(strValue);
                    Toast.MakeText(this.Activity, GetString(Resource.String.maxvat) + " 99.99", ToastLength.Short).Show();
                    txtVat.Text = (9999 * 0.01).ToString("#,##0.00");
                    strValue = txtVat.Text;
                    return;
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetValue at Vat");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }
        private void SetBtnClear()
        {
            if (txtVat.Text != "")
            {
                btnClear.Enabled = true;
                btnClear.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }
            else
            {
                btnClear.Enabled = false;
                btnClear.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.colorrule, null));
            }
        }
        private void SetBtnSave()
        {
            decimal.TryParse(DataCashingAll.setmerchantConfig.TAXRATE , out decimal taxrate );
            decimal.TryParse(txtVat.Text, out decimal taxinput);
            if (taxinput != taxrate || taxType.ToString() != DataCashingAll.setmerchantConfig.TAXTYPE)
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
        private void BtnExclude_Click(object sender, EventArgs e)
        {
            taxType = 'E';
            SetBtnTaxType();
            SetBtnSave();
        }

        private void BtnInclude_Click(object sender, EventArgs e)
        {
            taxType = 'I';
            SetBtnTaxType();
            SetBtnSave();
        }
        private void SetBtnTaxType()
        {
            if (taxType == 'E')
            {
                btnExclude.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                btnExclude.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnInclude.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                btnInclude.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
            }
            else
            {
                btnInclude.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                btnInclude.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnExclude.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                btnExclude.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
            }
        }
        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtVat.Text = "";
            txtVat.Hint = "0.00";
            strValue = "0";
            SetBtnClear();
            SetBtnSave();
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "default");
        }
    }
}