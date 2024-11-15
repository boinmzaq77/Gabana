using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Gabana.Model;
using System;

namespace Gabana.Droid
{
    public class DialogOptionCart : Android.Support.V4.App.DialogFragment
    {
        LinearLayout lnaddRemark, lnAddDiscount, lnClearCart, lndeleteRemark;
        FrameLayout lnRemark;
        ImageView imgDeleteRemark;
        TextView txtcancel, txtaddRemark, txtAddDiscount, txtClearCart, txtShowRemark;

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

        public static DialogOptionCart NewInstance()
        {
            var frag = new DialogOptionCart { Arguments = new Bundle() };
            return frag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.dialog_option_cart, container, false);
            try
            {
                lnaddRemark = view.FindViewById<LinearLayout>(Resource.Id.lnaddRemark);
                lnAddDiscount = view.FindViewById<LinearLayout>(Resource.Id.lnAddDiscount);
                lnClearCart = view.FindViewById<LinearLayout>(Resource.Id.lnClearCart);
                lndeleteRemark = view.FindViewById<LinearLayout>(Resource.Id.lndeleteRemark);
                lnRemark = view.FindViewById<FrameLayout>(Resource.Id.lnRemark);

                txtcancel = view.FindViewById<TextView>(Resource.Id.txtcancel);
                txtShowRemark = view.FindViewById<TextView>(Resource.Id.txtShowRemark);
                txtaddRemark = view.FindViewById<TextView>(Resource.Id.txtaddRemark);
                txtaddRemark = view.FindViewById<TextView>(Resource.Id.txtaddRemark);
                txtAddDiscount = view.FindViewById<TextView>(Resource.Id.txtAddDiscount);
                txtClearCart = view.FindViewById<TextView>(Resource.Id.txtClearCart);
                imgDeleteRemark = view.FindViewById<ImageView>(Resource.Id.imgDeleteRemark);


                txtcancel.Click += Txtcancel_Click;
                lnaddRemark.Click += LnaddRemark_Click;
                lnAddDiscount.Click += LnAddDiscount_Click;
                lnClearCart.Click += LnClearCart_Click;
                txtaddRemark.Click += LnaddRemark_Click;
                txtAddDiscount.Click += LnAddDiscount_Click;
                txtClearCart.Click += LnClearCart_Click;
                lndeleteRemark.Click += LndeleteRemark_Click;
                imgDeleteRemark.Click += LndeleteRemark_Click;

                

                if (CartActivity.addRemark)
                {
                    lnaddRemark.Visibility = ViewStates.Gone;
                    lnRemark.Visibility = ViewStates.Visible;

                    txtShowRemark.Text = (PosActivity.tranWithDetails.tran.Comments != null) ? PosActivity.tranWithDetails.tran.Comments :  "" ;
                }
                else
                {
                    lnRemark.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this.Activity, ex.Message, ToastLength.Short).Show();

            }
            return view;
        }

        private void LndeleteRemark_Click(object sender, EventArgs e)
        {
            lnRemark.Visibility = ViewStates.Gone;
            lnaddRemark.Visibility = ViewStates.Visible;

            //ลบ Remark ออกจาก TranWithDetail
            PosActivity.tranWithDetails.tran.Comments = null;
            CartActivity.addRemark = false;
            CartActivity.SetTranDetail(PosActivity.tranWithDetails);
            Intent intent = new Intent(Context, typeof(CartActivity));
            StartActivity(intent);
            Dismiss();
        }

        private void LnClearCart_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(Context, typeof(CartActivity));
            PosActivity.tranWithDetails = null;
            CartActivity.SetTranDetail(PosActivity.tranWithDetails);
            CartActivity.clearcart = true;
            StartActivity(intent);
            Dismiss();
        }

        private void LnAddDiscount_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(Context, typeof(AddDiscountActivity));
            CartActivity.showdiscount = true;
            StartActivity(intent);
        }

        private void LnaddRemark_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(Context, typeof(AddRemarkActivitycs));
            CartActivity.addRemark = true;
            AddRemarkActivitycs.SetTranDetail(PosActivity.tranWithDetails);
            StartActivity(intent);
            Dismiss();
            lnRemark.Visibility = ViewStates.Visible;
            lnaddRemark.Visibility = ViewStates.Gone;
        }

        private void Txtcancel_Click(object sender, EventArgs e)
        {
            Dismiss();
        }

    }
}