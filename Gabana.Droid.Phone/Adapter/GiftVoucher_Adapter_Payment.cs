using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Gabana.Droid.ListData;
using System;

namespace Gabana.Droid.Adapter
{
    public class GiftVoucher_Adapter_Payment : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public ListGiftVoucher listvoucher;
        public string positionClick;


        public GiftVoucher_Adapter_Payment(ListGiftVoucher l)
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

                vh.Check.Visibility = ViewStates.Invisible;
                if (GiftVoucherPaymentActivity.voucher != null)
                {
                    ORM.MerchantDB.GiftVoucher voucher = new ORM.MerchantDB.GiftVoucher();
                    voucher = GiftVoucherPaymentActivity.voucher;
                    if (voucher.Equals(listvoucher[position]))
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
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.giftvoucher_adapter_payment, parent, false);
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