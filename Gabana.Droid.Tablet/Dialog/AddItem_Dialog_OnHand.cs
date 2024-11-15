using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Tablet.Fragments.Items;
using Gabana.ShareSource;
using Gabana3.JAM.Trans;
using LinqToDB.SqlQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Dialog
{
    public class AddItem_Dialog_OnHand : AndroidX.Fragment.App.DialogFragment
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
        AddItem_Dialog_OnHand dialog_onhand;
        View view; 
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.additem_dialog_onhand, container, false);
            dialog_onhand = this;
            try
            {
                //Dialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.Transparent);
                CombinUI();
                SetUIEvent();

            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }

            return view;
        }

        private void SetUIEvent()
        {
            lnBack.Click += LnBack_Click;
            btndeletenumber.Click += Btndeletenumber_Click;
            btnSave.Click += BtnSave_Click;
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
            btnClear.Click += BtnClear_Click;
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            txtAmount.Text = "0";
            strValue = txtAmount.Text;
            SetBtnSave();
        }

        private void Btnnumber9_Click(object sender, EventArgs e)
        {
            strValue = txtAmount.Text;
            SetValue(btnnumber9);
        }

        private void Btnnumber8_Click(object sender, EventArgs e)
        {
            strValue = txtAmount.Text;
            SetValue(btnnumber8);
        }

        private void Btnnumber7_Click(object sender, EventArgs e)
        {
            strValue = txtAmount.Text;
            SetValue(btnnumber7);
        }

        private void Btnnumber6_Click(object sender, EventArgs e)
        {
            strValue = txtAmount.Text;
            SetValue(btnnumber6);
        }

        private void Btnnumber5_Click(object sender, EventArgs e)
        {
            strValue = txtAmount.Text;
            SetValue(btnnumber5);
        }

        private void Btnnumber4_Click(object sender, EventArgs e)
        {
            strValue = txtAmount.Text;
            SetValue(btnnumber4);
        }

        private void Btnnumber3_Click(object sender, EventArgs e)
        {
            strValue = txtAmount.Text;
            SetValue(btnnumber3);
        }

        private void Btnnumber2_Click(object sender, EventArgs e)
        {
            strValue = txtAmount.Text;
            SetValue(btnnumber2);
        }

        private void Btnnumber1_Click(object sender, EventArgs e)
        {
            strValue = txtAmount.Text;
            SetValue(btnnumber1);
        }

        private void Btnnumber0_Click(object sender, EventArgs e)
        {
            strValue = txtAmount.Text;
            SetValue(btnnumber0);
        }
        int count = 0;

        public void SetValue(Button btn)
        {
            string amount = "0";
            if (!string.IsNullOrEmpty(txtAmount.Text))
            {
                var datas = Utils.CheckLenghtValue(txtAmount.Text);
                amount = (Convert.ToInt64(datas)).ToString("#,##0");
            }
            else
            {
                amount = "0";
            }
            var num = btn.Text.ToString();
            btn.RequestFocus();

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

            var data = Utils.CheckLenghtValue(amount);
            if (data.Length > 6)
            {
                Toast.MakeText(this.Activity, GetString(Resource.String.maxitem) + " 999,999", ToastLength.Short).Show();
                txtAmount.Text = "999,999";
                strValue = txtAmount.Text;
                return;
            }
            txtAmount.Text = (Convert.ToInt64(data)).ToString("#,##0");
            strValue = txtAmount.Text;
            SetBtnSave();
        }

        private static string ipage;

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtAmount.Text != string.Empty)
                {
                    switch (ipage)
                    {
                        case "additem":
                            Item_Fragment_AddItem.SetOnhand(txtAmount.Text);
                            break;
                        case "addtopping":
                            Item_Fragment_AddTopping.SetOnhand(txtAmount.Text);
                            break;
                        case "POS_item":
                            POS_Dialog_AddItem.SetOnhand(txtAmount.Text);
                            break;
                        case "POS_topping":
                            POS_Dialog_AddTopping.SetOnhand(txtAmount.Text);
                            break;
                        default:
                            break;
                    }
                }
                dialog_onhand.Dismiss();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
                btnSave.Enabled = true;
                return;
            }

        }

        string strValue = "0"; //strValue คือ จำนวนเงินที่จะจ่าย
        private void Btndeletenumber_Click(object sender, EventArgs e)
        {
            try
            {
                decimal damount;
                string amount;
                int indexclear = 0;
                strValue = txtAmount.Text;

                if (strValue.Contains('-'))
                {
                    amount = strValue.Remove(strValue.Length - 1);
                    if (amount == "-")
                    {
                        damount = 0;
                    }
                    else
                    {
                        damount = Convert.ToInt32(amount);
                    }
                    txtAmount.Text = damount.ToString("#,##0");
                    strValue = txtAmount.Text;
                    txtAmount.Focusable = true;
                    indexclear = txtAmount.Text.LastIndexOf(".");
                    return;
                }

                if (strValue != string.Empty && strValue.Length > 1)
                {
                    strValue = Utils.CheckLenghtValue(strValue);
                    strValue = strValue.Remove(strValue.Length - 1);
                    txtAmount.Text = Convert.ToInt64(Utils.CheckLenghtValue(strValue)).ToString("#,##0");
                    txtAmount.Focusable = true;
                    return;
                }

                //กรณีกดลบจนเหลือตัวสุดท้าย ถ้าลบอีกครั้งให้เป็น 1 //ui ให้เปลี่ยนเป็นเหลือ 0
                if (strValue != string.Empty && strValue.Length == 1)
                {
                    strValue = "0";
                    txtAmount.Text = strValue;
                    return;
                }
                SetBtnSave();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }

        }
        private void SetBtnSave()
        {
            if (txtAmount.Text != "")
            {
                btndeletenumber.Enabled = true;
                btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                btnSave.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
            }
            else
            {
                btndeletenumber.Enabled = false;
                btnSave.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.colorrule, null));
                btnSave.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);

            }
        }
        private void LnBack_Click(object sender, EventArgs e)
        {
            dialog_onhand.Dismiss();
        }

        FrameLayout lnBack;
        TextView txtdescrpit_amount, txtAmount;
        Button btnnumber1, btnnumber2, btnnumber3, btnnumber4, btnnumber5, btnnumber6, btnnumber7, btnnumber8, btnnumber9, btnClear, btnnumber0, btnSave;
        ImageButton btndeletenumber;
        private void CombinUI()
        {
            lnBack = view.FindViewById<FrameLayout>(Resource.Id.lnBack);
            txtdescrpit_amount = view.FindViewById<TextView>(Resource.Id.txtdescrpit_amount);
            txtAmount = view.FindViewById<TextView>(Resource.Id.txtAmount);
            btnnumber1 = view.FindViewById<Button>(Resource.Id.btnnumber1);
            btnnumber2 = view.FindViewById<Button>(Resource.Id.btnnumber2);
            btnnumber3 = view.FindViewById<Button>(Resource.Id.btnnumber3);
            btnnumber4 = view.FindViewById<Button>(Resource.Id.btnnumber4);
            btnnumber5 = view.FindViewById<Button>(Resource.Id.btnnumber5);
            btnnumber6 = view.FindViewById<Button>(Resource.Id.btnnumber6);
            btnnumber7 = view.FindViewById<Button>(Resource.Id.btnnumber7);
            btnnumber8 = view.FindViewById<Button>(Resource.Id.btnnumber8);
            btnnumber9 = view.FindViewById<Button>(Resource.Id.btnnumber9);
            btnClear = view.FindViewById<Button>(Resource.Id.btnClear);
            btnnumber0 = view.FindViewById<Button>(Resource.Id.btnnumber0);
            btndeletenumber = view.FindViewById<ImageButton>(Resource.Id.btndeletenumber);
            btnSave = view.FindViewById<Button>(Resource.Id.btnSave);

        }

        internal static void SetPage(string p)
        {
            ipage = p;
        }
    }
}