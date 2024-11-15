using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Adapter.Payment
{
    internal class Payment_Adapter_Balance : RecyclerView.Adapter
    {
        ListPayment listPayment;
        public event EventHandler<int> ItemClick;

        public Payment_Adapter_Balance(ListPayment l)
        {
            listPayment = l;
        }
        public override int ItemCount
        {
            get { return listPayment == null ? 0 : listPayment.Count; }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                ListViewPaymentHolder vh = holder as ListViewPaymentHolder;
                Utils.SetPaymentImage(vh.ImgPayment, listPayment.tranPayments[position].PaymentType);
                vh.NamePayment.Text = Utils.SetPaymentName(listPayment.tranPayments[position].PaymentType);
                vh.Price.Text = Utils.DisplayDecimal(listPayment.tranPayments[position].PaymentAmount);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.payment_adapter_balance, parent, false);
            ListViewPaymentHolder vh = new ListViewPaymentHolder(itemView, OnClick);

            return vh;
        }
        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }

    }

    public class ListViewPaymentHolder : RecyclerView.ViewHolder
    {
        public ImageView ImgPayment { get; set; }
        public TextView NamePayment { get; set; }
        public TextView Price { get; set; }

        public ListViewPaymentHolder(View itemview, Action<int> listener) : base(itemview)
        {
            ImgPayment = itemview.FindViewById<ImageView>(Resource.Id.imgPayment);
            NamePayment = itemview.FindViewById<TextView>(Resource.Id.txtPaymentType);
            Price = itemview.FindViewById<TextView>(Resource.Id.txtPrice);



            itemview.Click += (sender, e) => listener(base.Position);


        }
    }
}