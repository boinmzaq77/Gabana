using Android.Support.V7.Widget;
using Android.Views;
using Gabana.Droid.ListData;
using Gabana.Model;
using System;
using System.Collections.Generic;

namespace Gabana.Droid.Adapter
{
    public class Receipt_Adapter_Cash : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        private List<PaymentTypeAmount> paymenttypeamout;

        public Receipt_Adapter_Cash(List<PaymentTypeAmount> p)
        {
            paymenttypeamout = p;
        }
        public override int ItemCount
        {
            get { return paymenttypeamout == null ? 0 : paymenttypeamout.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                ListViewReceiptHolder vh = holder as ListViewReceiptHolder;
                vh.txtNameItem.Text = paymenttypeamout[position].DetailType;
                vh.txtPrice.Text = Utils.DisplayDecimal(paymenttypeamout[position].amount);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.receipt_adapter_cash, parent, false);
            ListViewReceiptHolder vh = new ListViewReceiptHolder(itemView, OnClick);

            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }
    }
}