using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Gabana.Droid.Tablet.Fragments.POS;
using Gabana.Model;
using Gabana.ORM.MerchantDB;
using Gabana.ShareSource;
using Gabana3.JAM.Trans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Dialog
{
    public class Cart_Dialog_Option : AndroidX.Fragment.App.DialogFragment
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

        public static Cart_Dialog_Option cart_optiion;
        View view;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.cart_dialog_option, container, false);
            try
            {
                //Dialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.Transparent);
                cart_optiion = this;
                tranWithDetails = MainActivity.tranWithDetails;
                CombinUI();                

                double discount;
                double disDiscont = 0.0;
                var tranTradDiscount = tranWithDetails.tranTradDiscounts.Where(x => x.DiscountType == "MD").FirstOrDefault();
                if (tranTradDiscount != null)
                {
                    imgDeleteDiscount.Visibility = ViewStates.Visible;
                    lndeleteDiscount.Visibility = ViewStates.Visible;
                    txtShowDiscount.Visibility = ViewStates.Visible;

                    var CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
                    var check = tranTradDiscount.FmlDiscount.IndexOf('%');
                    if (check == -1)
                    {
                        CartDiscount = tranTradDiscount.FmlDiscount;
                        discount = Convert.ToDouble(CartDiscount);
                        disDiscont = discount;
                        txtShowDiscount.Text = CURRENCYSYMBOLS + Utils.DisplayDecimal(Convert.ToDecimal(disDiscont));
                    }
                    else
                    {

                        discount = Convert.ToDouble(tranTradDiscount.FmlDiscount.Remove(check));
                        txtShowDiscount.Text = Utils.DisplayDecimal(Convert.ToDecimal(discount)) + "%";
                    }
                }
                else
                {
                    imgDeleteDiscount.Visibility = ViewStates.Gone;
                    lndeleteDiscount.Visibility = ViewStates.Gone;
                    txtShowDiscount.Visibility = ViewStates.Gone;
                }

            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }

            return view;
        }
        LinearLayout lnClearCart, lndeleteRemark, lndeleteDiscount;
        FrameLayout lnRemark, lnAddDiscount;
        ImageView imgDeleteRemark, imgDeleteDiscount;
        TextView textDone, txtAddDiscount, txtClearCart, txtShowDiscount, txtRemark;
        EditText edtRemark;
        static TranWithDetailsLocal tranWithDetails;
        string CartDiscount = "";
        private void CombinUI()
        {
            lnAddDiscount = view.FindViewById<FrameLayout>(Resource.Id.lnAddDiscount);
            lnClearCart = view.FindViewById<LinearLayout>(Resource.Id.lnClearCart);
            lndeleteRemark = view.FindViewById<LinearLayout>(Resource.Id.lndeleteRemark);
            lnRemark = view.FindViewById<FrameLayout>(Resource.Id.lnRemark);
            lndeleteDiscount = view.FindViewById<LinearLayout>(Resource.Id.lndeleteDiscount);
            textDone = view.FindViewById<TextView>(Resource.Id.txtDone);
            txtRemark = view.FindViewById<TextView>(Resource.Id.txtRemark);
            edtRemark = view.FindViewById<EditText>(Resource.Id.edtRemark);
            txtAddDiscount = view.FindViewById<TextView>(Resource.Id.txtAddDiscount);
            txtClearCart = view.FindViewById<TextView>(Resource.Id.txtClearCart);
            txtShowDiscount = view.FindViewById<TextView>(Resource.Id.txtShowDiscount);
            imgDeleteRemark = view.FindViewById<ImageView>(Resource.Id.imgDeleteRemark);
            imgDeleteDiscount = view.FindViewById<ImageView>(Resource.Id.imgDeleteDiscount);

            textDone.Click += TextDone_Click; 
            lnAddDiscount.Click += LnAddDiscount_Click;            
            txtAddDiscount.Click += LnAddDiscount_Click;
            lnClearCart.Click += LnClearCart_Click;
            txtClearCart.Click += LnClearCart_Click;
            lndeleteRemark.Click += LndeleteRemark_Click;
            imgDeleteRemark.Click += LndeleteRemark_Click;
            lnRemark.Click += LnRemark_Click;
            txtRemark.Click += LnRemark_Click;
            lndeleteDiscount.Click += LndeleteDiscount_Click;
            imgDeleteDiscount.Click += LndeleteDiscount_Click;
            edtRemark.Text = tranWithDetails.tran.Comments;
            if (string.IsNullOrEmpty(edtRemark.Text))
            {
                lndeleteRemark.Visibility = ViewStates.Gone;
                edtRemark.Visibility = ViewStates.Gone;
            }
            else
            {
                lndeleteRemark.Visibility = ViewStates.Visible;
                edtRemark.Visibility = ViewStates.Visible;
            }
            edtRemark.TextChanged += EdtRemark_TextChanged;
        }

        private void TextDone_Click(object sender, EventArgs e)
        {
            POS_Fragment_Cart.fragment_cart.OnResume();
            Dismiss();
        }

        private void EdtRemark_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
           tranWithDetails.tran.Comments = edtRemark.Text;
        }

        private void LndeleteDiscount_Click(object sender, EventArgs e)
        {
            try
            {
                imgDeleteDiscount.Visibility = ViewStates.Gone;
                lndeleteDiscount.Visibility = ViewStates.Gone;
                txtShowDiscount.Visibility = ViewStates.Gone;

                POS_Fragment_Cart.addDiscount = false;
                //Case เปิด cart หรือ cartscan หน้าเดียว

                TranTradDiscount tranTradDiscount;
                tranTradDiscount = tranWithDetails.tranTradDiscounts.Where(x => x.DiscountType == "MD").FirstOrDefault();
                if (tranTradDiscount != null)
                {
                    var data = tranWithDetails.tranTradDiscounts.FindIndex(x => x.TradDiscountNo == tranTradDiscount.TradDiscountNo);
                    tranWithDetails.tranTradDiscounts.RemoveAt(data);
                    tranWithDetails = BLTrans.Caltran(tranWithDetails);
                    MainActivity.tranWithDetails = tranWithDetails;
                    POS_Fragment_Main.fragment_main.OnResume();
                    POS_Fragment_Cart.fragment_cart.OnResume();                    
                }               

                if (POS_Dialog_Scan.scan != null)
                {
                    POS_Dialog_Scan.scan.OnResume();    
                }
                Dismiss();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }
        private void LnRemark_Click(object sender, EventArgs e)
        {
            try
            {
                edtRemark.Visibility = ViewStates.Visible;
                edtRemark.SetFocusable(ViewFocusability.FocusableAuto);
                edtRemark.SetCursorVisible(true);
                edtRemark.RequestFocus();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity,ex.Message,ToastLength.Short);
            }
        }
        private void LndeleteRemark_Click(object sender, EventArgs e)
        {
            lnRemark.Visibility = ViewStates.Gone;
            POS_Fragment_Cart.addRemark = false;    
            
            //ลบ Remark ออกจาก TranWithDetail
            tranWithDetails.tran.Comments = null;
            MainActivity.tranWithDetails = tranWithDetails;
            POS_Fragment_Main.fragment_main.OnResume();
            POS_Fragment_Cart.fragment_cart.OnResume();            

            if (POS_Dialog_Scan.scan != null)
            {
                POS_Dialog_Scan.scan.OnResume();
            }
        }
        private void LnClearCart_Click(object sender, EventArgs e)
        {
            var fragment = new Cart_Dialog_ClearCart();
            fragment.Show(Activity.SupportFragmentManager, nameof(Cart_Dialog_ClearCart));
        }
        private void LnAddDiscount_Click(object sender, EventArgs e)
        {
            //ส่วนลดท้ายบิล           
            POS_Dialog_Discount.CartDiscount = true;
            var fragment = new POS_Dialog_Discount();
            fragment.Show(Activity.SupportFragmentManager, nameof(POS_Dialog_Discount));   
        }

        private void Txtcancel_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

    }


}