﻿using Android.Support.V7.Widget;
using Android.Views;
using Gabana.Droid.ListData;
using Gabana.Model;
using System;
using System.Collections.Generic;

namespace Gabana.Droid.Adapter
{
    public class Report_Adapter_ShowCustom : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public List<ReportProfit> listTime;
        public string positionClick;
        string TypeReport;
        public Report_Adapter_ShowCustom(List<ReportProfit> l, string t)
        {
            listTime = l;
            TypeReport = t;
        }
        public override int ItemCount
        {
            get { return listTime == null ? 0 : listTime.Count; }
        }

        public List<ReportHourly> ReportHourlies { get; }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ListViewTimeReporHolder vh = holder as ListViewTimeReporHolder;
            vh.Time.Text = listTime[position].dateTime;
            if (TypeReport == "SalesReport")
            {
                vh.Amount.Text = listTime[position].sumGrandTotal.ToString("#,##0.00");

            }
            if (TypeReport == "ProfitReport")
            {
                vh.Amount.Text = listTime[position].sumProfitTotal.ToString("#,##0.00");
            }
        }
        private int i = 0;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.report_adapter_main, parent, false);
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
}