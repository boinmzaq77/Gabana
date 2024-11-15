using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Model;
using Gabana.ShareSource;
using Gabana3.JAM.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Adapter.Report
{
    internal class Report_Adapter_ShowPayment : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public List<SalesByPaymentResponse> listTime;
        public string positionClick;
        private string CURRENCYSYMBOLS;
        List<Microcharts.ChartEntry> salesByPayments;
        public Report_Adapter_ShowPayment(List<SalesByPaymentResponse> l, List<Microcharts.ChartEntry> s)
        {
            listTime = l;
            salesByPayments = s;
        }


        public override int ItemCount
        {
            get { return listTime == null ? 0 : listTime.Count; }
        }

        public List<ReportHourly> ReportHourlies { get; }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            try
            {
                ListViewPaymentReporHolder vh = holder as ListViewPaymentReporHolder;
                vh.Type.Text = Utils.SetPaymentNameRP(listTime[position].paymentType);
                var res = salesByPayments.Where(x => x.Label == Utils.SetPaymentNameChart(listTime[position].paymentType)).FirstOrDefault();
                var color = res.Color.ToString();
                vh.Image.SetBackgroundColor(Android.Graphics.Color.ParseColor(color));
                vh.Amount.Text = CURRENCYSYMBOLS + " " + listTime[position].sumTotalAmount.ToString("#,##0.00");
                vh.Percent.Text = listTime[position].percentTotal.ToString("#,##0.00") + "%";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.report_adapter_payment, parent, false);
            ListViewPaymentReporHolder vh = new ListViewPaymentReporHolder(itemView, OnClick);
            CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }

    }
    public class ListViewPaymentReporHolder : RecyclerView.ViewHolder
    {
        public ImageView Image { get; set; }
        public TextView Type { get; set; }
        public TextView Amount { get; set; }
        public TextView Percent { get; set; }

        public ListViewPaymentReporHolder(View itemview, Action<int> listener) : base(itemview)
        {
            Image = itemview.FindViewById<ImageView>(Resource.Id.imageColor);
            Type = itemview.FindViewById<TextView>(Resource.Id.txtType);
            Amount = itemview.FindViewById<TextView>(Resource.Id.txtAmount);
            Percent = itemview.FindViewById<TextView>(Resource.Id.txtPercent);



            itemview.Click += (sender, e) => listener(base.Position);


        }
    }
}