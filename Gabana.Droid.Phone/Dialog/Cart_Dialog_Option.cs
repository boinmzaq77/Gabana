using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using Gabana.ShareSource;
using System;
using System.Linq;

namespace Gabana.Droid.Phone
{
    public class Cart_Dialog_Option : Android.Support.V4.App.DialogFragment
    {
        LinearLayout lnaddRemark, lnClearCart, lndeleteRemark, lndeleteDiscount;
        FrameLayout lnRemark, lnAddDiscount;
        ImageView imgDeleteRemark, imgDeleteDiscount;
        TextView txtcancel, txtaddRemark, txtAddDiscount, txtClearCart, txtShowRemark, txtShowDiscount;
        static TranWithDetailsLocal tranWithDetails;
        string CartDiscount = "";

        public override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        public static Cart_Dialog_Option NewInstance()
        {
            var frag = new Cart_Dialog_Option { Arguments = new Bundle() };
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.cart_dialog_option, container, false);
            try
            {
                Dialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.Transparent);
                lnaddRemark = view.FindViewById<LinearLayout>(Resource.Id.lnaddRemark);
                lnAddDiscount = view.FindViewById<FrameLayout>(Resource.Id.lnAddDiscount);
                lnClearCart = view.FindViewById<LinearLayout>(Resource.Id.lnClearCart);
                lndeleteRemark = view.FindViewById<LinearLayout>(Resource.Id.lndeleteRemark);
                lnRemark = view.FindViewById<FrameLayout>(Resource.Id.lnRemark);
                lndeleteDiscount = view.FindViewById<LinearLayout>(Resource.Id.lndeleteDiscount);

                txtcancel = view.FindViewById<TextView>(Resource.Id.txtcancel);
                txtShowRemark = view.FindViewById<TextView>(Resource.Id.txtShowRemark);
                txtaddRemark = view.FindViewById<TextView>(Resource.Id.txtaddRemark);
                txtaddRemark = view.FindViewById<TextView>(Resource.Id.txtaddRemark);
                txtAddDiscount = view.FindViewById<TextView>(Resource.Id.txtAddDiscount);
                txtClearCart = view.FindViewById<TextView>(Resource.Id.txtClearCart);
                txtShowDiscount = view.FindViewById<TextView>(Resource.Id.txtShowDiscount);
                imgDeleteRemark = view.FindViewById<ImageView>(Resource.Id.imgDeleteRemark);
                imgDeleteDiscount = view.FindViewById<ImageView>(Resource.Id.imgDeleteDiscount);

                txtcancel.Click += Txtcancel_Click;
                lnaddRemark.Click += LnaddRemark_Click;
                lnAddDiscount.Click += LnAddDiscount_Click;
                lnClearCart.Click += LnClearCart_Click;
                txtaddRemark.Click += LnaddRemark_Click;
                txtAddDiscount.Click += LnAddDiscount_Click;
                txtClearCart.Click += LnClearCart_Click;
                lndeleteRemark.Click += LndeleteRemark_Click;
                imgDeleteRemark.Click += LndeleteRemark_Click;
                lnRemark.Click += LnRemark_Click;
                lndeleteDiscount.Click += LndeleteDiscount_Click;
                imgDeleteDiscount.Click += LndeleteDiscount_Click;


                if (!string.IsNullOrEmpty(tranWithDetails.tran.Comments))
                {
                    lnaddRemark.Visibility = ViewStates.Gone;
                    lnRemark.Visibility = ViewStates.Visible;
                    txtShowRemark.Text = (tranWithDetails.tran.Comments != null) ? tranWithDetails.tran.Comments : "";
                }
                else
                {
                    lnRemark.Visibility = ViewStates.Gone;
                }

                //lnAddDiscount ลบ discount จากบิล
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
                        txtShowDiscount.Text = discount + "%";
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

