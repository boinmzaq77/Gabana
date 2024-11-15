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
    internal class Report_Adapter_ShowCategory : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public List<SalesByCategoryResponse> listTime;
        public string positionClick;
        private string CURRENCYSYMBOLS;

        public Report_Adapter_ShowCategory(List<SalesByCategoryResponse> l)
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
            vh.Time.Text = listTime[position].categoryName;
            vh.Amount.Text = CURRENCYSYMBOLS + " " + listTime[position].sumTotalAmount.ToString("#,##0.00");
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.report_adapter_showcategory, parent, false);
            ListViewTimeReporHolder vh = new ListViewTimeReporHolder(itemView, OnClick);
            CURRENCYSYMBOLS = DataCashingAll.setmerchantConfig.CURRENCY_SYMBOLS;
            return vh;
        }

        private void OnClick(int obj)
        {
            if (ItemClick != null)
                ItemClick(this, obj);
        }
    }

}