using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.CardView.Widget;
using AndroidX.RecyclerView.Widget;
using Gabana.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gabana.Droid.Tablet.Adapter.Report
{
    internal class Report_Adapter_ShowHour : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public List<ReportHourly> listTime;
        public string positionClick;


        public Report_Adapter_ShowHour(List<ReportHourly> l)
        {
            listTime = l;
        }
        public override int ItemCount
        {
            get { return listTime == null ? 0 : listTime.Count; }
        }

        public List<ReportHourly> ReportHourlies { get; }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ListViewTimeReporHolder vh = holder as ListViewTimeReporHolder;
            vh.Time.Text = listTime[position].Hourlyname;
            vh.Amount.Text = listTime[position].value.ToString("#,##0.00");

        }
        private int i = 0;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.report_adapter_time, parent, false);
            ListViewTimeReporHolder vh = new ListViewTimeReporHolder(itemView, OnClick);
            CardView cardView1 = itemView.FindViewById<CardView>(Resource.Id.cardView1);
            if (i % 2 != 0)
            {
                cardView1.SetBackgroundColor(Android.Graphics.Color.AliceBlue);
            }
            else
            {
                cardView1.SetBackgroundColor(Android.Graphics.Color.White);
            }
            i++;
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }
    }
    public class ListViewTimeReporHolder : RecyclerView.ViewHolder
    {
        public TextView Time { get; set; }
        public TextView Amount { get; set; }

        public ListViewTimeReporHolder(View itemview, Action<int> listener) : base(itemview)
        {
            Time = itemview.FindViewById<TextView>(Resource.Id.txtDate);
            Amount = itemview.FindViewById<TextView>(Resource.Id.txtAmount);

            itemview.Click += (sender, e) => listener(base.Position);


        }
    }

}