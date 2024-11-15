
using Android.Support.V7.Widget;
using Android.Views;
using Gabana.Droid.ListData;
using Gabana.Model;
using System;

namespace Gabana.Droid.Adapter
{
    public class Report_Adapter_Payment : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public ListPaymentType listPayment;
        public string positionClick;

        public Report_Adapter_Payment(ListPaymentType l)
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
                ListViewCustomerHolder vh = holder as ListViewCustomerHolder;
                //vh.CustomerName.Text = listPayment[position].Type?.ToString();

                //Utils.SetPaymentImage(vh.imageCustomer, listPayment[position].Type);
                vh.CustomerName.Text = Utils.SetPaymentNameRP(listPayment[position].Type);

                var index = ReportPaymentActivity.listChoosePayment.FindIndex(x => x.Type == listPayment[position].Type);
                if (index == -1)
                {
                    vh.SelectCustomer.Visibility = ViewStates.Invisible;
                }
                else
                {
                    vh.SelectCustomer.Visibility = ViewStates.Visible;
                }
                vh.imageCustomer.SetImageResource(listPayment[position].Logo);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.report_adapter_choosepayment, parent, false);
            ListViewCustomerHolder vh = new ListViewCustomerHolder(itemView, OnClick);
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }

    }
}