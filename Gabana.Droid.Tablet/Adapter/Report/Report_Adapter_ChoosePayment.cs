using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Droid.Tablet.Dialog;
using Gabana.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Adapter.Report
{
    internal class Report_Adapter_ChoosePayment : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public ListPaymentType listPayment;
        public string positionClick;

        public Report_Adapter_ChoosePayment(ListPaymentType l)
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
                //vh.CustomerName.Text = listPayment[position].Type?.ToString();

                //Utils.SetPaymentImage(vh.imageCustomer, listPayment[position].Type);
                vh.PaymentName.Text = Utils.SetPaymentNameRP(listPayment[position].Type);

                var index = Report_Dialog_Payment.listChoosePayment.FindIndex(x => x.Type == listPayment[position].Type);
                if (index == -1)
                {
                    vh.ImgStatusSelect.Visibility = ViewStates.Invisible;
                }
                else
                {
                    vh.ImgStatusSelect.Visibility = ViewStates.Visible;
                }
                vh.ImgPayment.SetImageResource(listPayment[position].Logo);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.report_adapter_choosepayment, parent, false);
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
        public TextView PaymentName { get; set; }
        public ImageButton ImgStatusSelect { get; set; }
        public ImageView ImgPayment { get; set; }

        public ListViewPaymentHolder(View itemview, Action<int> listener) : base(itemview)
        {
            PaymentName = itemview.FindViewById<TextView>(Resource.Id.PaymentName);
            ImgStatusSelect = itemview.FindViewById<ImageButton>(Resource.Id.imgSelect);
            ImgPayment = itemview.FindViewById<ImageView>(Resource.Id.ImgPayment);


            itemview.Click += (sender, e) => listener(base.Position);

        }
    }
}