using Android.Support.V7.Widget;
using Android.Views;
using Gabana.Droid.ListData;
using Gabana.Model;
using Gabana3.JAM.Report;
using System;
using System.Collections.Generic;

namespace Gabana.Droid.Adapter
{
    public class Report_Adapter_ShowByBranch : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public List<SaleReportBranch> listSaleByBranch;
        public string positionClick;


        public Report_Adapter_ShowByBranch(List<SaleReportBranch> l)
        {
            listSaleByBranch = l;
        }
        public override int ItemCount
        {
            get { return listSaleByBranch == null ? 0 : listSaleByBranch.Count; }
        }

        public List<SalesByBranchModel> salesByBranchs { get; }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ListViewTimeReporHolder vh = holder as ListViewTimeReporHolder;

            vh.Time.Text = listSaleByBranch[position].BranchName?.ToString();
            vh.Amount.Text = listSaleByBranch[position].sumGrandTotal.ToString("#,##0.00");

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