using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Gabana3.DataModel.Items;
using System;
using System.Collections.Generic;
using TinyInsightsLib;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class AddCashGuideActivity : AppCompatActivity
    {
        public static AddCashGuideActivity addCash;
        ImageButton imagebtnBack, btndeletenumber;
        TextView txtDisCount;
        Button btnpoint, btnnumber1, btnnumber2, btnnumber3, btnnumber4, btnnumber5, btnnumber6, btnnumber7, btnnumber8, btnnumber9, btnnumber0, btnClear;
        internal Button btnApplyCashGuide;
        string strValue;
        int count = 0, DECIMALPOINT = 2;
        public static bool CartDiscount;
        string newDiscount;
        string currency, DECIMALPOINTDISPLAY = "2";
        decimal dec;
        static CashTemplate cashguide;
        private bool first, flagdatachange = false;
        CashTemplateManage CashTemplateManage = new CashTemplateManage();

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.addcashguide_activity_main);

                addCash = this;
                imagebtnBack = FindViewById<ImageButton>(Resource.Id.imagebtnBack);
                btndeletenumber = FindViewById<ImageButton>(Resource.Id.btndeletenumber);
                txtDisCount = FindViewById<TextView>(Resource.Id.txtDisCount);
                btnpoint = FindViewById<Button>(Resource.Id.btnpoint);
                btnnumber0 = FindViewById<Button>(Resource.Id.btnnumber0);
                btnnumber1 = FindViewById<Button>(Resource.Id.btnnumber1);
                btnnumber2 = FindViewById<Button>(Resource.Id.btnnumber2);
                btnnumber3 = FindViewById<Button>(Resource.Id.btnnumber3);
                btnnumber4 = FindViewById<Button>(Resource.Id.btnnumber4);
                btnnumber5 = FindViewById<Button>(Resource.Id.btnnumber5);
                btnnumber6 = FindViewById<Button>(Resource.Id.btnnumber6);
                btnnumber7 = FindViewById<Button>(Resource.Id.btnnumber7);
                btnnumber8 = FindViewById<Button>(Resource.Id.btnnumber8);
                btnnumber9 = FindViewById<Button>(Resource.Id.btnnumber9);
                btnClear = FindViewById<Button>(Resource.Id.btnClear);
                btnApplyCashGuide = FindViewById<Button>(Resource.Id.btnApplyCash);
                btnApplyCashGuide.Click += BtnApplyCashGuide_Click;
                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += ImagebtnBack_Click;

                TextView textTitle = FindViewById<TextView>(Resource.Id.textTitle);
                imagebtnBack.Click += ImagebtnBack_Click;
                btndeletenumber.Click += Btndeletenumber_Click;
                btnpoint.Click += Btnpoint_Click;
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

                CheckJwt();

                if (cashguide == null)
                {
                    txtDisCount.Text = "";
                    txtDisCount.Hint = Utils.DisplayDecimal(0);
                    textTitle.Text = GetString(Resource.String.addcashguide_activity_title);
                    btnApplyCashGuide.Text = GetString(Resource.String.addcashguide_activity_btnadd);
                }
                else
                {
                    txtDisCount.Hint = Utils.DisplayDecimal(cashguide.Amount);
                    txtDisCount.Text = "";
                    textTitle.Text = GetString(Resource.String.addcashguide_activity_titleedit);
                    btnApplyCashGuide.Text = GetString(Resource.String.addcashguide_activity_titleedit);
                    //txtDisCount.Text = Utils.DisplayDecimal(cashguide.Amount);
                }
                strValue = txtDisCount.Text;
                txtDisCount.TextChanged += TxtDisCount_TextChanged;
                DECIMALPOINTDISPLAY = DataCashingAll.setmerchantConfig.DECIMAL_POINT_DISPLAY; //ทศนิยม
                
                btnClear.Click += BtnClear_Click;

                currency = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;

                #region Old Code
                if (!string.IsNullOrEmpty(DECIMALPOINTDISPLAY))
                {
                    if (DECIMALPOINTDISPLAY == "4")
                    {
                        //dec = (decimal)0.0001;
                        DECIMALPOINT = 4;
                    }
                    else
                    {
                        //dec = (decimal)0.01;
                        DECIMALPOINT = 2;
                    }
                }
                else
                {
                    //dec = (decimal)0.01;
                    DECIMALPOINT = 2;
                }
                #endregion

                SetBtnClear();
                first = false;
                SetButtonAdd(false);
                _ = TinyInsights.TrackPageViewAsync("OnCreate : AddCashGuideActivity");

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("AddCashguid");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void TxtDisCount_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            SetBtnClear();
        }
        private async void BtnApplyCashGuide_Click(object sender, EventArgs e)
        {
            DialogLoading dialogLoading = new DialogLoading();
            if (dialogLoading.Cancelable != false)
            {
                dialogLoading.Cancelable = false;
                dialogLoading?.Show(SupportFragmentManager, nameof(DialogLoading));
            }

            if (!await GabanaAPI.CheckNetWork())
            {
                Toast.MakeText(this, GetString(Resource.String.nointernet), ToastLength.Short).Show();
                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
                return;
            }

            if (!await GabanaAPI.CheckSpeedConnection())
            {
                Toast.MakeText(this, GetString(Resource.String.unstableinternet), ToastLength.Long).Show();
                if (dialogLoading != null)
                {
                    dialogLoading.DismissAllowingStateLoss();
                    dialogLoading.Dismiss();
                }
                return;
            }

            if (cashguide == null)
            {
                InsertCashGuide();
            }
            else
            {
                UpdateCashGuide();
            }

            if (dialogLoading != null)
            {
                dialogLoading.DismissAllowingStateLoss();
                dialogLoading.Dismiss();
            }
        }
        async void InsertCashGuide()
        {
            try
            {
                List<CashTemplate> lst = new List<CashTemplate>();
                lst = await CashTemplateManage.GetAllCashTemplate(DataCashingAll.MerchantId);
                if (lst == null)
                {
                    return;
                }
                //กำหนด TypeNo
                int count = lst.Count;

                List<ORM.Master.CashTemplate> lstcash = new List<ORM.Master.CashTemplate>();
                if (string.IsNullOrEmpty(txtDisCount.Text))
                {
                    Toast.MakeText(this, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                    return;
                }

                ORM.Master.CashTemplate cash = new ORM.Master.CashTemplate()
                {
                    DateModified = Utils.GetTranDate(DateTime.UtcNow),
                    Amount = Convert.ToDecimal(strValue),
                    CashTemplateNo = count,
                    MerchantID = DataCashingAll.MerchantId
                };
                lstcash.Add(cash);

                //API
                var insertToAPI = await GabanaAPI.PostDataCashTemplate(lstcash);
                if (insertToAPI == null)
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                    return;
                }

                CashTemplate cashLocal = new CashTemplate()
                {
                    DateModified = insertToAPI[0].DateModified,
                    Amount = Convert.ToDecimal(insertToAPI[0].Amount),
                    CashTemplateNo = insertToAPI[0].CashTemplateNo,
                    MerchantID = insertToAPI[0].MerchantID
                };

                //แล้วเพิ่มใหม่
                var insert = await CashTemplateManage.InsertCashTemplate(cashLocal);
                if (!insert)
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                    return;
                }

                //CashGuideActivity.flagLoadData = true;
                CashGuideActivity.SetFocusCashGuide(cashLocal);
                Toast.MakeText(this, GetString(Resource.String.savesucess), ToastLength.Short).Show();
                this.Finish();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("InsertCashGuide at Cashguid");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        async void UpdateCashGuide()
        {
            try
            {
                List<ORM.Master.CashTemplate> lstcash = new List<ORM.Master.CashTemplate>();
                if (string.IsNullOrEmpty(txtDisCount.Text))
                {
                    Toast.MakeText(this, GetString(Resource.String.notcompletedata), ToastLength.Short).Show();
                    return;
                }

                ORM.Master.CashTemplate cash = new ORM.Master.CashTemplate()
                {
                    DateModified = Utils.GetTranDate(DateTime.UtcNow),
                    Amount = Convert.ToDecimal(strValue),
                    CashTemplateNo = (int)cashguide.CashTemplateNo,
                    MerchantID = DataCashingAll.MerchantId
                };
                lstcash.Add(cash);

                //API
                var insertToAPI = await GabanaAPI.PutDataCashTemplate(lstcash);
                if (!insertToAPI.Status)
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                    return;
                }

                CashTemplate cashLocal = new CashTemplate()
                {
                    DateModified = Utils.GetTranDate(DateTime.UtcNow),
                    Amount = Convert.ToDecimal(strValue),
                    CashTemplateNo = (int)cashguide.CashTemplateNo,
                    MerchantID = DataCashingAll.MerchantId
                };

                //แล้วเพิ่มใหม่
                var update = await CashTemplateManage.UpdateCashTemplate(cashLocal);
                if (!update)
                {
                    Toast.MakeText(this, GetString(Resource.String.cannotsave), ToastLength.Short).Show();
                    return;
                }

                //CashGuideActivity.flagLoadData = true;
                CashGuideActivity.SetFocusCashGuide(cashLocal);
                Toast.MakeText(this, GetString(Resource.String.savesucess), ToastLength.Short).Show();
                this.Finish();
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("UpdateCashGuide at Cashguid");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        private void CheckDataChange()
        {
            if (first)
            {
                SetButtonAdd(false);
                return;
            }
            if (cashguide == null)
            {
                if (!string.IsNullOrEmpty(txtDisCount.Text))
                {
                    if (Convert.ToDecimal(txtDisCount.Text) == 0)
                    {
                        SetButtonAdd(false);
                        return;
                    }
                    else
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }
                }
                SetButtonAdd(false);
            }
            else
            {
                if (string.IsNullOrEmpty(txtDisCount.Text))
                {
                    SetButtonAdd(false);
                    return;
                }
                if (txtDisCount.Text != Utils.DisplayDecimal(cashguide.Amount))
                {
                    if (Convert.ToDecimal(txtDisCount.Text) == 0)
                    {
                        SetButtonAdd(false);
                        return;
                    }
                    else
                    {
                        SetButtonAdd(true);
                        flagdatachange = true;
                        return;
                    }
                }

                SetButtonAdd(false);
            }
        }
        private void SetButtonAdd(bool enable)
        {
            if (enable)
            {
                btnApplyCashGuide.SetBackgroundResource(Resource.Drawable.btnblue);
                btnApplyCashGuide.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnApplyCashGuide.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnApplyCashGuide.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
            }
            btnApplyCashGuide.Enabled = enable;
        }
        private void SetBtnClear()
        {
            if (!string.IsNullOrEmpty(txtDisCount.Text) && Convert.ToDecimal(txtDisCount.Text) > 0)
            {
                btnClear.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
            }
            else
            {
                btnClear.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.colorrule, null));
            }
        }        
        private void BtnClear_Click(object sender, EventArgs e)
        {
            strValue = "0";
            txtDisCount.Text = "";
            txtDisCount.Hint = Utils.DisplayDecimal(0) ;            
            SetBtnClear();
            CheckDataChange();
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
        bool clickPoint = false;
        private void Btnpoint_Click(object sender, EventArgs e)
        {
            clickPoint = true;
            SetValue(btnpoint);
        }
        private void Btndeletenumber_Click(object sender, EventArgs e)
        {
            try
            {
                //ถ้าลบตัวเลขหมดแล้ว ให้เป็น 0.00 ที่ตัวเลขที่แสดง
                //txtDisCount.Hint = 0.00;
                //txtDisCount.Text = string.empty ;

                int indexpoint = strValue.LastIndexOf(".");
                int indexclear = 0;
                decimal damount;
                string amount;

                if (strValue.Contains('%'))
                {
                    amount = strValue.Remove(strValue.Length - 1);
                    damount = Convert.ToDecimal(amount);
                    txtDisCount.Text = Utils.DisplayDecimal(damount);
                    strValue = txtDisCount.Text;
                    txtDisCount.Focusable = true;
                    indexclear = txtDisCount.Text.LastIndexOf(".");
                    return;
                }

                #region Old Code
                //if (strValue != string.Empty && strValue.Length > 1)
                //{
                //    amount = strValue.Remove(strValue.Length - 1);
                //    damount = Convert.ToDecimal(amount);
                //    txtDisCount.Text = Utils.DisplayDecimal(damount);
                //    strValue = txtDisCount.Text;
                //    txtDisCount.Focusable = true;
                //    indexclear = txtDisCount.Text.LastIndexOf(".");
                //}
                //else
                //{
                //    strValue = "";
                //    txtDisCount.Text = strValue;
                //    txtDisCount.Focusable = true;
                //}
                //if (txtDisCount.Text != "")
                //{
                //    damount = Convert.ToDecimal(txtDisCount.Text);
                //    if (DECIMALPOINTDISPLAY == "4")
                //    {
                //        amount = (damount * 1000).ToString();
                //    }
                //    else
                //    {
                //        amount = (damount * 10).ToString();
                //    }
                //}
                //else
                //{
                //    amount = "0";
                //}

                //txtDisCount.Text = Utils.DisplayDecimal(Convert.ToDecimal(amount) * dec);
                //strValue = txtDisCount.Text;

                //if (indexpoint > indexclear)
                //{
                //    count = 0;
                //} 
                #endregion

                if (strValue != string.Empty && strValue.Length > 1)
                {
                    strValue = strValue.Remove(strValue.Length - 1);
                    string str = "";
                    str = strValue;
                    txtDisCount.Text = Utils.DisplayComma(str);
                }
                else
                {
                    strValue = "0";
                    txtDisCount.Text = "";
                    txtDisCount.Hint = Utils.DisplayDecimal(0);
                    txtDisCount.Focusable = true;
                }
                SetBtnClear();
                CheckDataChange();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        private void ImagebtnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
        }
        public void DialogCheckBack()
        {
            base.OnBackPressed();
            flagdatachange = false;
            CartActivity.addDiscount = false;
            CashGuideActivity.flagLoadData = false;
        }
        public override void OnBackPressed()
        {
            try
            {
                if (cashguide == null)
                {
                    if (!flagdatachange)
                    {
                        DialogCheckBack(); return;
                    }

                    MainDialog dialogcashguide = new MainDialog();
                    Bundle bundlecashguide = new Bundle();
                    String myMessagecashguide = Resource.Layout.add_dialog_back.ToString();
                    bundlecashguide.PutString("message", myMessagecashguide);
                    bundlecashguide.PutString("fromPage", "cash");
                    dialogcashguide.Arguments = bundlecashguide;
                    dialogcashguide.Show(SupportFragmentManager, myMessagecashguide);
                    return;
                }

                if (!flagdatachange)
                {
                    DialogCheckBack(); return;
                }

                MainDialog dialog = new MainDialog();
                Bundle bundle = new Bundle();
                String myMessage = Resource.Layout.edit_dialog_back.ToString();
                bundle.PutString("message", myMessage);
                bundle.PutString("fromPage", "cash");
                dialog.Arguments = bundle;
                dialog.Show(SupportFragmentManager, myMessage);
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void SetValue(Button btn)
        {
            try
            {
                #region Old Code
                //if (strValue == "0" && btn.Text != ".")
                //{
                //    strValue = string.Empty;
                //    txtDisCount.Text = strValue;
                //}
                //string amount;
                //if (txtDisCount.Text != "")
                //{
                //    var damount = Convert.ToDouble(txtDisCount.Text);
                //    if (DECIMALPOINTDISPLAY == "4")
                //    {
                //        amount = (damount * 10000).ToString();
                //    }
                //    else
                //    {
                //        amount = (damount * 100).ToString();
                //    }
                //}
                //else
                //{
                //    amount = "0";
                //}
                //var num = btn.Text.ToString();
                //btn.RequestFocus();

                //if (count == 0)
                //{
                //    switch (num)
                //    {
                //        case "0":
                //            amount += num;
                //            break;
                //        case "1":
                //            amount += num;
                //            break;
                //        case "2":
                //            amount += num;
                //            break;
                //        case "3":
                //            amount += num;
                //            break;
                //        case "4":
                //            amount += num;
                //            break;
                //        case "5":
                //            amount += num;
                //            break;
                //        case "6":
                //            amount += num;
                //            break;
                //        case "7":
                //            amount += num;
                //            break;
                //        case "8":
                //            amount += num;
                //            break;
                //        case "9":
                //            amount += num;
                //            break;
                //        default:
                //            amount += num;
                //            count++;
                //            break;
                //    }
                //}
                //else
                //{
                //    switch (num)
                //    {
                //        case "0":
                //            amount += num;
                //            break;
                //        case "1":
                //            amount += num;
                //            break;
                //        case "2":
                //            amount += num;
                //            break;
                //        case "3":
                //            amount += num;
                //            break;
                //        case "4":
                //            amount += num;
                //            break;
                //        case "5":
                //            amount += num;
                //            break;
                //        case "6":
                //            amount += num;
                //            break;
                //        case "7":
                //            amount += num;
                //            break;
                //        case "8":
                //            amount += num;
                //            break;
                //        case "9":
                //            amount += num;
                //            break;
                //        default:
                //            amount += num;
                //            break;
                //    }
                //}

                //txtDisCount.Text = Utils.DisplayDecimal(Convert.ToDecimal(amount) * dec);
                //strValue = txtDisCount.Text;
                //newDiscount = strValue; 
                #endregion

                string str = "";
                if (strValue == "0" && btn.Text != ".")
                {
                    strValue = string.Empty;
                    txtDisCount.Text = strValue;
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
                        if (sp.Length == DECIMALPOINT)
                        {
                            return;
                        }
                    }
                    str = strValue + btn.Text.ToString();
                }
                strValue = str;
                txtDisCount.Text = Utils.DisplayComma(str);

                CheckDataChange();

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
                    Toast.MakeText(this, GetString(Resource.String.maxamount) + " " + maxdata, ToastLength.Short).Show();
                    txtDisCount.Text = maxdata;
                    strValue = txtDisCount.Text;
                    newDiscount = strValue;
                    return;
                }

                SetBtnClear();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetValue at addDiscount");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        internal static void SetCashTemplate(CashTemplate cash)
        {
            cashguide = cash;
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

