using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Tablet.Fragments.POS;
using Gabana.Model;
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
    public class Cart_Dialog_ChangePrice : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static Cart_Dialog_Option NewInstance()
        {
            var frag = new Cart_Dialog_Option { Arguments = new Bundle() };
            return frag;
        }
        View view;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.cart_dialog_changprice, container, false);
            try
            {
                //Dialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.Transparent);
                CombinUI();

            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }

            _ = TinyInsights.TrackPageViewAsync("OnCreateView : Cart_Dialog_Option");
            return view;
        }

        LinearLayout lnBack;
        Button btnClear;
        TextView txtNewPrice;
        Button btnpoint, btnnumber0, btnnumber1, btnnumber2, btnnumber3, btnnumber4, btnnumber5, btnnumber6, 
            btnnumber7, btnnumber8, btnnumber9;
        LinearLayout btndeletenumber;
        Button btnDone;
        string strValue, DECIMALPOINTDISPLAY;
        int DECIMALPOINT = 2;
        public static TranDetailItemNew itemNew;
        private void CombinUI()
        {
            lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            btnClear = view.FindViewById<Button>(Resource.Id.btnClear);
            txtNewPrice = view.FindViewById<TextView>(Resource.Id.txtNewPrice);
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
            btndeletenumber = view.FindViewById<LinearLayout>(Resource.Id.btndeletenumber);
            btnDone = view.FindViewById<Button>(Resource.Id.btnDone);


            lnBack.Click += LnBack_Click; ;
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


            txtNewPrice.Hint = Utils.DisplayDecimal(0);
            txtNewPrice.Text = strValue;
            btnClear.Click += BtnClear_Click;
            btnDone.Click += BtnDone_Click;

            DECIMALPOINTDISPLAY = DataCashingAll.setmerchantConfig.DECIMAL_POINT_DISPLAY; //ทศนิยม
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

            SetBtnClear();
            SetBtnSave();
        }
        public static TranWithDetailsLocal TranWithDetails;
        private void BtnDone_Click(object sender, EventArgs e)
        {
            try
            {
                decimal price = 0;
                if (string.IsNullOrEmpty(txtNewPrice.Text))
                {
                    price = Convert.ToDecimal("0");
                }
                else
                {
                    price = Convert.ToDecimal(txtNewPrice.Text);
                }
                TranWithDetails = BLTrans.ChangePrice(TranWithDetails, itemNew, price);
                DataCashing.ModifyTranOrder = true;
                TranWithDetails = BLTrans.Caltran(TranWithDetails);

                MainActivity.tranWithDetails = TranWithDetails;
                POS_Fragment_Main.fragment_main.OnResume();
                POS_Fragment_Cart.fragment_cart.OnResume();

                if (POS_Dialog_Scan.scan != null)
                {
                    POS_Dialog_Scan.scan.OnResume();
                }
                this.Dismiss();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnDone_Click at changePass");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                return;
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtNewPrice.Text = "";
            txtNewPrice.Hint = Utils.DisplayDecimal(0);
            strValue = "0";
            SetBtnClear();
            SetBtnSave();
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
                    txtNewPrice.Text = Utils.DisplayComma(str);
                }
                else
                {
                    strValue = "0";
                    txtNewPrice.Text = "";
                    txtNewPrice.Hint = "0.00";
                    txtNewPrice.Focusable = true;
                }

                SetBtnClear();
                SetBtnSave();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }
        private void SetBtnSave()
        {
            if (txtNewPrice.Text != itemNew.TotalPrice.ToString() && txtNewPrice.Text != "")
            {
                btnDone.Enabled = true;
                btnDone.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnDone.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
            }
            else
            {
                btnDone.Enabled = false;
                btnDone.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                btnDone.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
            }
        }
        private void SetBtnClear()
        {
            if (txtNewPrice.Text != "")
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
        public static void SetSysItem(TranDetailItemNew item, TranWithDetailsLocal tranWithDetails)
        {
            itemNew = item;
            TranWithDetails = tranWithDetails;
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
                    txtNewPrice.Text = strValue;
                }

                if (clickPoint)
                {
                    #region กดตัวเลขแสดงทศนิยมหลังค่าที่กรอก .00
                    //if (strValue.Contains(".00"))
                    //{
                    //    var val = strValue.Split(".00");
                    //    strValue = val[0];
                    //} 
                    #endregion
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

                #region กดตัวเลขแสดงทศนิยมหลังค่าที่กรอก .00
                //txtVat.Text = (Convert.ToDouble(str)).ToString("#,##0.00");
                //txtVat.Text = Utils.DisplayComma(Convert.ToDecimal(str)); 
                #endregion

                txtNewPrice.Text = Utils.DisplayComma(str);

                string maxdata;
                if (DECIMALPOINTDISPLAY == "4")
                {
                    maxdata = Utils.DisplayDecimal((decimal)9999999999.9999);
                    //maxdata = Utils.DisplayDecimal((decimal)9999999999999.9999);
                }
                else
                {
                    maxdata = Utils.DisplayDecimal((decimal)9999999999.99);
                    //maxdata = Utils.DisplayDecimal((decimal)9999999999999.99);
                }

                if (Convert.ToDecimal(maxdata) < Convert.ToDecimal(strValue))
                {
                    Toast.MakeText(this.Activity, GetString(Resource.String.maxprice) + maxdata, ToastLength.Short).Show();
                    txtNewPrice.Text = maxdata;
                    strValue = txtNewPrice.Text;
                    return;
                }

                SetBtnClear();
                SetBtnSave();
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetValue at ChangePride");
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }

        private void LnBack_Click(object sender, EventArgs e)
        {
            this.Dismiss();
        }
    }
}