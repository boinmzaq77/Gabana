using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Adapter.Payment
{
    internal class Payment_Adapter_ReceiptCash : RecyclerView.Adapter
    {

        public event EventHandler<int> ItemClick;
        private List<PaymentTypeAmount> paymenttypeamout;
        public Payment_Adapter_ReceiptCash(List<PaymentTypeAmount> p)
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
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.payment_adapter_receiptcash, parent, false);
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