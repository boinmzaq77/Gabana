using Android.Support.V7.Widget;
using Android.Views;
using Gabana.Droid.ListData;
using Gabana.Model;
using Gabana.ShareSource;
using Gabana3.JAM.Report;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gabana.Droid.Adapter
{
    public class Report_Adapter_ShowCustomer : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public List<SalesByCustomerResponse> listTime;
        public string positionClick;
        private string CURRENCYSYMBOLS;
        List<ORM.MerchantDB.Customer> customers;
        public Report_Adapter_ShowCustomer(List<SalesByCustomerResponse> l, List<ORM.MerchantDB.Customer> listCustomer)
        {
            listTime = l;
            customers = listCustomer;
        }


        public override int ItemCount
        {
            get { return listTime == null ? 0 : listTime.Count; }
        }

        public List<ReportHourly> ReportHourlies { get; }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            ListViewTimeReporHolder vh = holder as ListViewTimeReporHolder;

            vh.Time.Text = listTime[position].customerName;
            vh.Amount.Text = CURRENCYSYMBOLS + " " + listTime[position].sumTotalAmount.ToString("#,##0.00");

            var customer = customers.Where(x => x.CustomerName.Replace("\u200b", "") == listTime[position].customerName.Replace("\u200b", "")).FirstOrDefault();
            if (customer == null)
            {
                return;
            }

            var cloudpath = customer.PicturePath == null ? string.Empty : customer.PicturePath;
            var localpath = customer.PictureLocalPath == null ? string.Empty : customer.PictureLocalPath;

            if (string.IsNullOrEmpty(localpath))
            {
                if (string.IsNullOrEmpty(cloudpath))
                {
                    //defalut
                    vh.Profile.SetBackgroundResource(Resource.Mipmap.defaultcust);
                }
                else
                {
                    //cloud
                    Utils.SetImage(vh.Profile, cloudpath);
                }
            }
            else
            {
                //local
                Android.Net.Uri uri = Android.Net.Uri.Parse(localpath);
                vh.Profile.SetImageURI(uri);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.report_adapter_customer, parent, false);
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