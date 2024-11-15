using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Adapter.Option;
using Gabana.Droid.Tablet.Adapter.Pos;
using Gabana.Droid.Tablet.Fragments.POS;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana.ShareSource.Manage;
using Google.Android.Material.Chip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyInsightsLib;

namespace Gabana.Droid.Tablet.Dialog
{
    public class POS_Dialog_Quantity : AndroidX.Fragment.App.DialogFragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
        public static POS_Dialog_Quantity NewInstance()
        {
            var frag = new POS_Dialog_Quantity { Arguments = new Bundle() };
            return frag;
        }

        View view;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.pos_dialog_quantity, container, false);
            try
            {
                //Dialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.Transparent);
                CombinUI();
                setQuanity = backtoQuantity == null ? 0 : backtoQuantity.Value;
                if (setQuanity != 0)
                {
                    strcurrent = setQuanity.ToString();
                }
                else
                {
                    strcurrent = "1";
                }
                strcurrent = Utils.CheckLenghtValue(strcurrent);
                editValue.Text = (Convert.ToInt32(strcurrent)).ToString("#,##0");
                btnOK.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                btnOK.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));

                editValue.Focusable = true;
                editValue.TextChanged += EditValue_TextChanged1;
                strcurrent = string.Empty;

                _ = TinyInsights.TrackPageViewAsync("OnCreateView : POS_Dialog_Quantity");
                return view;

            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("at quantity");
                Toast.MakeText(this.Context, ex.Message, ToastLength.Short).Show();
                return view;
            }
        }
        private void EditValue_TextChanged1(object sender, Android.Text.TextChangedEventArgs e)
        {
            try
            {
                decimal value = 0;
                if (Decimal.TryParse(editValue.Text, out value))
                {
                    if (value > 0)
                    {
                        btnOK.Enabled = true;
                        btnOK.SetBackgroundResource(Resource.Drawable.btnBluerectangle);
                        btnOK.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.textIcon, null));
                    }
                    else
                    {
                        btnOK.Enabled = false;
                        btnOK.SetBackgroundResource(Resource.Drawable.btnWhiteBorderBlueRD5);
                        btnOK.SetTextColor(Application.Context.Resources.GetColor(Resource.Color.primaryDark, null));
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        ImageButton  btndeletenumber, imgDecrease, imgIncrease;
        Button btnOK, btnnumber1, btnnumber2, btnnumber3, btnnumber4, btnnumber5, btnnumber6, btnnumber7, btnnumber8, btnnumber9, btnnumber0;
        TextView editValue;
        string strcurrent;
        public POS_Dialog_Quantity quantity;
        public int setQuanity;
        private static int? backtoQuantity;
        internal static void SetBackQuantity(int? i)
        {
            backtoQuantity = i;
        }
        private void CombinUI()
        {

            imgDecrease = view.FindViewById<ImageButton>(Resource.Id.imageButtonDecrease);
            imgIncrease = view.FindViewById<ImageButton>(Resource.Id.imageButtonIncrease);
            editValue = view.FindViewById<TextView>(Resource.Id.editTextQuantity);
            btnOK = view.FindViewById<Button>(Resource.Id.buttonOK);
            btndeletenumber = view.FindViewById<ImageButton>(Resource.Id.btndeletenumber);
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

            LinearLayout lnBack = view.FindViewById<LinearLayout>(Resource.Id.lnBack);
            lnBack.Click += ImgBack_Click;
            btnOK.Click += BtnOK_Click;
            imgDecrease.Click += ImgDecrease_Click;
            imgIncrease.Click += ImgIncrease_Click;
            btndeletenumber.Click += Btndeletenumber_Click;

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
        }
        private void Btnnumber9_Click(object sender, EventArgs e)
        {
            editValue.Text = strcurrent;
            editValue.Focusable = true;
            SetQuantity(btnnumber9);
        }

        private void Btnnumber8_Click(object sender, EventArgs e)
        {
            editValue.Text = strcurrent;
            editValue.Focusable = true;
            SetQuantity(btnnumber8);
        }

        private void Btnnumber7_Click(object sender, EventArgs e)
        {
            editValue.Text = strcurrent;
            editValue.Focusable = true;
            SetQuantity(btnnumber7);
        }

        private void Btnnumber6_Click(object sender, EventArgs e)
        {
            editValue.Text = strcurrent;
            editValue.Focusable = true;
            SetQuantity(btnnumber6);
        }

        private void Btnnumber5_Click(object sender, EventArgs e)
        {
            editValue.Text = strcurrent;
            editValue.Focusable = true;
            SetQuantity(btnnumber5);
        }

        private void Btnnumber4_Click(object sender, EventArgs e)
        {
            editValue.Text = strcurrent;
            editValue.Focusable = true;
            SetQuantity(btnnumber4);
        }

        private void Btnnumber3_Click(object sender, EventArgs e)
        {
            editValue.Text = strcurrent;
            editValue.Focusable = true;
            SetQuantity(btnnumber3);
        }

        private void Btnnumber2_Click(object sender, EventArgs e)
        {
            editValue.Text = strcurrent;
            editValue.Focusable = true;
            SetQuantity(btnnumber2);
        }

        private void Btnnumber1_Click(object sender, EventArgs e)
        {
            editValue.Text = strcurrent;
            editValue.Focusable = true;
            SetQuantity(btnnumber1);
        }

        private void Btnnumber0_Click(object sender, EventArgs e)
        {
            editValue.Text = strcurrent;
            editValue.Focusable = true;
            SetQuantity(btnnumber0);
        }
        private async void ImgDecrease_Click(object sender, EventArgs e)
        {
            try
            {
                int temp;
                strcurrent = editValue.Text;
                strcurrent = Utils.CheckLenghtValue(strcurrent);
                switch (strcurrent)
                {
                    case "":
                        editValue.Text = string.Empty;
                        break;
                    case "0":
                        editValue.Text = "0";
                        break;
                    case "1":
                        editValue.Text = "1";
                        break;
                    default:
                        temp = int.Parse(strcurrent);
                        strcurrent = temp - 1 + "";
                        editValue.Text = Convert.ToInt64(strcurrent).ToString("#,##0");
                        break;
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ImgDecrease_Click at Quantity");
                Log.Debug("error", ex.Message);
                Console.WriteLine(ex.Message);
            }
        }
        private void ImgIncrease_Click(object sender, EventArgs e)
        {
            try
            {
                int temp;
                strcurrent = editValue.Text;
                if (strcurrent == string.Empty)
                {
                    strcurrent = "1";
                    editValue.Text = Convert.ToInt32(strcurrent).ToString("#,##0");
                }
                else
                {
                    strcurrent = Utils.CheckLenghtValue(strcurrent);
                    temp = int.Parse(strcurrent);
                    strcurrent = temp + 1 + "";
                    if (Convert.ToInt32(strcurrent) > 999999)
                    {
                        editValue.Text = 999999.ToString("#,##0");
                        strcurrent = "999999";
                    }
                    else
                    {
                        editValue.Text = Convert.ToInt32(strcurrent).ToString("#,##0");
                    }
                }
            }
            catch (Exception ex)
            {
                _ = TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("ImgIncrease_Click at Quantity");
                Log.Debug("error", ex.Message);
                Console.WriteLine(ex.Message);
            }
        }
        private void Btndeletenumber_Click(object sender, EventArgs e)
        {
            strcurrent = editValue.Text.Trim();
            if (strcurrent != string.Empty && strcurrent.Length > 1)
            {
                strcurrent = Utils.CheckLenghtValue(strcurrent);
                strcurrent = strcurrent.Remove(strcurrent.Length - 1);
                editValue.Text = Convert.ToInt64(Utils.CheckLenghtValue(strcurrent)).ToString("#,##0");
                editValue.Focusable = true;
                return;
            }

            //กรณีกดลบจนเหลือตัวสุดท้าย ถ้าลบอีกครั้งให้เป็น 1 //ui ให้เปลี่ยนเป็นเหลือ 0
            if (strcurrent != string.Empty && strcurrent.Length == 1)
            {
                strcurrent = "0";
                editValue.Text = strcurrent;
                editValue.Focusable = true;
                return;
            }
        }

        private void ImgBack_Click(object sender, EventArgs e)
        {
            if (DataCashing.flagEditQuantity)
            {
                DataCashing.setQuantityToCart = 1;
                DataCashing.flagEditQuantity = false;
            }
            else
            {
                POS_Fragment_Main.setQuantity = 1;
            }
            Dismiss();
        }
        private async void BtnOK_Click(object sender, EventArgs e)
        {
            try
            {
                decimal value = 0, data = 0;
                if (Decimal.TryParse(editValue.Text, out value))
                {
                    if (value > 0)
                    {
                        data = value;
                    }
                    else
                    {
                        data = 0;
                    }
                }

                if (DataCashing.flagEditQuantity)
                {
                    setQuanity = (int)data;
                    DataCashing.EditQuantity = setQuanity;
                    POS_Fragment_Cart.fragment_cart.OnResume();
                    if (POS_Dialog_Scan.scan != null)
                    {
                        POS_Dialog_Scan.scan.OnResume();
                    }                    
                    Dismiss();
                }
                else
                {
                    setQuanity = (int)data;
                    POS_Fragment_Main.setQuantity = setQuanity;
                    POS_Fragment_Main.fragment_main.SettextQuantity(setQuanity);
                    POS_Fragment_Cart.fragment_cart.OnResume();
                    if (POS_Dialog_Scan.scan != null)
                    {
                        POS_Dialog_Scan.scan.OnResume();
                    }
                    DataCashing.setQuantityToCart = setQuanity;
                    Dismiss();
                }
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("BtnOK_Click at Quantity");
                Log.Debug("error", ex.Message);
                Console.WriteLine(ex.Message);
            }
        }
        public async void SetQuantity(Button btn)
        {
            try
            {
                string amount = "0";
                if (!string.IsNullOrEmpty(editValue.Text))
                {
                    amount = editValue.Text;
                }
                else
                {
                    amount = "0";
                }

                btn.RequestFocus();
                var num = btn.Text.ToString();
                switch (num)
                {
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

                decimal data = 0, checkValue = 0;
                //if (Int32.TryParse(amount.Replace(",", ""), out data))
                if (Decimal.TryParse(amount, out data))
                {
                    if (data > 0)
                    {
                        checkValue = data;
                    }
                    else
                    {
                        checkValue = 0;
                    }
                }

                //var data = Convert.ToInt32(amount);
                if (checkValue > 999999)
                {
                    Toast.MakeText(this.Context, Application.Context.GetString(Resource.String.maxitem) + " 999,999", ToastLength.Short).Show();
                    editValue.Text = "999,999";
                    strcurrent = editValue.Text;
                    decimal value1 = 0;
                    //if (Int32.TryParse(strcurrent.Replace(",", ""), out value1))
                    if (Decimal.TryParse(strcurrent, out value1))
                    {
                        if (value1 > 0)
                        {
                            setQuanity = (int)value1;
                        }
                        else
                        {
                            setQuanity = 0;
                        }
                    }
                    return;
                }

                editValue.Text = checkValue.ToString("#,##0");
                strcurrent = editValue.Text;
                decimal value = 0;
                //if (Int32.TryParse(strcurrent.Replace(",",""), out value))
                if (Decimal.TryParse(strcurrent, out value))
                {
                    if (value > 0)
                    {
                        setQuanity = (int)value;
                    }
                    else
                    {
                        setQuanity = 0;
                    }
                }
                editValue.Focusable = true;
            }
            catch (Exception ex)
            {
                await TinyInsights.TrackErrorAsync(ex);
                _ = TinyInsights.TrackPageViewAsync("SetQuantity at Quantity");
                Toast.MakeText(this.Context, ex.Message, ToastLength.Long).Show();
                return;
            }
        }

    }


}