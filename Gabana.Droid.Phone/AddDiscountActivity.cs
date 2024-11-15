
using Android.App;
using Android.OS;
using Android.Widget;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using System;
using System.Linq;
using TinyInsightsLib;

namespace Gabana.Droid
{
    [Activity(Label = "@string/app_name", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, Theme = "@style/AppTheme.Main", MainLauncher = false, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Exported = false)]
    public class AddDiscountActivity : Activity
    {
        ImageButton imagebtnBack, btndeletenumber;
        TextView txtDisCount;
        Button btnpoint, btnnumber1, btnnumber2, btnnumber3, btnnumber4,
                btnnumber5, btnnumber6, btnnumber7, btnnumber8, btnnumber9,
                btnnumber0, btnClear, btnApplyDiscount, btnCurrency, btnPercent;
        string strValue;
        char typeDiscount, oldtypeDiscount;
        public static TranDetailItemNew tranDetailItem;
        public static TranWithDetailsLocal tranWithDetails;
        TranTradDiscount tradDiscount = new TranTradDiscount();
        public static bool CartDiscount;
        string newDiscount, tranDiscount;
        string currency, DECIMALPOINTDISPLAY;
        decimal dec;
        int DECIMALPOINT = 2;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                Xamarin.Essentials.Platform.Init(this, savedInstanceState);
                SetContentView(Resource.Layout.adddiscount_activity_main);
                imagebtnBack = FindViewById<ImageButton>(Resource.Id.imagebtnBack);
                btndeletenumber = FindViewById<ImageButton>(Resource.Id.btndeletenumber);
                txtDisCount = FindViewById<TextView>(Resource.Id.txtDisCount);
                txtDisCount.TextChanged += TxtDisCount_TextChanged;
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
                btnApplyDiscount = FindViewById<Button>(Resource.Id.btnApplyDiscount);
                btnClear = FindViewById<Button>(Resource.Id.btnClear);
                btnCurrency = FindViewById<Button>(Resource.Id.btnCurrency);
                btnPercent = FindViewById<Button>(Resource.Id.btnPercent);
                LinearLayout lnBack = FindViewById<LinearLayout>(Resource.Id.lnBack);
                lnBack.Click += ImagebtnBack_Click;
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

                txtDisCount.Hint = Utils.DisplayDecimal(0);
                txtDisCount.Text = strValue;

                DECIMALPOINTDISPLAY = DataCashingAll.setmerchantConfig.DECIMAL_POINT_DISPLAY; //ทศนิยม
                btnClear.Click += BtnClear_Click;
                btnApplyDiscount.Click += BtnApplyDiscount_Click;
                typeDiscount = 'C';
                btnPercent.Click += BtnPercent_Click;
                btnCurrency.Click += BtnCurrency_Click;
                currency = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
                if (string.IsNullOrEmpty(currency))
                {
                    currency = "฿";
                }
                btnCurrency.Text = currency;

                CheckJwt();

                if (!string.IsNullOrEmpty(DECIMALPOINTDISPLAY))
                {
                    if (DECIMALPOINTDISPLAY == "4")
                    {
                        DECIMALPOINT = 4;
                    }
                    else
                    {
                        DECIMALPOINT = 2;
                    }
                }
                else
                {
                    DECIMALPOINT = 2;
                }

                var tranTradDiscount = tranWithDetails.tranTradDiscounts.Where(x => x.DiscountType == "MD").FirstOrDefault();
                if (CartDiscount && tranTradDiscount != null)
                {
                    var check = tranTradDiscount.FmlDiscount.IndexOf('%');
                    if (check == -1)
                    {
                        oldtypeDiscount = 'C';
                        typeDiscount = 'C';
                        tranDiscount = tranTradDiscount.FmlDiscount;
                        txtDisCount.Text = Utils.DisplayDecimal(Convert.ToDecimal(tranDiscount));
                        strValue = txtDisCount.Text;
                    }
                    else
                    {
                        oldtypeDiscount = 'P';
                        typeDiscount = 'P';
                        tranDiscount = tranTradDiscount.FmlDiscount.Remove(check);
                        txtDisCount.Text = Utils.DisplayDecimal(Convert.ToDecimal(tranDiscount));
                        strValue = txtDisCount.Text;
                    }
                }
                if (!CartDiscount && !string.IsNullOrEmpty(tranDetailItem.FmlDiscountRow))
                {
                    var index = tranDetailItem.FmlDiscountRow.IndexOf('%');
                    if (index == -1)
                    {
                        oldtypeDiscount = 'C';
                        typeDiscount = 'C';
                        tranDiscount = tranDetailItem.FmlDiscountRow;
                        txtDisCount.Text = Utils.DisplayDecimal(Convert.ToDecimal(tranDiscount));
                        strValue = txtDisCount.Text;
                    }
                    else
                    {
                        oldtypeDiscount = 'P';
                        typeDiscount = 'P';
                        tranDiscount = tranDetailItem.FmlDiscountRow.Remove(index);
                        txtDisCount.Text = Utils.DisplayDecimal(Convert.ToDecimal(tranDiscount));
                        strValue = txtDisCount.Text;
                    }
                }
                SetBtnClear();
                SetTypeBtnDiscount();
                SetBtnSave();
                _ = TinyInsights.TrackPageViewAsync("OnCreate : AddDiscountActivity");

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("Add Discount");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void TxtDisCount_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            SetBtnClear();
            SetBtnSave();
        }
        private void BtnCurrency_Click(object sender, EventArgs e)
        {
            typeDiscount = 'C';
            SetTypeBtnDiscount();
            SetBtnSave();
        }

