using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Tablet.Fragments.POS;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana3.JAM.Trans;
using LinqToDB.SqlQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Dialog
{
    public class POS_Dialog_Discount : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static POS_Dialog_Discount NewInstance()
        {
            var frag = new POS_Dialog_Discount { Arguments = new Bundle() };
            return frag;
        }
        public static POS_Dialog_Discount dialog_discount;
        View view;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.pos_dialog_discount, container, false);
            try
            {
                CombinUI();
                SetEventUI();
                txtDisCount.Hint = Utils.DisplayDecimal(0);
                txtDisCount.Text = strValue;

                DECIMALPOINTDISPLAY = DataCashingAll.setmerchantConfig?.DECIMAL_POINT_DISPLAY; //ทศนิยม
                typeDiscount = 'C';
                currency = DataCashingAll.setmerchantConfig?.CURRENCY_SYMBOLS;
                if (string.IsNullOrEmpty(currency))
                {
                    currency = "฿";
                }
                btnCurrency.Text = currency;
                tranWithDetails = MainActivity.tranWithDetails;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }

            _ = TinyInsights.TrackPageViewAsync("OnCreateView : POS_Dialog_Discount");
            return view;
        }        

        LinearLayout lnBack;
        Button btnpoint, btnnumber0, btnnumber1, btnnumber2, btnnumber3, btnnumber4, btnnumber5, btnnumber6,
            btnnumber7, btnnumber8, btnnumber9, btnClear, btnCurrency, btnPercent, btnApplyDiscount;
        string currency, DECIMALPOINTDISPLAY;
        LinearLayout btndeletenumber;
        TextView txtDisCount;
        private void CombinUI()
        {
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            btnClear = view.FindViewById<Button>(Resource.Id.btnClear);
            btnpoint = view.FindViewById<Button>(Resource.Id.btnpoint);
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
            btnCurrency = view.FindViewById<Button>(Resource.Id.btnCurrency);
            btnPercent = view.FindViewById<Button>(Resource.Id.btnPercent);
            btnApplyDiscount = view.FindViewById<Button>(Resource.Id.btnApplyDiscount);
            btndeletenumber = view.FindViewById<LinearLayout>(Resource.Id.btndeletenumber);
            txtDisCount = view.FindViewById<TextView>(Resource.Id.txtDisCount);
        }

        private void SetEventUI()
        {
            txtDisCount.TextChanged += TxtDisCount_TextChanged;
            lnBack.Click += LnBack_Click;
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
            btnApplyDiscount.Click += BtnApplyDiscount_Click;
            btnClear.Click += BtnClear_Click;
            btnPercent.Click += BtnPercent_Click;
            btnCurrency.Click += BtnCurrency_Click;
        }

        public static void SetTrtanDetailItem(TranDetailItemNew detailItemNew)
        {
            tranDetailItem = detailItemNew;
        }

        private void SetTypeBtnDiscount()
        {
            if (typeDiscount == 'C')
            {
                newDiscount = txtDisCount.Text;
                btnPercent.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                btnPercent.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnCurrency.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                btnCurrency.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
            }
            else
            {
                newDiscount = txtDisCount.Text + "%";
                btnPercent.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                btnPercent.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnCurrency.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                btnCurrency.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
            }
        }
        private void BtnCurrency_Click(object sender, EventArgs e)
        {
            typeDiscount = 'C';
            SetTypeBtnDiscount();
            SetBtnSave();
            txtDisCount.Hint = strValue;
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
                    Toast.MakeText(this.Activity, GetString(Resource.String.maxdiscount) +
                    Utils.DisplayDouble(maxValue) + "%", ToastLength.Short).Show();
                    txtDisCount.Text = Utils.DisplayDecimal(maxValue);
                    strValue = txtDisCount.Text;
                }
                SetTypeBtnDiscount();
                SetBtnSave();
                txtDisCount.Hint = strValue;

            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnPercent_Click at Add Discount");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }
        public static TranWithDetailsLocal tranWithDetails;
        public static TranDetailItemNew tranDetailItem;
        TranTradDiscount tradDiscount = new TranTradDiscount();
        public static bool CartDiscount; //CartDiscount = true -> มาจากการกดส่วนลดท้ายบิล , CartDiscount = false -> มาจากการกดส่วนลดต่อสินค้า
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

                MainActivity.tranWithDetails = tranWithDetails;
                POS_Fragment_Main.fragment_main.OnResume();
                POS_Fragment_Cart.fragment_cart.OnResume();

                if (POS_Dialog_Scan.scan != null)
                {
                    POS_Dialog_Scan.scan.OnResume();
                }

                if (Cart_Dialog_Option.cart_optiion != null)
                {
                    Cart_Dialog_Option.cart_optiion.Dismiss();
                }
                this.Dialog.Dismiss();                                
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                await TinyInsights.TrackPageViewAsync("BtnApplyDiscount_Click at Add Discount");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        int DECIMALPOINT = 2;
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
                    decimal result = 0;
                    decimal.TryParse(txtDisCount.Hint, out result);
                    if (result != 0)
                    {
                        strValue = string.Empty;
                        txtDisCount.Hint = "0.00";
                    }

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
                        Toast.MakeText(this.Activity, GetString(Resource.String.maxdiscount) +
                                               maxdiscount, ToastLength.Short).Show();
                        txtDisCount.Text = Utils.DisplayDecimal(maxdiscount);
                        strValue = txtDisCount.Text;
                    }
                    if (Convert.ToDecimal(maxdata) < Convert.ToDecimal(strValue))
                    {
                        Toast.MakeText(this.Activity, GetString(Resource.String.maxdiscount) +
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
                        Toast.MakeText(this.Activity, GetString(Resource.String.maxdiscount) + Utils.DisplayDouble(maxValue) + "%", ToastLength.Short).Show();
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
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
        private void TxtDisCount_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            SetBtnClear();
            SetBtnSave();
        }
        string newDiscount, tranDiscount;
        char typeDiscount, oldtypeDiscount;
        private void SetBtnSave()
        {
            if (tranDiscount != txtDisCount.Text?.ToString() || typeDiscount != oldtypeDiscount)
            {
                btnApplyDiscount.Enabled = true;
                btnApplyDiscount.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnApplyDiscount.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnApplyDiscount.Enabled = false;
                btnApplyDiscount.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnApplyDiscount.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
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
                btnClear.SetTextColor(Resources.GetColor(Resource.Color.primaryDark, null));
            }
        }
        string strValue;
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
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }
        private void LnBack_Click(object sender, EventArgs e)
        {
            POS_Fragment_Cart.addDiscount = false;
            this.Dismiss();
        }
    }
}