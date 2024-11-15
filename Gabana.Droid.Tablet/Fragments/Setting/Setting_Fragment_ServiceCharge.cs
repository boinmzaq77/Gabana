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
using LinqToDB.SqlQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Fragments.Setting
{
    public class Setting_Fragment_ServiceCharge : AndroidX.Fragment.App.Fragment
    {
        public static Setting_Fragment_ServiceCharge fragment_serviceCharge;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }
        public static Setting_Fragment_ServiceCharge NewInstance()
        {
            Setting_Fragment_ServiceCharge frag = new Setting_Fragment_ServiceCharge();
            return frag;
        }
        View view;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.setting_fragment_servicecharge, container, false);
            try
            {
                fragment_serviceCharge = this;
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

                if (string.IsNullOrEmpty(DataCashingAll.setmerchantConfig.SERVICECHARGE_RATE))
                {
                    SERVICECHARGERATE = "";
                }
                else
                {
                    SERVICECHARGERATE = DataCashingAll.setmerchantConfig.SERVICECHARGE_RATE; //อัตราการคิด service charge
                }

                SERVICECHARGETYPE = DataCashingAll.setmerchantConfig.SERVICECHARGE_TYPE; //รูปแบบการคิด service charge A,B               
                DECIMALPOINTDISPLAY = DataCashingAll.setmerchantConfig.DECIMAL_POINT_DISPLAY; //ทศนิยม
                if (!string.IsNullOrEmpty(DECIMALPOINTDISPLAY))
                {
                    if (DECIMALPOINTDISPLAY == "4")
                    {
                        dec = (decimal)0.0001;
                    }
                    else
                    {
                        dec = (decimal)0.01;
                    }
                }
                else
                {
                    dec = (decimal)0.01;
                }

                txtServiceCharge.Hint = Utils.DisplayDecimal(0) + "%";

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

                if (SERVICECHARGERATE.Contains('%'))
                {
                    txtServiceCharge.Text = servicecharge == 0 ? Utils.DisplayDecimal(0) : Utils.DisplayDecimal(servicecharge) + "%";
                }
                else
                {
                    txtServiceCharge.Text = servicecharge == 0 ? Utils.DisplayDecimal(0) : Utils.DisplayDecimal(servicecharge);
                }

                typeServiceCharge = SERVICECHARGETYPE;
                strValue = txtServiceCharge.Text;

                SetBtnClear();
                SetBtnSave();
                SetTypeBtnService();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        ImageButton imagebtnBack, btndeletenumber;
        TextView txtServiceCharge;
        Button btnPercent, btnnumber1, btnnumber2, btnnumber3, btnnumber4, btnnumber5, btnnumber6, btnnumber7, btnnumber8, btnnumber9, btnnumber0, btnClear, btnSave;
        Button btnAfterDiscount, btnBeforeDiscount;
        public static double newPrice;

        private void ComBineUI()
        {
            imagebtnBack = view.FindViewById<ImageButton>(Resource.Id.imagebtnBack);
            btndeletenumber = view.FindViewById<ImageButton>(Resource.Id.btndeletenumber);
            txtServiceCharge = view.FindViewById<TextView>(Resource.Id.txtServiceCharge);
            btnPercent = view.FindViewById<Button>(Resource.Id.btnPercent);
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
            btnSave = view.FindViewById<Button>(Resource.Id.btnSave);
            btnClear = view.FindViewById<Button>(Resource.Id.btnClear);
            LinearLayout lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            lnBack.Click += ImagebtnBack_Click;

            btndeletenumber.Click += Btndeletenumber_Click;
            btnPercent.Click += BtnPercent_Click;
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

            btnAfterDiscount = view.FindViewById<Button>(Resource.Id.btnAfter);
            btnBeforeDiscount = view.FindViewById<Button>(Resource.Id.btnBefore);
            btnBeforeDiscount.Click += BtnBefore_Click;
            btnAfterDiscount.Click += BtnAfter_Click;

            btnClear.Click += BtnClear_Click;
            btnSave.Click += BtnSave_Click;

        }
        string typeServiceCharge;

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtServiceCharge.Text = "";
            strValue = "0";
            txtServiceCharge.Hint = Utils.DisplayDecimal((decimal)newPrice);
            SetBtnClear();
            SetBtnSave();
        }
        private async void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var checkNet = await GabanaAPI.CheckNetWork();
                if (checkNet)
                {
                    UpdateMerchantConfig();
                }
                else
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("btnSave_Click at ServiceCharge");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }
        private void BtnAfter_Click(object sender, EventArgs e)
        {
            typeServiceCharge = "A";
            SetTypeBtnService();
        }

        private void BtnBefore_Click(object sender, EventArgs e)
        {
            typeServiceCharge = "B";
            SetTypeBtnService();
        }
        private void SetTypeBtnService()
        {
            if (typeServiceCharge == "A")
            {
                btnAfterDiscount.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                btnAfterDiscount.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnBeforeDiscount.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                btnBeforeDiscount.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
            }
            else
            {
                btnBeforeDiscount.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                btnBeforeDiscount.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnAfterDiscount.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                btnAfterDiscount.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
            }
        }

        List<Gabana.ORM.Master.MerchantConfig> lstmerchantConfig = new List<Gabana.ORM.Master.MerchantConfig>();
        Gabana.ORM.Master.MerchantConfig merchantConfig = new ORM.Master.MerchantConfig();
        MerchantConfigManage configManage = new MerchantConfigManage();
        async void UpdateMerchantConfig()
        {
            try
            {
                lstmerchantConfig = new List<Gabana.ORM.Master.MerchantConfig>();
                merchantConfig = new ORM.Master.MerchantConfig()
                {
                    MerchantID = DataCashingAll.MerchantId,
                    CfgKey = "SERVICECHARGE_TYPE",
                    CfgString = typeServiceCharge
                };
                lstmerchantConfig.Add(merchantConfig);

                merchantConfig = new ORM.Master.MerchantConfig()
                {
                    MerchantID = DataCashingAll.MerchantId,
                    CfgKey = "SERVICECHARGE_RATE",
                    CfgString = strValue
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
                        CfgKey = "SERVICECHARGE_TYPE",
                        CfgString = typeServiceCharge
                    };
                    lstlocal.Add(localConfig);

                    MerchantConfig localConfig2 = new MerchantConfig()
                    {
                        MerchantID = DataCashingAll.MerchantId,
                        CfgKey = "SERVICECHARGE_RATE",
                        CfgString = strValue
                    };
                    lstlocal.Add(localConfig2);

                    var localVAT = await configManage.InsertorReplaceListMerchantConfig(lstlocal);
                    if (localVAT)
                    {
                        SERVICECHARGERATE = strValue;
                        SERVICECHARGETYPE = typeServiceCharge.ToString();

                        DataCashingAll.setmerchantConfig.SERVICECHARGE_RATE = SERVICECHARGERATE;
                        DataCashingAll.setmerchantConfig.SERVICECHARGE_TYPE = SERVICECHARGETYPE;

                        MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "default");
                        Setting_Fragment_Main.SetShowNewData("servicechange");
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
                _ = TinyInsights.TrackPageViewAsync("UpdateMerchantConfig at ServiceCharge");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }


        private void Btnnumber9_Click(object sender, EventArgs e)
        {
            strValue = txtServiceCharge.Text;
            SetValue(btnnumber9);
        }

        private void Btnnumber8_Click(object sender, EventArgs e)
        {
            strValue = txtServiceCharge.Text;
            SetValue(btnnumber8);
        }

        private void Btnnumber7_Click(object sender, EventArgs e)
        {
            strValue = txtServiceCharge.Text;
            SetValue(btnnumber7);
        }

        private void Btnnumber6_Click(object sender, EventArgs e)
        {
            strValue = txtServiceCharge.Text;
            SetValue(btnnumber6);
        }

        private void Btnnumber5_Click(object sender, EventArgs e)
        {
            strValue = txtServiceCharge.Text;
            SetValue(btnnumber5);
        }

        private void Btnnumber4_Click(object sender, EventArgs e)
        {
            strValue = txtServiceCharge.Text;
            SetValue(btnnumber4);
        }

        private void Btnnumber3_Click(object sender, EventArgs e)
        {
            strValue = txtServiceCharge.Text;
            SetValue(btnnumber3);
        }

        private void Btnnumber2_Click(object sender, EventArgs e)
        {
            strValue = txtServiceCharge.Text;
            SetValue(btnnumber2);
        }

        private void Btnnumber1_Click(object sender, EventArgs e)
        {
            strValue = txtServiceCharge.Text;
            SetValue(btnnumber1);
        }
        private void Btnnumber0_Click(object sender, EventArgs e)
        {
            strValue = txtServiceCharge.Text;
            SetValue(btnnumber0);
        }
        private void ImagebtnBack_Click(object sender, EventArgs e)
        {
            MainActivity.main_activity.LoadFragmentMain(Resource.Id.lnSetting, "setting", "default");
        }

        string strValue = Utils.DisplayDecimal(0);
        string SERVICECHARGERATE, DECIMALPOINTDISPLAY, SERVICECHARGETYPE;
        int count = 0;
        decimal dec;

        private void Btndeletenumber_Click(object sender, EventArgs e)
        {
            try
            {
                int indexpoint = strValue.LastIndexOf(".");
                int indexclear = 0;
                decimal damount;
                string amount;

                if (strValue.Contains('%'))
                {
                    amount = strValue.Remove(strValue.Length - 1);
                    damount = Convert.ToDecimal(amount);
                    //txtServiceCharge.Text = Utils.DisplayDecimal(damount * (decimal)0.01);
                    txtServiceCharge.Text = Utils.DisplayDecimal(damount);
                    strValue = txtServiceCharge.Text;
                    txtServiceCharge.Focusable = true;
                    indexclear = txtServiceCharge.Text.LastIndexOf(".");
                    return;
                }

                if (strValue != string.Empty && strValue.Length > 1)
                {
                    amount = strValue.Remove(strValue.Length - 1);
                    damount = Convert.ToDecimal(amount);
                    txtServiceCharge.Text = Utils.DisplayDecimal(damount);
                    strValue = txtServiceCharge.Text;
                    txtServiceCharge.Focusable = true;
                    indexclear = txtServiceCharge.Text.LastIndexOf(".");
                }
                else
                {
                    strValue = "";
                    txtServiceCharge.Text = strValue;
                    txtServiceCharge.Focusable = true;
                }

                if (txtServiceCharge.Text != "")
                {
                    damount = Convert.ToDecimal(txtServiceCharge.Text);
                    if (DECIMALPOINTDISPLAY == "4")
                    {
                        amount = (damount * 1000).ToString();
                    }
                    else
                    {
                        amount = (damount * 10).ToString();
                    }
                }
                else
                {
                    amount = "0";
                }

                txtServiceCharge.Text = Utils.DisplayDecimal(Convert.ToDecimal(amount) * dec);
                strValue = txtServiceCharge.Text;

                if (indexpoint > indexclear)
                {
                    count = 0;
                }
                SetBtnClear();
                SetBtnSave();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }
        private void SetBtnClear()
        {
            if (txtServiceCharge.Text != "")
            {
                btndeletenumber.Enabled = true;
                btnClear.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }
            else
            {
                btndeletenumber.Enabled = false;
                btnClear.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.colorrule, null));
            }
        }
        private void SetBtnSave()
        {
            try
            {
                var index = strValue.IndexOf('%');
                string service = txtServiceCharge.Text;
                if (index != -1)
                {
                    service = strValue.Remove(index);
                }

                var index2 = SERVICECHARGERATE.IndexOf('%');
                string service2 = SERVICECHARGERATE;
                if (index2 != -1)
                {
                    service2 = DataCashingAll.setmerchantConfig.SERVICECHARGE_RATE.Remove(index2);
                }

                decimal servicechargeNew = Convert.ToDecimal(string.IsNullOrEmpty(service) ? "0" : service);
                decimal servicechargeOld = Convert.ToDecimal(string.IsNullOrEmpty(service2) ? "0" : service2);

                if (servicechargeNew != servicechargeOld)
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
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("UpdateMerchantConfig at ServiceCharge");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private void BtnPercent_Click(object sender, EventArgs e)
        {
            try
            {
                strValue = txtServiceCharge.Text;
                if (string.IsNullOrEmpty(strValue))
                {
                    strValue = "0";
                }

                if (!strValue.Contains("%"))
                {
                    SetValue(btnPercent);
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("Btnpoint_Click at ServiceCharge");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }
        public void SetValue(Button btn)
        {
            try
            {
                if (strValue == "0" && btn.Text != ".")
                {
                    strValue = string.Empty;
                    txtServiceCharge.Text = strValue;
                }
                string amount;
                if (txtServiceCharge.Text != "")
                {
                    if (txtServiceCharge.Text.Contains('%'))
                    {
                        var data = txtServiceCharge.Text.Replace("%", "");
                        var damount = Convert.ToDouble(data);
                        amount = (damount * 10000).ToString("#,###.####") + "%";
                    }
                    else
                    {
                        var damount = Convert.ToDouble(txtServiceCharge.Text);
                        if (DECIMALPOINTDISPLAY == "4")
                        {
                            amount = (damount * 10000).ToString();
                        }
                        else
                        {
                            amount = (damount * 100).ToString();
                        }
                    }
                }
                else
                {
                    amount = "0";
                }
                var num = btn.Text.ToString();
                btn.RequestFocus();

                if (count == 0)
                {
                    switch (num)
                    {
                        case "0":
                            amount += num;
                            break;
                        case "1":
                            amount += num;
                            break;
                        case "2":
                            amount += num;
                            break;
                        case "3":
                            amount += num;
                            break;
                        case "4":
                            amount += num;
                            break;
                        case "5":
                            amount += num;
                            break;
                        case "6":
                            amount += num;
                            break;
                        case "7":
                            amount += num;
                            break;
                        case "8":
                            amount += num;
                            break;
                        case "9":
                            amount += num;
                            break;
                        default:
                            amount += num;
                            count++;
                            break;
                    }
                }
                else
                {
                    switch (num)
                    {
                        case "0":
                            amount += num;
                            break;
                        case "1":
                            amount += num;
                            break;
                        case "2":
                            amount += num;
                            break;
                        case "3":
                            amount += num;
                            break;
                        case "4":
                            amount += num;
                            break;
                        case "5":
                            amount += num;
                            break;
                        case "6":
                            amount += num;
                            break;
                        case "7":
                            amount += num;
                            break;
                        case "8":
                            amount += num;
                            break;
                        case "9":
                            amount += num;
                            break;
                        default:
                            amount += num;
                            break;
                    }
                }

                if (amount.Contains('%'))
                {
                    var data = amount.Replace("%", "");
                    txtServiceCharge.Text = Utils.DisplayDecimal(Convert.ToDecimal(data) * dec) + "%";
                    strValue = txtServiceCharge.Text;
                }
                else
                {
                    txtServiceCharge.Text = Utils.DisplayDecimal(Convert.ToDecimal(amount) * dec);
                    strValue = txtServiceCharge.Text;
                }

                SetBtnClear();
                SetBtnSave();

                //Percent
                if (strValue.Contains('%'))
                {
                    decimal maxValue = 100;
                    var data = strValue.Replace("%", "");
                    if (maxValue < Convert.ToDecimal(data))
                    {
                        Toast.MakeText(this.Activity, GetString(Resource.String.maxservicecharg) + " " + Utils.DisplayDecimal(maxValue) + "%", ToastLength.Short).Show();
                        txtServiceCharge.Text = Utils.DisplayDecimal(maxValue) + "%";
                        strValue = txtServiceCharge.Text;
                        return;
                    }
                }
                else
                {
                    //Currency
                    // 6 หลัก 999,999.displayDecimal  
                    string maxdata;
                    if (DECIMALPOINTDISPLAY == "4")
                    {
                        maxdata = Utils.DisplayDecimal((decimal)999999.9999);
                    }
                    else
                    {
                        maxdata = Utils.DisplayDecimal((decimal)999999.99);
                    }

                    if (Convert.ToDecimal(maxdata) < Convert.ToDecimal(strValue))
                    {
                        Toast.MakeText(this.Activity, GetString(Resource.String.maxservicecharg) + " " + maxdata, ToastLength.Short).Show();
                        txtServiceCharge.Text = maxdata;
                        strValue = txtServiceCharge.Text;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetValue at ServiceCharge");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

    }
}