using Android.Support.V7.Widget;
using Android.Views;
using Gabana.Droid.ListData;
using System;

namespace Gabana.Droid.Adapter
{
    class Balance_Adapter_Main : RecyclerView.Adapter
    {
        ListPayment listPayment;
        public event EventHandler<int> ItemClick;


        public Balance_Adapter_Main(ListPayment l)
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
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.balance_adapter_main, parent, false);
            ListViewPaymentHolder vh = new ListViewPaymentHolder(itemView, OnClick);

            return vh;
        }
        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }
    }
}