        private void BtnPercent_Click(object sender, EventArgs e)
        {
            try
            {
                typeDiscount = 'P';
                SetTypeBtnDiscount();
                decimal maxValue = 100;
                string data;
                if (string.IsNullOrEmpty(strValue))
                {
                    strValue = "0";
                }
                var check = strValue.IndexOf('%');
                if (check == -1)
                {
                    data = strValue;
                }
                else
                {
                    data = strValue.Replace("%", "");
                }
                if (maxValue < Convert.ToDecimal(data))
                {
                    var checklenght = Utils.CheckLenghtValue(strValue);
                    Toast.MakeText(this, GetString(Resource.String.maxdiscount) +
                    Utils.DisplayDouble(maxValue) + "%", ToastLength.Short).Show();
                    txtDisCount.Text = Utils.DisplayDecimal(maxValue);
                    strValue = txtDisCount.Text;
                }
                SetTypeBtnDiscount();
                SetBtnSave();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnPercent_Click at Add Discount");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }
        private async void BtnApplyDiscount_Click(object sender, EventArgs e)
        {
            //AddDiscountDetailItem(TranWithDetailsLocal tranthis, TranDetailItemNew Item, decimal discount, char type)
            //sysItemIDSelect discount ต่อ row
            try
            {
                tradDiscount = tranWithDetails.tranTradDiscounts.Where(x => x.DiscountType == "MD").FirstOrDefault();
                if (CartDiscount)
                {
                    if (tradDiscount == null)
                    {
                        TranTradDiscount dis = new TranTradDiscount()
                        {
                            MerchantID = tranWithDetails.tran.MerchantID,
                            SysBranchID = tranWithDetails.tran.SysBranchID,
                            TranNo = tranWithDetails.tran.TranNo,
                            PriorityNo = 0,
                            FOnTop = 0,
                            DiscountType = "MD",
                            FmlDiscount = newDiscount
                        };
                        tranWithDetails = BLTrans.AddDiscount(tranWithDetails, dis);
                        CartDiscount = false;
                    }
                    else
                    {
                        decimal.TryParse(txtDisCount.Text?.ToString(), out decimal inputDiscount);

                        if (inputDiscount == 0)
                        {
                            tranWithDetails = BLTrans.RemoveDiscount(tranWithDetails, "MD");
                            CartDiscount = false;
                        }

                        if (inputDiscount > 0)
                        {
                            if (tranDiscount != newDiscount || oldtypeDiscount != typeDiscount)
                            {
                                tradDiscount.FmlDiscount = newDiscount;
                                tranWithDetails = BLTrans.RemoveDiscount(tranWithDetails, "MD");
                                TranTradDiscount dis = new TranTradDiscount()
                                {
                                    MerchantID = tranWithDetails.tran.MerchantID,
                                    SysBranchID = tranWithDetails.tran.SysBranchID,
                                    TranNo = tranWithDetails.tran.TranNo,
                                    PriorityNo = 0,
                                    FOnTop = 0,
                                    DiscountType = "MD",
                                    FmlDiscount = newDiscount
                                };
                                tranWithDetails = BLTrans.AddDiscount(tranWithDetails, dis);
                                DataCashing.ModifyTranOrder = true;
                                //tranWithDetails = BLTrans.Caltran(tranWithDetails);
                                CartDiscount = false;
                            }
                        }
                    }
                }
                else
                {
                    tranWithDetails = BLTrans.AddDiscountDetailItem(tranWithDetails, tranDetailItem, Convert.ToDecimal(strValue), typeDiscount);
                    DataCashing.ModifyTranOrder = true;
                    tranWithDetails = BLTrans.Caltran(tranWithDetails);
                }
                if (CartActivity.cart != null)
                {
                    CartActivity.SetTranDetail(tranWithDetails);
                }
                if (CartScanActivity.scan != null)
                {
                    CartScanActivity.SetTranDetail(tranWithDetails);
                }
                this.Finish();

            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("BtnApplyDiscount_Click at Add Discount");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private void SetTypeBtnDiscount()
        {
            if (typeDiscount == 'C')
            {
                newDiscount = txtDisCount.Text;                
                btnPercent.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
                btnPercent.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnCurrency.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                btnCurrency.SetBackgroundResource(Resource.Drawable.btnblue);
            }
            else
            {
                newDiscount = txtDisCount.Text + "%";
                btnPercent.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                btnPercent.SetBackgroundResource(Resource.Drawable.btnblue);
                btnCurrency.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
                btnCurrency.SetBackgroundResource(Resource.Drawable.btnborderblue);
            }
        }

        private void SetBtnClear()
        {
            if (string.IsNullOrEmpty(txtDisCount.Text))
            {
                btnClear.SetTextColor(Resources.GetColor(Resource.Color.colorrule, null));
            }
            else
            {
                btnClear.SetTextColor(Resources.GetColor(Resource.Color.editbluecolor, null));
            }
        }

        private void SetBtnSave()
        {
            if (tranDiscount != txtDisCount.Text?.ToString() || typeDiscount != oldtypeDiscount)
            {
                btnApplyDiscount.Enabled = true;
                btnApplyDiscount.SetBackgroundResource(Resource.Drawable.btnblue);
                btnApplyDiscount.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnApplyDiscount.Enabled = false;
                btnApplyDiscount.SetBackgroundResource(Resource.Drawable.btnborderblue);
                btnApplyDiscount.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.editbluecolor, null));
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtDisCount.Text = "";
            txtDisCount.Hint = Utils.DisplayDecimal(0);
            strValue = "0";
            if (typeDiscount == 'P')
            {
                txtDisCount.Hint = Utils.DisplayDecimal(0) + "%";
            }
            else
            {
                txtDisCount.Hint = Utils.DisplayDecimal(0);
            }
            SetBtnClear();
            SetBtnSave();
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
                    txtDisCount.Hint = "0.00";
                    txtDisCount.Focusable = true;
                }

                SetBtnClear();
                SetTypeBtnDiscount();
                SetBtnSave();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        private void ImagebtnBack_Click(object sender, EventArgs e)
        {
            OnBackPressed();
            CartActivity.addDiscount = false;
        }

        public void SetValue(Button btn)
        {
            try
            {

                string str = "";
                if (string.IsNullOrEmpty(strValue))
                {
                    strValue = "0";
                }
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

                if (typeDiscount == 'C')
                {
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

                    decimal maxdiscount = 0;
                    //discount bill
                    if (CartDiscount)
                    {
                        maxdiscount = tranWithDetails.tranDetailItemWithToppings.Sum(x => x.tranDetailItem.Amount);
                    }
                    else //discount item
                    {
                        if (!string.IsNullOrEmpty(tranDetailItem.FmlDiscountRow))
                        {
                            maxdiscount += Convert.ToDecimal(tranDiscount);
                        }
                        maxdiscount += tranDetailItem.Amount;
                    }

                    if (Convert.ToDecimal(strValue) > maxdiscount)
                    {
                        Toast.MakeText(this, GetString(Resource.String.maxdiscount) +
                                               maxdiscount, ToastLength.Short).Show();
                        txtDisCount.Text = Utils.DisplayDecimal(maxdiscount);
                        strValue = txtDisCount.Text;
                    }
                    if (Convert.ToDecimal(maxdata) < Convert.ToDecimal(strValue))
                    {
                        Toast.MakeText(this, GetString(Resource.String.maxdiscount) +
                                                maxdata, ToastLength.Short).Show();
                        txtDisCount.Text = maxdata;
                        strValue = txtDisCount.Text;
                    }
                }
                else
                {
                    //P % 3 
                    // ค่าสูงสุด 100%
                    decimal maxValue = 100;
                    if (maxValue < Convert.ToDecimal(strValue))
                    {
                        //var checklenght = Utils.CheckLenghtValue(strValue);
                        Toast.MakeText(this, GetString(Resource.String.maxdiscount) + Utils.DisplayDouble(maxValue) + "%", ToastLength.Short).Show();
                        txtDisCount.Text = Utils.DisplayDouble(maxValue);
                        strValue = txtDisCount.Text;
                    }
                }

                SetBtnClear();
                SetTypeBtnDiscount();
                SetBtnSave();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetValue at addDiscount");
                Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
            }
        }

        public static void SetTrtanDetailItem(TranDetailItemNew detailItemNew)
        {
            tranDetailItem = detailItemNew;
        }

        public static void SetTranDetail(TranWithDetailsLocal t)
        {
            tranWithDetails = t;
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

