using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Adapter.Setting;
using Gabana.Droid.Tablet.Fragments.PayMent;
using Gabana.Droid.Tablet.Fragments.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Adapter.Payment
{
    internal class Payment_Adapter_GiftVoucher : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public ListGiftVoucher listvoucher;
        public string positionClick;
        public Payment_Adapter_GiftVoucher(ListGiftVoucher l)
        {
            listvoucher = l;
        }
        public override int ItemCount
        {
            get { return listvoucher == null ? 0 : listvoucher.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                ListViewGiftVoucherHolder vh = holder as ListViewGiftVoucherHolder;
                vh.GiftVoucherID.Text = listvoucher[position].GiftVoucherCode;
                vh.GiftVoucherName.Text = listvoucher[position].GiftVoucherName;
                vh.Discount.Text = listvoucher[position].FmlAmount;

                vh.Check.Visibility = ViewStates.Gone;
                if (Payment_Fragment_GiftVoucher.voucher != null)
                {
                    var voucher = Payment_Fragment_GiftVoucher.voucher;
                    if (voucher.GiftVoucherCode  == listvoucher[position].GiftVoucherCode)
                    {
                        vh.Check.Visibility = ViewStates.Visible;
                    }
                }

            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.Message, ToastLength.Short).Show();
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.payment_adapter_giftvoucher, parent, false);
            ListViewGiftVoucherHolder vh = new ListViewGiftVoucherHolder(itemView, OnClick);
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }

    }

}