        private void LndeleteDiscount_Click(object sender, EventArgs e)
        {
            try
            {
                imgDeleteDiscount.Visibility = ViewStates.Gone;
                lndeleteDiscount.Visibility = ViewStates.Gone;
                txtShowDiscount.Visibility = ViewStates.Gone;

                Intent intent;

                //Case เปิด cart และ cartscan ทั้ง 2 หน้า
                if (CartActivity.cart != null)
                {
                    CartActivity.addDiscount = false;
                }

                if (CartScanActivity.scan != null)
                {
                    CartScanActivity.addDiscount = false;
                }

                //Case เปิด cart หรือ cartscan หน้าเดียว
                if (CartActivity.CurrentActivity)
                {
                    var tranTradDiscount = tranWithDetails.tranTradDiscounts.Where(x => x.DiscountType == "MD").FirstOrDefault();
                    if (tranTradDiscount != null)
                    {
                        var data = tranWithDetails.tranTradDiscounts.FindIndex(x => x.TradDiscountNo == tranTradDiscount.TradDiscountNo);
                        tranWithDetails.tranTradDiscounts.RemoveAt(data);
                        tranWithDetails = BLTrans.Caltran(tranWithDetails);
                        CartActivity.SetTranDetail(tranWithDetails);
                        intent = new Intent(Application.Context, typeof(CartActivity));
                        StartActivity(intent);
                        Dismiss();
                    }
                }
                else
                {
                    var tranTradDiscount = tranWithDetails.tranTradDiscounts.Where(x => x.DiscountType == "MD").FirstOrDefault();
                    if (tranTradDiscount != null)
                    {
                        var data = tranWithDetails.tranTradDiscounts.FindIndex(x => x.TradDiscountNo == tranTradDiscount.TradDiscountNo);
                        tranWithDetails.tranTradDiscounts.RemoveAt(data);
                        tranWithDetails = BLTrans.Caltran(tranWithDetails);
                        CartActivity.SetTranDetail(tranWithDetails);
                        intent = new Intent(Application.Context, typeof(CartScanActivity));
                        StartActivity(intent);
                        Dismiss();
                    }
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();
            }
        }
        private void LnRemark_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(Application.Context, typeof(AddRemarkActivitycs));
            AddRemarkActivitycs.SetTranDetail(tranWithDetails);
            StartActivity(intent);
            Dismiss();
            lnRemark.Visibility = ViewStates.Visible;
            lnaddRemark.Visibility = ViewStates.Gone;
        }
        private void LndeleteRemark_Click(object sender, EventArgs e)
        {
            lnRemark.Visibility = ViewStates.Gone;
            lnaddRemark.Visibility = ViewStates.Visible;

            Intent intent;

            //Case เปิด cart หรือ cartscan หน้าเดียว
            if (CartActivity.CurrentActivity)
            {
                //ลบ Remark ออกจาก TranWithDetail
                tranWithDetails.tran.Comments = null;
                //CartActivity.addRemark = false;
                CartActivity.SetTranDetail(tranWithDetails);
                intent = new Intent(Application.Context, typeof(CartActivity));
                StartActivity(intent);
                Dismiss();
            }
            else
            {
                //ลบ Remark ออกจาก TranWithDetail
                tranWithDetails.tran.Comments = null;
                // CartScanActivity.addRemark = false;
                CartScanActivity.SetTranDetail(tranWithDetails);
                intent = new Intent(Application.Context, typeof(CartScanActivity));
                StartActivity(intent);
                Dismiss();
            }
        }
        private void LnClearCart_Click(object sender, EventArgs e)
        {

            if (CartActivity.CurrentActivity)
            {
                CartActivity.cart.DialogClearCart();
                Cart_Dialog_ClearCart.SetTranDetail(tranWithDetails);
                Dismiss();
            }
            else
            {
                CartScanActivity.scan.DialogClearCart();
                Cart_Dialog_ClearCart.SetTranDetail(tranWithDetails);
                Dismiss();
            }

        }
        private void LnAddDiscount_Click(object sender, EventArgs e)
        {
            if (CartActivity.CurrentActivity)
            {
                StartActivity(new Intent(Application.Context, typeof(AddDiscountActivity)));
                AddDiscountActivity.SetTranDetail(tranWithDetails);
                AddDiscountActivity.CartDiscount = true;
                CartActivity.addDiscount = true;
                Dismiss();
            }
            else
            {
                StartActivity(new Intent(Application.Context, typeof(AddDiscountActivity)));
                AddDiscountActivity.SetTranDetail(tranWithDetails);
                AddDiscountActivity.CartDiscount = true;
                CartScanActivity.addDiscount = true;
                Dismiss();
            }
        }
        private void LnaddRemark_Click(object sender, EventArgs e)
        {
            if (CartActivity.CurrentActivity)
            {
                Intent intent = new Intent(Application.Context, typeof(AddRemarkActivitycs));
                AddRemarkActivitycs.SetTranDetail(tranWithDetails);
                StartActivity(intent);
                Dismiss();
                lnRemark.Visibility = ViewStates.Visible;
                lnaddRemark.Visibility = ViewStates.Gone;
            }
            else
            {
                Intent intent = new Intent(Application.Context, typeof(AddRemarkActivitycs));
                AddRemarkActivitycs.SetTranDetail(tranWithDetails);
                StartActivity(intent);
                Dismiss();
                lnRemark.Visibility = ViewStates.Visible;
                lnaddRemark.Visibility = ViewStates.Gone;
            }
        }
        private void Txtcancel_Click(object sender, EventArgs e)
        {
            Dismiss();
        }
        public static void SetTranDetail(TranWithDetailsLocal t)
        {
            tranWithDetails = t;
        }

    }
}